using UnityEngine;

namespace Layout
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;

        private Vector2 _coordinates;

        private bool _isFree;
        
        void Start()
        {
            _rectTransform.localScale = new Vector3(1, 1, 1);
            _isFree = true;
        }

        public void SetPosition(Vector2 position, float cellSize)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();

            rectTransform.anchorMin = position;
            rectTransform.anchorMax = position;

            rectTransform.anchoredPosition = new Vector2(0, 0);

            rectTransform.sizeDelta = new Vector2(cellSize, cellSize);
            
            //Debug.Log(gameObject.GetComponent<Image>().sprite.bounds.size.x);

            _rectTransform = rectTransform;
        }

        public void SetColumn(int column)
        {
            _coordinates.x = column;
        }
        
        public float GetColumn()
        {
            return _coordinates.x;
        }

        public void SetRow(int row)
        {
            _coordinates.y = row;
        }

        public float GetRow()
        {
            return _coordinates.y;
        }

        public void SetFree(bool isFree)
        {
            _isFree = isFree;
        }

        public bool IsFree()
        {
            return _isFree;
        }

        public void SetCoordinates(int column, int row)
        {
            _coordinates = new Vector2(column, row);
        }

        public Vector2 GetCoordinates()
        {
            return _coordinates;
        }
    }
}
