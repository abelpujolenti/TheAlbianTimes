using System;
using System.Collections;
using UnityEngine;

namespace Editorial
{
    public class Tray : MonoBehaviour
    {
        private const float TIME_TO_SLIDE = 2f;
        private const float SPEED = 15f;
        private const float DISTANCE_TO_SLIDE_ON_COORDINATE_Y = -2.2f;
        
        private Vector3 _hidedPosition;
        private Vector3 _extendedPosition;

        private bool _extended;

        void Start()
        {
            _hidedPosition = transform.position;
            _extendedPosition = new Vector2(_hidedPosition.x, _hidedPosition.y + DISTANCE_TO_SLIDE_ON_COORDINATE_Y);
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
    }
}
