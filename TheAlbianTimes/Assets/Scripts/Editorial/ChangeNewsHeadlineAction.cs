using Managers;
using UnityEngine;

namespace Editorial
{
    public class ChangeNewsHeadlineAction : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;

        private readonly Vector3[] _corners = new Vector3[4];
        
        private Vector2 _containerMinCoordinates;
        private Vector2 _containerMaxCoordinates;
        
        private void OnEnable()
        {
            EventsManager.OnDropNewsHeadline += Action;
        }

        private void OnDisable()
        {
            EventsManager.OnDropNewsHeadline -= Action;
        }

        void Start()
        {
            _rectTransform.GetWorldCorners(_corners);
            
            SetContainerLimiters();
        }
        
        private void Action(NewsHeadline newsHeadline, Vector3 mousePosition)
        {
            gameObject.SetActive(false);
            
            Vector3 screenWorldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            
            if (!IsCoordinateInsideBounds(screenWorldMousePosition))
            {
                newsHeadline.DropNewsHeadline(mousePosition);
                return;
            }   
            
            newsHeadline.SetOrigin(newsHeadline.transform.localPosition);
            
            EventsManager.OnChangeNewsHeadlineContent();
        }

        private void SetContainerLimiters()
        {
            _containerMinCoordinates.x = _corners[0].x;
            _containerMinCoordinates.y = _corners[1].y;
            _containerMaxCoordinates.x = _corners[2].x;
            _containerMaxCoordinates.y = _corners[3].y;
        }

        private bool IsCoordinateInsideBounds(Vector2 coordinate)
        {
            return coordinate.x < _containerMaxCoordinates.x && coordinate.x > _containerMinCoordinates.x &&
                   coordinate.y > _containerMaxCoordinates.y && coordinate.y < _containerMinCoordinates.y;
        }
    }
}
