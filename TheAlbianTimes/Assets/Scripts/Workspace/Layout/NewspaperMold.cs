using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;
using Workspace.Editorial;

namespace Workspace.Layout
{
    public class NewspaperMold : InteractableRectTransform
    {
        private const float TIME_TO_SLIDE = 2f;
        private const float SPEED_MOVEMENT = 15f;
        
        private const int PADDING = 5;
        private const int MIN_ANCHOR_X = 0;
        private const int MAX_ANCHOR_X = 1;
        private const int MIN_ANCHOR_Y = 0;
        private const int MAX_ANCHOR_Y = 1;

        [SerializeField] private RectTransform _rectTransform;
        
        [SerializeField] private int _columns;
        [SerializeField] private int _rows;

        [SerializeField] private GameObject _cellPrefab;

        [SerializeField] private MoldPlace _moldPlace;

        [SerializeField] private Publisher _publisher;

        [SerializeField] private NewsFolder _newsFolder;

        [SerializeField] private MoldLocker _moldLocker;

        [SerializeField]private List<NewsHeadline> _newsHeadlines;

        private Cell[][] _cells;
        
        private Vector2[][] _cellsPositions;

        private readonly Vector3[] _layoutCorners = new Vector3[4];

        private Vector2 _minAnchorForCell;
        private Vector2 _maxAnchorForCell;
        private Vector2 _layoutMinCoordinates;
        private Vector2 _layoutMaxCoordinates;
        private Vector2 _initialPosition;

        private Coroutine _moveCoroutine;

        private float _cellSize;

        private void OnEnable()
        {
            EventsManager.OnPreparingCells += TakeCells;
            EventsManager.OnSuccessfulSnap += SnapNewsHeadline;
            EventsManager.OnGrabSnappedPiece += RemoveNewsHeadline;
        }

        private void OnDisable()
        {
            EventsManager.OnPreparingCells -= TakeCells;
            EventsManager.OnSuccessfulSnap -= SnapNewsHeadline;
            EventsManager.OnGrabSnappedPiece -= RemoveNewsHeadline;
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

            _initialPosition = _rectTransform.localPosition;
            draggable = false;
        }

        protected override void BeginDrag(BaseEventData data)
        {
            if (!draggable)
            {
                return;
            }
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
            }
            base.BeginDrag(data);
        }

        protected override void EndDrag(BaseEventData data)
        {
            if (!draggable)
            {
                return;
            }
            PointerEventData pointerData = (PointerEventData)data;

            if (_moldPlace.IsCoordinateInsideBounds(pointerData.position))
            {
                LayoutManager.Instance.SetIsMoldInPlace(true);
                _moveCoroutine = StartCoroutine(Slide(transform.localPosition));
            }
            else
            {
                LayoutManager.Instance.SetIsMoldInPlace(false);    
            }
            
            _publisher.IsCoordinateInsideBounds(pointerData.position);
            
            base.EndDrag(data);
        }

        private IEnumerator Slide(Vector2 origin)
        {
            float timer = 0;
            
            while (timer < TIME_TO_SLIDE)
            {
                timer = MoveToDestination(origin, _initialPosition, timer);
                yield return null;
            }
        }

        private float MoveToDestination(Vector2 origin, Vector2 destination, float timer)
        {
            timer += Time.deltaTime * SPEED_MOVEMENT;
            transform.localPosition = Vector3.Lerp(origin, destination, timer / TIME_TO_SLIDE);
            
            return timer;
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
            _minAnchorForCell.x = MathUtil.Map(_cellSize - _cellSize / 2, 0, sizeDelta.x, MIN_ANCHOR_X, MAX_ANCHOR_X);
            _maxAnchorForCell.x = _minAnchorForCell.x + 1;

            _minAnchorForCell.y = MathUtil.Map(_cellSize - _cellSize / 2, 0, sizeDelta.y, MIN_ANCHOR_Y, MAX_ANCHOR_Y);
            _maxAnchorForCell.y = _minAnchorForCell.y + 1;
        }

        private void CreateCell(int i, int j, float cellSize, Vector2 sizeDelta)
        {
            GameObject cellGameObject = Instantiate(_cellPrefab, gameObject.transform, false);
            Cell cellPrefab = cellGameObject.GetComponent<Cell>();
            float cellPositionX = MathUtil.Map(cellSize * i, 0, sizeDelta.x, _minAnchorForCell.x, _maxAnchorForCell.x);
            float cellPositionY = MathUtil.Map(cellSize * j, 0, sizeDelta.y, _minAnchorForCell.y, _maxAnchorForCell.y);
            cellPrefab.SetPosition(new Vector2(cellPositionX, cellPositionY), cellSize - PADDING);
            cellPrefab.SetCoordinates(i, j);
            _cells[i][j] = cellPrefab;
            _cellsPositions[i][j] = cellPrefab.transform.position;
        }

        private bool IsCoordinateInsideLayout(Vector2 worldCoordinate)
        {
            return worldCoordinate.x > _layoutMinCoordinates.x && worldCoordinate.x < _layoutMaxCoordinates.x &&
                   worldCoordinate.y < _layoutMinCoordinates.y && worldCoordinate.y > _layoutMaxCoordinates.y;
        }

        private Cell[] TakeCells(NewsHeadlineSubPiece draggedSubPiece, Vector2 mousePosition, NewsHeadlineSubPiece[] newsHeadlinePieces)
        {
            if (!IsCoordinateInsideLayout(mousePosition) || draggable)
            {
                return null;
            }
            
            Cell[] desiredCells = LookForCells(draggedSubPiece, mousePosition, newsHeadlinePieces);

            if (desiredCells == null)
            {
                return null;
            }

            foreach (Cell cell in desiredCells)
            {
                if (cell.IsFree())
                {
                    continue;
                }
                return null;
            }

            return desiredCells;
        }

        private NewsHeadlineSubPiece FindRealDraggedSubPiece(NewsHeadlineSubPiece[] newsHeadlinePieces)
        {
            NewsHeadlineSubPiece draggedSubPiece = newsHeadlinePieces[0];
            for (int i = 0; i < newsHeadlinePieces.Length; i++)
            {
                var p = newsHeadlinePieces[i];
                Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, p.transform.position);
                Vector3 closestScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, draggedSubPiece.transform.position);
                if (Vector2.Distance(screenPos, Input.mousePosition) < Vector2.Distance(closestScreenPos, Input.mousePosition))
                {
                    draggedSubPiece = p;
                }
            }
            return draggedSubPiece;
        }

        private Cell[] LookForCells(NewsHeadlineSubPiece draggedSubPiece, Vector2 mousePosition, NewsHeadlineSubPiece[] newsHeadlinePieces)
        {
            //this sucks but the received draggedsubpiece is consistently wrong
            draggedSubPiece = FindRealDraggedSubPiece(newsHeadlinePieces);

            Cell[] desiredCells = new Cell[newsHeadlinePieces.Length];

            int index = LookingForPieceInsideArray(draggedSubPiece, newsHeadlinePieces);
            
            desiredCells[index] = LookForCellForDraggedPiece(mousePosition);

            foreach (NewsHeadlineSubPiece piece in newsHeadlinePieces)
            {
                if (IsCoordinateInsideLayout(
                    (Vector2)piece.transform.position -
                    (Vector2)draggedSubPiece.transform.position +
                    (Vector2)(desiredCells[index].transform.position)))
                {
                    continue;
                }

                return null;
            }
            return LookForCellsForNeighborPieces(index, draggedSubPiece, desiredCells, newsHeadlinePieces);
        }

        private int LookingForPieceInsideArray(NewsHeadlineSubPiece draggedSubPiece, NewsHeadlineSubPiece[] allPieces)
        {
            int index = 0;

            for (; index < allPieces.Length; index++)
            {
                if (draggedSubPiece.transform.position == allPieces[index].transform.position)
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

        private Cell[] LookForCellsForNeighborPieces(int index, NewsHeadlineSubPiece draggedSubPiece, Cell[] desiredCells, NewsHeadlineSubPiece[] newsHeadlinePieces)
        {
            for (int i = 0; i < newsHeadlinePieces.Length; i++)
            {
                if (i == index)
                {
                    continue;
                }

                desiredCells[i] = LookForDesiredCell(i, index, draggedSubPiece, desiredCells, newsHeadlinePieces);
            }

            return desiredCells;
        }

        private Cell LookForDesiredCell(int i, int index, NewsHeadlineSubPiece draggedSubPiece, Cell[] desiredCells, NewsHeadlineSubPiece[] newsHeadlinePieces)
        {

            int xCoordinateRelativeToDraggedPiece = 
                (int)(newsHeadlinePieces[i].GetCoordinates().x - draggedSubPiece.GetCoordinates().x);
                
            int yCoordinateRelativeToDraggedPiece =
                (int)(newsHeadlinePieces[i].GetCoordinates().y - draggedSubPiece.GetCoordinates().y);

            int finalCellCoordinateX = (int)(desiredCells[index].GetColumn() + xCoordinateRelativeToDraggedPiece);

            int finalCellCoordinateY = (int)(desiredCells[index].GetRow() + yCoordinateRelativeToDraggedPiece);

            return _cells[finalCellCoordinateX][finalCellCoordinateY];
        }

        private Vector3 SnapNewsHeadline(Cell[] snappedCells, NewsHeadline newsHeadline, GameObject piece)
        {
            piece.transform.SetParent(gameObject.transform);
            
            _newsHeadlines.Add(newsHeadline);
            
            foreach (Cell cell in snappedCells)
            {
                cell.SetFree(false);
            }

            if (_newsHeadlines.Count == _newsFolder.GetNewsInLayoutAmount() && _newsFolder.GetNewsHeadlinesLength() == 0)
            {
                _moldLocker.blink = true;
            }

            return snappedCells[0].transform.position;
        }

        private void RemoveNewsHeadline(NewsHeadline newsHeadline)
        {
            _newsHeadlines.Remove(newsHeadline);

            _moldLocker.blink = false;
        }

        public NewsHeadline[] GetNewsHeadlines()
        {
            return _newsHeadlines.ToArray();
        }

        public void SetDraggable(bool draggable) 
        {
            base.draggable = draggable;
            LayoutManager.Instance.SetIsMoldDraggable(draggable);
        }

        public bool IsDraggable() 
        {
            return draggable;
        }
    }
}
