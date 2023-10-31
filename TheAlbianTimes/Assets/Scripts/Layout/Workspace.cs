using System.Runtime.CompilerServices;
using UnityEngine;

namespace Layout
{
    public class Workspace : MonoBehaviour
    {
        private const int PADDING = 5;

        [SerializeField] private RectTransform _rectTransform;
        
        [SerializeField] private int _columns;
        [SerializeField] private int _rows;

        [SerializeField] private GameObject _cellPrefab;

        [SerializeField] private NewsHeadline _newsHeadline;

        private Vector2 minAnchor;
        private Vector2 maxAnchor;

        private float cellSize;

        private float minAnchorX = 0;
        private float maxAnchorX = 1;
        private float minAnchorY = 0;
        private float maxAnchorY = 1;

        private Cell[][] _cells;

        private Vector2 ModifySizeDelta()
        {

            Vector2 sizeDelta = _rectTransform.sizeDelta;

            cellSize = _rectTransform.rect.height / _rows;

            sizeDelta.x = cellSize * _columns;

            _rectTransform.sizeDelta = new Vector2(sizeDelta.x, sizeDelta.y);

            return sizeDelta;
        }

        void Start()
        {
            _cells = new Cell[_columns][];

            for (int i = 0; i < _columns; i++)
            {
                _cells[i] = new Cell[_rows];
            }

            Vector2 sizeDelta = ModifySizeDelta();

            DefineMinMaxAnchors(sizeDelta);

            for (int i = 0; i < _columns; i++)
            {
                for (int j = 0; j < _rows; j++)
                {
                    CreateCell(i, j, cellSize, sizeDelta);
                }
            }
        }

        private void DefineMinMaxAnchors(Vector2 sizeDelta)
        {

            minAnchor.x = Map(cellSize - cellSize / 2, 0, sizeDelta.x, minAnchorX, maxAnchorX);
            maxAnchor.x = 1 + minAnchor.x;

            minAnchor.y = Map(cellSize - cellSize / 2, 0, sizeDelta.y, minAnchorY, maxAnchorY);
            maxAnchor.y = 1 + minAnchor.y;
        }

        private void CreateCell(int i, int j, float cellSize, Vector2 sizeDelta)
        {

            GameObject cellGameObject = Instantiate(_cellPrefab, gameObject.transform, true);
            Cell cellPrefab = cellGameObject.GetComponent<Cell>();
            float cellPositionX = Map(cellSize * i, 0, sizeDelta.x, minAnchor.x, maxAnchor.x);
            float cellPositionY = Map(cellSize * j, 0, sizeDelta.y, minAnchor.y, maxAnchor.y);
            cellPrefab.SetPosition(new Vector2(cellPositionX, cellPositionY), cellSize - PADDING);
            cellPrefab.SetColumn(i);
            cellPrefab.SetRow((_rows - 1) - j);
            _cells[i][j] = cellPrefab;
        }

        private float Map(float value, float originalMin, float originalMax, float newMin, float newMax)
        {
            return newMin + (value - originalMin) * (newMax - newMin) / (originalMax - originalMin);
        }
    }
}
