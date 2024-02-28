using UnityEngine;

namespace Workspace.Layout
{
    public class MoldPlace : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;

        private Vector3[] _corners = new Vector3[4];
    
        private Vector2 _containerMinCoordinates;
        private Vector2 _containerMaxCoordinates;

        private Camera _camera;

        private void Start()
        {
            _rectTransform.GetWorldCorners(_corners);
            _camera = Camera.main;
            SetContainerLimiters();
        }

        private void SetContainerLimiters()
        {
            _containerMinCoordinates.x = _corners[0].x;
            _containerMinCoordinates.y = _corners[1].y;
            _containerMaxCoordinates.x = _corners[2].x;
            _containerMaxCoordinates.y = _corners[3].y;
        }

        public bool IsCoordinateInsideBounds(Vector2 coordinate)
        {
            Vector3 screenWorldMousePosition = _camera.ScreenToWorldPoint(coordinate);
        
            return screenWorldMousePosition.x < _containerMaxCoordinates.x && screenWorldMousePosition.x > _containerMinCoordinates.x &&
                   screenWorldMousePosition.y > _containerMaxCoordinates.y && screenWorldMousePosition.y < _containerMinCoordinates.y;
        }
    }
}
