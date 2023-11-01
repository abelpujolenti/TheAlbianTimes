using System;
using UnityEngine;

namespace Layout
{
    public class Workspace : MonoBehaviour
    {
        private const int PADDING = 5;
        private const int MIN_ANCHOR_X = 0;
        private const int MAX_ANCHOR_X = 1;
        private const int MIN_ANCHOR_Y = 1;
        private const int MAX_ANCHOR_Y = 0;

        [SerializeField] private RectTransform _rectTransform;
        
        [SerializeField] private int _columns;
        [SerializeField] private int _rows;

        [SerializeField] private GameObject _cellPrefab;

        private Cell[][] _cells;
        
        private Vector2[][] _cellsPositions;

        private readonly Vector3[] _layoutCorners = new Vector3[4];

        private Vector2 _minAnchorForCell;
        private Vector2 _maxAnchorForCell;
        private Vector2 _layoutMinCoordinates;
        private Vector2 _layoutMaxCoordinates;

        private float _cellSize;

        private void OnEnable()
        {
            ActionsManager.OnPreparingCells += TakeCells;
            ActionsManager.OnSuccessFul += SnapNewsHeadline;
        }

        private void OnDisable()
        {
            ActionsManager.OnPreparingCells -= TakeCells;
            ActionsManager.OnSuccessFul -= SnapNewsHeadline;
        }

        private Vector2 ModifySizeDelta()
        {

            Vector2 sizeDelta = _rectTransform.sizeDelta;

            _cellSize = _rectTransform.rect.height / _rows;

            sizeDelta.x = _cellSize * _columns;

            _rectTransform.sizeDelta = new Vector2(sizeDelta.x, sizeDelta.y);

            return sizeDelta;
        }

        void Start()
        {
            
            _cells = new Cell[_columns][];
            _cellsPositions = new Vector2[_columns][];

            for (int i = 0; i < _columns; i++)
            {
                _cells[i] = new Cell[_rows];
                _cellsPositions[i] = new Vector2[_rows];
            }

            Vector2 sizeDelta = ModifySizeDelta();
            
            _rectTransform.GetWorldCorners(_layoutCorners);
            
            SetLayoutLimiters();

            DefineMinMaxAnchors(sizeDelta);

            for (int i = 0; i < _columns; i++)
            {
                for (int j = 0; j < _rows; j++)
                {
                    CreateCell(i, j, _cellSize, sizeDelta);
                }
            }
        }

        private void SetLayoutLimiters()
        {
            _layoutMinCoordinates.x = _layoutCorners[0].x;
            _layoutMinCoordinates.y = _layoutCorners[1].y;
            _layoutMaxCoordinates.x = _layoutCorners[2].x;
            _layoutMaxCoordinates.y = _layoutCorners[3].y;
        }

        private void DefineMinMaxAnchors(Vector2 sizeDelta)
        {
            _minAnchorForCell.x = Map(_cellSize - _cellSize / 2, 0, sizeDelta.x, MIN_ANCHOR_X, MAX_ANCHOR_X);
            _maxAnchorForCell.x = 1 + _minAnchorForCell.x;

            _minAnchorForCell.y = Map(sizeDelta.y - (_cellSize - _cellSize / 2), sizeDelta.y, 0, MIN_ANCHOR_Y, MAX_ANCHOR_Y);
            _maxAnchorForCell.y = _minAnchorForCell.y - 1;
        }

        private void CreateCell(int i, int j, float cellSize, Vector2 sizeDelta)
        {
            GameObject cellGameObject = Instantiate(_cellPrefab, gameObject.transform, true);
            Cell cellPrefab = cellGameObject.GetComponent<Cell>();
            float cellPositionX = Map(cellSize * i, 0, sizeDelta.x, _minAnchorForCell.x, _maxAnchorForCell.x);
            float cellPositionY = Map(cellSize * j, 0, sizeDelta.y, _minAnchorForCell.y, _maxAnchorForCell.y);
            cellPrefab.SetPosition(new Vector2(cellPositionX, cellPositionY), cellSize - PADDING);
            cellPrefab.SetCoordinates(i, j);
            _cells[i][j] = cellPrefab;
            _cellsPositions[i][j] = cellPrefab.transform.position;
        }

        private float Map(float value, float originalMin, float originalMax, float newMin, float newMax)
        {
            return newMin + (value - originalMin) * (newMax - newMin) / (originalMax - originalMin);
        }

        private bool IsCoordinateInsideLayout(Vector2 worldCoordinate)
        {
            return worldCoordinate.x > _layoutMinCoordinates.x && worldCoordinate.x < _layoutMaxCoordinates.x &&
                   worldCoordinate.y < _layoutMinCoordinates.y && worldCoordinate.y > _layoutMaxCoordinates.y;
        }

        private Cell[] TakeCells(NewsHeadlinePiece draggedPiece, Vector2 mousePosition, NewsHeadlinePiece[] newsHeadlinePieces)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            if (!IsCoordinateInsideLayout(mousePosition))
            {
                return null;
            }
            
            return LookForCells(draggedPiece, mousePosition, newsHeadlinePieces);
        }

        private Cell[] LookForCells(NewsHeadlinePiece draggedPiece, Vector2 mousePosition, NewsHeadlinePiece[] newsHeadlinePieces)
        {

            foreach (NewsHeadlinePiece piece in newsHeadlinePieces)
            {
                if (!IsCoordinateInsideLayout(piece.transform.position))
                {
                    return null;
                }
            }
            
            Cell[] cells = new Cell[newsHeadlinePieces.Length];

            int index = LookingForPieceInsideArray(draggedPiece, newsHeadlinePieces);
            
            cells[index] = LookForCellForDraggedPiece(mousePosition);

            return LookForCellsForNeighborPieces(index, draggedPiece, cells, newsHeadlinePieces);
        }

        private int LookingForPieceInsideArray(NewsHeadlinePiece draggedPiece, NewsHeadlinePiece[] allPieces)
        {
            int index = 0;
            
            for (; index < allPieces.Length; index++)
            {
                if (draggedPiece == allPieces[index])
                {
                    break;
                }
            }

            return index;
        }

        private Cell LookForCellForDraggedPiece(Vector2 mousePosition)
        {
            Vector2 nearestCellPosition = _cellsPositions[0][0];

            Cell nearestCell = _cells[0][0];

            float nearestCellToMousePosition = (nearestCellPosition - mousePosition).magnitude;
            
            for (int i = 0; i < _columns; i++)
            {
                for (int j = 0; j < _rows; j++)
                {
                    float nextMagnitude = (_cellsPositions[i][j] - mousePosition).magnitude;
                    
                    if (nextMagnitude < nearestCellToMousePosition)
                    {
                        nearestCell = _cells[i][j];
                        nearestCellToMousePosition = nextMagnitude;
                    }
                }
            }

            return nearestCell;
        }

        private Cell[] LookForCellsForNeighborPieces(int index, NewsHeadlinePiece draggedPiece, Cell[] cells, NewsHeadlinePiece[] newsHeadlinePieces)
        {
            for (int i = 0; i < newsHeadlinePieces.Length; i++)
            {
                if (i == index)
                {
                    continue;
                }

                cells[i] = LookForDesiredCell(i, index, draggedPiece, cells, newsHeadlinePieces);
            }

            return cells;
        }

        private Cell LookForDesiredCell(int i, int index, NewsHeadlinePiece draggedPiece, Cell[] cells, NewsHeadlinePiece[] newsHeadlinePieces)
        {

            int xCoordinateRelativeToDraggedPiece = 
                (int)(newsHeadlinePieces[i].GetCoordinates().x - draggedPiece.GetCoordinates().x);
                
            int yCoordinateRelativeToDraggedPiece =
                (int)(newsHeadlinePieces[i].GetCoordinates().y - draggedPiece.GetCoordinates().y);

            int finalCellCoordinateX = (int)(cells[index].GetColumn() + xCoordinateRelativeToDraggedPiece);

            int finalCellCoordinateY = (int)(cells[index].GetRow() + yCoordinateRelativeToDraggedPiece);
            
            return _cells[finalCellCoordinateX][finalCellCoordinateY];
        }

        private Vector3 SnapNewsHeadline(Cell[] cells, Vector2 newsHeadlinePosition)
        {
            Vector2 baryCenter = new Vector2();

            foreach (Cell cell in cells)
            {
                cell.SetFree(false);
                Vector2 position = cell.transform.position;
                baryCenter += position;
            }

            baryCenter /= cells.Length;

            return baryCenter;
        }
    }
}
