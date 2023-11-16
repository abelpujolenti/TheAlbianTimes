using System;
using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace CameraController
{
    public class CameraSideScroll : InteractableRectTransform
    {
        private const float SCROLL_SPEED = 5;

        [SerializeField] private Camera _camera;

        [SerializeField] private CameraScrollContainer _container;
        
        [SerializeField] private bool _scrollRight;

        private readonly float _midPoint = MIN_X_POSITION_CAMERA + (MAX_X_POSITION_CAMERA - MIN_X_POSITION_CAMERA) / 2;

        private Coroutine _coroutine;

        private bool _rightSide;
        private bool _transfer;
        private bool _scrolling;
        private bool _exceed;

        private void OnEnable()
        {
            EventsManager.OnAssignGameObjectToDrag += SetGameObjectToDrag;
        }

        private void OnDisable()
        {
            EventsManager.OnAssignGameObjectToDrag -= SetGameObjectToDrag;
        }

        protected override void PointerEnter(BaseEventData data)
        {
            _scrolling = true;

            PointerEventData pointerData = (PointerEventData)data;
            
            EventsManager.OnAssignGameObjectToDrag(
                EventsManager.OnCrossMidPointWhileScrolling(pointerData));
            
            Vector2 mousePosition = _camera.ScreenToWorldPoint(pointerData.position);

            _rightSide = mousePosition.x > _midPoint;

            Vector2 offset = EventsManager.OnCheckDistanceToMouse(mousePosition);
            
            _coroutine = StartCoroutine(Scroll(pointerData, offset));
        }
        
        protected override void PointerExit(BaseEventData data)
        {
            _transfer = false;
            _scrolling = false;
        }

        private IEnumerator Scroll(PointerEventData pointerData, Vector2 offset)
        {
            while (_scrolling)
            {
                DragGameObject(pointerData, offset);
                
                Vector3 nextPosition = _camera.transform.position;
            
                if (_scrollRight)
                {
                    nextPosition.x += Time.deltaTime * SCROLL_SPEED;

                    if (EventsManager.OnExceedCameraLimitsWhileDragging != null)
                    {
                        EventsManager.OnExceedCameraLimitsWhileDragging(true);
                    }

                    if (nextPosition.x > MAX_X_POSITION_CAMERA)
                    {
                        _container.SubscribeOnExceedEvent();
                        _transfer = false;
                        _scrolling = false;
                        gameObject.SetActive(false);
                        StopCoroutine(_coroutine);
                    }
                }
                else
                {
                    nextPosition.x -= Time.deltaTime * SCROLL_SPEED;

                    if (EventsManager.OnExceedCameraLimitsWhileDragging != null) 
                    {
                        EventsManager.OnExceedCameraLimitsWhileDragging(true);
                    }

                    if (nextPosition.x < MIN_X_POSITION_CAMERA)
                    {
                        _container.SubscribeOnExceedEvent();
                        _transfer = false;
                        _scrolling = false;
                        gameObject.SetActive(false);
                        StopCoroutine(_coroutine);
                    }
                }
                
                _camera.transform.position = nextPosition;
                yield return null;
            }
        }

        private void DragGameObject(PointerEventData pointerData, Vector2 offset)
        {
            Vector2 mousePosition = _camera.ScreenToWorldPoint(pointerData.position);

            if (_rightSide)
            {
                if (mousePosition.x < _midPoint)
                {
                    _transfer = !_transfer;
                    _rightSide = false;
                    EventsManager.OnAssignGameObjectToDrag(
                        EventsManager.OnCrossMidPointWhileScrolling(pointerData));
                }
            }
            else
            {
                if (mousePosition.x > _midPoint)
                { _transfer = !_transfer;
                    _rightSide = true;
                    EventsManager.OnAssignGameObjectToDrag(
                        EventsManager.OnCrossMidPointWhileScrolling(pointerData));
                }
            }

            if (EventsManager.OnCrossMidPointWhileScrolling != null)
            {
            }            

            if (!_transfer)
            {
                mousePosition += offset;
            }

            gameObjectToDrag.transform.position = mousePosition;
        }

        private void SetGameObjectToDrag(GameObject gameObjectToDrag)
        {
            this.gameObjectToDrag = gameObjectToDrag;
        }
    }
}