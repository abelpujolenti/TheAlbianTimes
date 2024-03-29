using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utility
{
    public class InteractableRectTransform : MonoBehaviour
    {
        #region config
        public bool draggable;
        public bool hoverable;
        public bool clickable;
        #endregion

        #region variables
        protected const float MIN_X_POSITION_CAMERA = 0;
        protected const float MAX_X_POSITION_CAMERA = 17.77f;
        public bool held { get; protected set; }
        protected Vector2 direction;
        protected RectTransform rectTransform;
        protected Canvas canvas;
        protected RectTransform canvasRect;
        protected EventTrigger eventTrigger;
        protected GameObject gameObjectToDrag;
        protected Vector3 _vectorOffset;
        protected Camera _camera;
        #endregion

        protected void Awake()
        {
            gameObjectToDrag = gameObject;
            canvas = GetComponentInParent<Canvas>();
            _camera = Camera.main;
            Setup();
        }
        protected virtual void Setup()
        {
            rectTransform = (RectTransform)transform;
            if (canvas == null)
            {
                if (transform.parent)
                {
                    if (transform.parent.GetComponentInParent<Canvas>() == null)
                    {
                        return;
                    }
                    
                    canvas = transform.parent.GetComponentInParent<Canvas>();
                }
                return;
            }
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
            if (hoverable)
            {
                AddEventTrigger(EventTriggerType.PointerEnter, PointerEnter);
                AddEventTrigger(EventTriggerType.PointerExit, PointerExit);
            }
            if (clickable)
            {
                AddEventTrigger(EventTriggerType.PointerClick, PointerClick);   
                AddEventTrigger(EventTriggerType.PointerDown, PointerDown);   
                AddEventTrigger(EventTriggerType.PointerUp, PointerUp);   
            }
        }
        private void AddEventTrigger(EventTriggerType triggerType, Action<BaseEventData> func)
        {
            foreach (var t in eventTrigger.triggers)
            {
                if (t.eventID == triggerType) return;
            }
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = triggerType;
            entry.callback.AddListener((eventData) => { func(eventData); });
            eventTrigger.triggers.Add(entry);
        }

        protected virtual void BeginDrag(BaseEventData data)
        {
            PointerEventData pointerData = (PointerEventData) data;

            Vector2 mousePosition = _camera.ScreenToWorldPoint(pointerData.position);

            _vectorOffset = (Vector2)gameObjectToDrag.transform.position - mousePosition;
        
            held = true;
        }

        protected virtual void Drag(BaseEventData data)
        {
            if (!held && !draggable) return;
        
            Vector3 mousePosition = GetMousePositionOnCanvas(data);

            Vector3 mousePositionInWorld = canvas.transform.TransformPoint(mousePosition) + _vectorOffset;

            gameObjectToDrag.transform.position = new Vector3(mousePositionInWorld.x, mousePositionInWorld.y, transform.position.z);
        }
        
        protected virtual void EndDrag(BaseEventData data)
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

        protected virtual void PointerUp(BaseEventData data)
        {
        }

        protected virtual void PointerDown(BaseEventData data)
        {
        }

        protected Vector3 GetMousePositionOnCanvas(BaseEventData data)
        {
            Vector2 mousePosition;
        
            PointerEventData pointerData = (PointerEventData) data;
            Vector2 canvasTopRight = new Vector2(canvasRect.rect.width / 2, canvasRect.rect.height / 2);
            Vector2 canvasBottomLeft = new Vector2(-canvasRect.rect.width / 2, -canvasRect.rect.height / 2);
        
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                pointerData.position,
                _camera,
                out mousePosition
            );
            mousePosition.x = Math.Max(Math.Min(mousePosition.x, canvasTopRight.x), canvasBottomLeft.x);
            mousePosition.y = Math.Max(Math.Min(mousePosition.y, canvasTopRight.y), canvasBottomLeft.y);

            return new Vector3(mousePosition.x, mousePosition.y, 0);
        }

        public void SetCanvas(Canvas canvas)
        {
            this.canvas = canvas;
            Setup();
        }

        protected IEnumerator SetPositionCoroutine(Vector3 start, Vector3 end, float t)
        {
            yield return TransformUtility.SetPositionCoroutine(gameObjectToDrag.transform, start, end, t);
        }

        protected IEnumerator SetRotationCoroutine(float zRotation, float t)
        {
            yield return TransformUtility.SetRotationCoroutine(gameObjectToDrag.transform, zRotation, t);
        }
    }
}
