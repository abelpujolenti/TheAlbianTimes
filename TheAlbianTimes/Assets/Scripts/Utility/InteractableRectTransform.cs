using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utility
{
    public class InteractableRectTransform : MonoBehaviour
    {
        #region config
        public bool moveCamera;
        public bool draggable;
        public bool hoverable;
        public bool clickable;
        #endregion
        #region variables

        protected const float MIN_X_POSITION_CAMERA = 0;
        protected const float MAX_X_POSITION_CAMERA = 17.8f;
        public bool held { get; protected set; }
        protected Vector2 direction;
        protected RectTransform rectTransform;
        protected Canvas canvas;
        protected RectTransform canvasRect;
        protected EventTrigger eventTrigger;
        protected GameObject gameObjectToDrag;
        private Vector2 _vectorOffset;
        private float _initialMouseXPosition;
        #endregion
        protected void Awake()
        {
            gameObjectToDrag = gameObject;
            Setup();
        }
        protected virtual void Setup()
        {
            rectTransform = (RectTransform)transform;
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null) canvas = transform.parent.GetComponentInParent<Canvas>();
            canvasRect = canvas.GetComponent<RectTransform>();

            eventTrigger = GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = gameObject.AddComponent<EventTrigger>();
            }
            if (draggable)
            {
                AddEventTrigger(EventTriggerType.Drag, Drag);
                AddEventTrigger(EventTriggerType.BeginDrag, BeginDrag);
                AddEventTrigger(EventTriggerType.EndDrag, EndDrag);
            }
            if (moveCamera)
            {
                AddEventTrigger(EventTriggerType.Drag, DragCamera);
                AddEventTrigger(EventTriggerType.BeginDrag, BeginDragCamera);
                AddEventTrigger(EventTriggerType.EndDrag, EndDragCamera);
            }
            if (hoverable)
            {
                AddEventTrigger(EventTriggerType.PointerEnter, PointerEnter);
                AddEventTrigger(EventTriggerType.PointerExit, PointerExit);
            }
            if (clickable)
            {
                AddEventTrigger(EventTriggerType.PointerClick, PointerClick);   
            }
        }
        private void AddEventTrigger(EventTriggerType triggerType, Action<BaseEventData> func)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = triggerType;
            entry.callback.AddListener((eventData) => { func(eventData); });
            eventTrigger.triggers.Add(entry);
        }

        protected virtual void BeginDrag(BaseEventData data)
        {
            PointerEventData pointerData = (PointerEventData) data;

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(pointerData.position);

            _vectorOffset = (Vector2)gameObjectToDrag.transform.position - mousePosition;
        
            held = true;
        }

        protected virtual void Drag(BaseEventData data)
        {
            if (!held && !draggable) return;
        
            Vector2 mousePosition = GetMousePositionOnCanvas(data);

            gameObjectToDrag.transform.position = (Vector2)canvas.transform.TransformPoint(mousePosition) + _vectorOffset;
        }
        
        protected virtual void EndDrag(BaseEventData data)
        {
            held = false;
        }
        
        protected virtual void BeginDragCamera(BaseEventData data)
        {
            PointerEventData pointerData = (PointerEventData) data;            

            _initialMouseXPosition = Camera.main.ScreenToWorldPoint(pointerData.position).x;
        
            held = true;
        }

        protected virtual void DragCamera(BaseEventData data)
        {
            if (!held && !moveCamera) return;
        
            PointerEventData pointerData = (PointerEventData) data;
            
            float mouseXPosition = Camera.main.ScreenToWorldPoint(new Vector3(pointerData.position.x, 0, 0)).x;

            float difference = mouseXPosition - Camera.main.transform.position.x;

            float nextXPosition = _initialMouseXPosition - difference;

            if (nextXPosition < MIN_X_POSITION_CAMERA || nextXPosition > MAX_X_POSITION_CAMERA)
            {
                return;
            }

            Camera.main.transform.position = new Vector3(_initialMouseXPosition - difference, 0, -10);
        }

        protected virtual void EndDragCamera(BaseEventData data)
        {
            held = false;
        }

        protected virtual void PointerEnter(BaseEventData data)
        {
        }
        protected virtual void PointerExit(BaseEventData data)
        {
        }

        protected virtual void PointerClick(BaseEventData data)
        {
        }

        private Vector2 GetMousePositionOnCanvas(BaseEventData data)
        {
            Vector2 mousePosition;
        
            PointerEventData pointerData = (PointerEventData) data;
            Vector2 canvasTopRight = new Vector2(canvasRect.rect.width / 2, canvasRect.rect.height / 2);
            Vector2 canvasBottomLeft = new Vector2(-canvasRect.rect.width / 2, -canvasRect.rect.height / 2);
        
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                pointerData.position,
                Camera.main,
                out mousePosition
            );
            mousePosition.x = Math.Max(Math.Min(mousePosition.x, canvasTopRight.x), canvasBottomLeft.x);
            mousePosition.y = Math.Max(Math.Min(mousePosition.y, canvasTopRight.y), canvasBottomLeft.y);

            return mousePosition;
        }
    }
}
