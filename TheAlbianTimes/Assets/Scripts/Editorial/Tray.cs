using System;
using System.Collections;
using Managers;
using UnityEngine;

namespace Editorial
{
    public class Tray : MonoBehaviour
    {
        private const float TIME_TO_SLIDE = 2f;
        private const float SPEED = 15f;
        private const float DISTANCE_TO_SLIDE_ON_COORDINATE_Y = -2.2f;

        [SerializeField] private RectTransform _rectTransform;

        private Vector3[] _corners = new Vector3[4];
        private Vector2 _containerMinCoordinates;
        private Vector2 _containerMaxCoordinates;
        
        private Vector3 _hidedPosition;
        private Vector3 _extendedPosition;

        private bool _extended;
        
        private void OnEnable()
        {
            EventsManager.OnDropNewsHeadline += Interaction;
        }

        private void OnDisable()
        {
            EventsManager.OnDropNewsHeadline -= Interaction;
        }

        void Start()
        {
            _hidedPosition = transform.position;
            _extendedPosition = new Vector2(_hidedPosition.x, _hidedPosition.y + DISTANCE_TO_SLIDE_ON_COORDINATE_Y);
        }

        private void Interaction(NewsHeadline newsHeadline, Vector3 mousePosition)
        {
            Vector3 screenWorldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            
            if (!IsCoordinateInsideBounds(screenWorldMousePosition))
            {
                newsHeadline.DropNewsHeadline(mousePosition);
                return;
            }   
            
            newsHeadline.SetOrigin(newsHeadline.transform.localPosition);
            Hide(newsHeadline.gameObject);
            
            EventsManager.OnChangeNewsHeadlineContent();
        }

        public void Hide(GameObject newsHeadline)
        {
            if (!_extended)
            {
                return;
            }
            _extended = false;
            if (newsHeadline == null)
            {
                StartCoroutine(Slide(_extendedPosition, _hidedPosition));
                return;
            }

            StartCoroutine(UntilHeadlineReachMidPoint(newsHeadline));
        }

        private IEnumerator UntilHeadlineReachMidPoint(GameObject newsHeadline)
        {
            while (newsHeadline.transform.position.y < transform.position.y)
            {
                yield return null;
            }
            StartCoroutine(Slide(_extendedPosition, _hidedPosition));
        }

        public void Extend()
        {
            if (_extended)
            {
                return;
            }
            _extended = true;
            StartCoroutine(Slide(_hidedPosition, _extendedPosition));
        }

        private IEnumerator Slide(Vector3 origin, Vector3 destination)
        {
            float timer = 0;

            while (timer < TIME_TO_SLIDE)
            {
                _rectTransform.GetWorldCorners(_corners);
            
                SetContainerLimiters();
                timer = MoveToDestination(origin, destination, timer);
                yield return null;
            }
        }

        private float MoveToDestination(Vector2 origin, Vector2 destination, float timer)
        {
            timer += Time.deltaTime * SPEED;
            transform.position = Vector3.Lerp(origin, destination, timer / TIME_TO_SLIDE);
            
            return timer;
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
