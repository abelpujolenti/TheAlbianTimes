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
        private const float SCROLL_TIME = 1f;
        
        private readonly float _midPoint = MIN_X_POSITION_CAMERA + (MAX_X_POSITION_CAMERA - MIN_X_POSITION_CAMERA) / 2;

        [SerializeField] private CameraScrollContainer _container;
        
        [SerializeField] private bool _toLayout;

        private CameraManager _cameraManagerInstance;
        
        private Coroutine _coroutine;

        private RectTransform _rectTransform;

        private bool _rightSide;
        private bool _transfer;
        
        private Action <float> _panCamera;

        private void Start()
        {
            _cameraManagerInstance = CameraManager.Instance;
            _rectTransform = GetComponent<RectTransform>();
            if (_toLayout)
            {
                _panCamera = time => _cameraManagerInstance.PanToLayout(time);
                return;
            }

            _panCamera = time => _cameraManagerInstance.PanToEditorial(time);
        }

        protected override void PointerEnter(BaseEventData data)
        {
            Scroll(data);
        }
        
        protected override void PointerExit(BaseEventData data)
        {
            _transfer = false;
        }

        private void Scroll(BaseEventData data)
        {
            if (_cameraManagerInstance.IsScrolling())
            {
                return;
            }
            
            _panCamera(SCROLL_TIME);

            PointerEventData pointerData = (PointerEventData)data;
            
            gameObjectToDrag = EventsManager.OnCrossMidPointWhileScrolling(pointerData);

            Vector2 mousePosition = _camera.ScreenToWorldPoint(pointerData.position);

            _rightSide = mousePosition.x > _midPoint;

            Vector2 offset = EventsManager.OnCheckDistanceToMouse(mousePosition);

            StartCoroutine(DragGameObject(pointerData, offset));
        }

        private IEnumerator DragGameObject(PointerEventData pointerData, Vector2 offset)
        {
            _container.FlipSideScroll(_rectTransform);

            while (_cameraManagerInstance.IsScrolling())
            {
                Vector2 mousePosition = _camera.ScreenToWorldPoint(pointerData.position);

                if (_rightSide)
                {
                    if (mousePosition.x < _midPoint)
                    {
                        _transfer = !_transfer;
                        _rightSide = false;
                    }
                }
                else
                {
                    if (mousePosition.x > _midPoint)
                    {
                        _transfer = !_transfer;
                        _rightSide = true;
                    }
                }
                
                gameObjectToDrag = EventsManager.OnCrossMidPointWhileScrolling != null
                    ? EventsManager.OnCrossMidPointWhileScrolling(pointerData)
                    : null; 

                if (!_transfer)
                {
                    mousePosition += offset;
                }

                if (gameObjectToDrag != null)
                {
                    gameObjectToDrag.transform.position = mousePosition;    
                }

                yield return null;
            }

            if (gameObjectToDrag != null)
            {
                EventsManager.OnStartEndDrag(true);
            }
            _container.FlipSideScroll(_rectTransform);
            gameObject.SetActive(false);
        }
    }
}