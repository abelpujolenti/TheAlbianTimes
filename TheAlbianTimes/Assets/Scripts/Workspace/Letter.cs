using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Workspace
{
    public class Letter : InteractableRectTransform
    {
        private const float TIME_TO_SLIDE = 0.4f;
        private const float LETTER_MAX_Y_COORDINATE = 0;
        private const float LETTER_MIN_Y_COORDINATE = -500f;
        private const float COORDINATE_Y_THRESHOLD_TO_HIDE = -240f;

        [SerializeField] private LetterDisplay _letterDisplay;
        
        [SerializeField] private TextMeshProUGUI _letterText;

        private float _mouseYPositionOnBeginDrag;
        private float _letterYPositionOnBeginDrag;

        private bool _dragging;

        private void OnEnable()
        {
            _dragging = false;
            Transform letterTransform = transform;
            Vector2 letterPosition = letterTransform.position;
            
            Vector2 origin = new Vector2(letterPosition.x, LETTER_MIN_Y_COORDINATE);
            Vector2 destination = new Vector2(letterPosition.x, LETTER_MAX_Y_COORDINATE);
            
            letterTransform.position = origin;
            StartCoroutine(Slide(origin, destination));
        }

        protected override void BeginDrag(BaseEventData data)
        {
            _dragging = true;
            PointerEventData pointerData = (PointerEventData)data;
            _mouseYPositionOnBeginDrag = Camera.main.ScreenToWorldPoint(pointerData.position).y;
            _letterYPositionOnBeginDrag = transform.position.y;
        }

        protected override void Drag(BaseEventData data)
        {
            Transform letterTransform = transform;
            PointerEventData pointerData = (PointerEventData)data;
            float currentMouseYPosition = Camera.main.ScreenToWorldPoint(pointerData.position).y;
            float mouseYDistanceFromInitialPositionToCurrentPosition = currentMouseYPosition - _mouseYPositionOnBeginDrag;

            float newYCoordinate = _letterYPositionOnBeginDrag + mouseYDistanceFromInitialPositionToCurrentPosition;

            if (newYCoordinate > LETTER_MAX_Y_COORDINATE)
            {
                return;
            }

            letterTransform.position = new Vector2(letterTransform.position.x, newYCoordinate);
        }

        protected override void EndDrag(BaseEventData data)
        {
            _dragging = false;
            Vector3 letterPosition = transform.localPosition;
            
            if (letterPosition.y > COORDINATE_Y_THRESHOLD_TO_HIDE)
            {
                return;
            }
            Vector2 destination = new Vector2(letterPosition.x, LETTER_MIN_Y_COORDINATE);
            
            StartCoroutine(Slide(letterPosition, destination));
        }

        public void SetText(String text)
        {
            _letterText.text = text;
        }

        private IEnumerator Slide(Vector2 origin, Vector2 destination)
        {
            float timer = 0;

            while (timer < TIME_TO_SLIDE && !_dragging)
            {
                timer = MoveToDestination(origin, destination, timer);
                yield return null;
            }

            if (destination.y < COORDINATE_Y_THRESHOLD_TO_HIDE)
            {
                _letterDisplay.HideLetter();
            }
        }

        private float MoveToDestination(Vector2 origin, Vector2 destination, float timer)
        {
            timer += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(origin, destination, timer / TIME_TO_SLIDE);
            
            return timer;
        }
    }
}
