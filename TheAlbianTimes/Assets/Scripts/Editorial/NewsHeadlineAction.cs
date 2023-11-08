using Managers;
using UnityEngine;

namespace Editorial
{
    public abstract class NewsHeadlineAction : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;

        private readonly Vector3[] corners = new Vector3[4];
        
        private Vector2 containerMinCoordinates;
        private Vector2 containerMaxCoordinates;

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
            _rectTransform.GetWorldCorners(corners);
            
            SetContainerLimiters();
        }

        private void SetContainerLimiters()
        {
            containerMinCoordinates.x = corners[0].x;
            containerMinCoordinates.y = corners[1].y;
            containerMaxCoordinates.x = corners[2].x;
            containerMaxCoordinates.y = corners[3].y;
        }

        protected abstract void Action(NewsHeadline newsHeadline, Vector3 mousePosition);

        protected bool IsCoordinateInsideBounds(Vector2 coordinate)
        {
            return coordinate.x < containerMaxCoordinates.x && coordinate.x > containerMinCoordinates.x &&
                   coordinate.y > containerMaxCoordinates.y && coordinate.y < containerMinCoordinates.y;
        }
    }
}
