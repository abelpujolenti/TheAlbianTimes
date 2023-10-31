using UnityEngine;

namespace Layout
{
    public class Workspace : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private int _columns;
        [SerializeField] private int _rows;

        [SerializeField] private GameObject _cellPrefab;
        void Start()
        {
            Vector2 aux = _rectTransform.sizeDelta;

            float cellHeight = _rectTransform.rect.height / _rows;
            float cellWidth = cellHeight * _columns;

            aux.x = cellWidth;

            Vector2 cellSize = _rectTransform.sizeDelta = new Vector2(aux.x, aux.y);
            
            Debug.Log(cellSize);

            for (int i = 0; i < _columns; i++)
            {
                for (int j = 0; j < _rows; j++)
                {
                    GameObject cellGameObject = Instantiate(_cellPrefab, gameObject.transform, true);
                    Cell cellPrefab = cellGameObject.GetComponent<Cell>();
                    cellPrefab.SetPosition(new Vector2(cellSize.x * j, cellSize.y * i));
                    cellPrefab.SetColumn(i);
                    cellPrefab.SetRow(j);
                }
            }
        }
    }
}
