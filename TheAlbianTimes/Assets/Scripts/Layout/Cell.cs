using UnityEngine;

namespace Layout
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        
        public int _column;
        public int _row;

        private bool _free;
        
        // Start is called before the first frame update
        void Start()
        {
            _rectTransform.localScale = new Vector3(1, 1, 1);
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log(_rectTransform.transform.position);
        }

        public void SetPosition(Vector2 position)
        {
            _rectTransform.transform.position = position;
        }

        public void SetColumn(int column)
        {
            _column = column;
        }
        
        public int GetColumn()
        {
            return _column;
        }

        public void SetRow(int row)
        {
            _row = row;
        }

        public int GetRow()
        {
            return _row;
        }

    }
}
