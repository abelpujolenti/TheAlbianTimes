using Editorial;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

namespace Layout
{
    public class NewsHeadlineSubPiece : InteractableRectTransform
    {
        private const float SIZE = 45.12856f;
        
        private readonly float _midPoint = MIN_X_POSITION_CAMERA + (MAX_X_POSITION_CAMERA - MIN_X_POSITION_CAMERA) / 2;
        
        private GameObject _gameObjectToTransferDrag;

        private NewsHeadline _newsHeadlineToTransferDrag;
        
        private NewsHeadlinePiece _newsHeadlinePiece;
        
        private Vector2 _coordinates;
        
        private Image _image;

        private new void Awake()
        {
            draggable = true;
            base.Awake();
            _image = GetComponent<Image>();
            _image.rectTransform.sizeDelta = new Vector2(SIZE, SIZE);
            gameObjectToDrag = transform.parent.gameObject;
            
        }

        public void SimulateBeginDrag(BaseEventData data)
        {
            BeginDrag(data);
        }

        protected override void BeginDrag(BaseEventData data)
        {
            base.BeginDrag(data);

            EventsManager.OnStartEndDrag(true);
            
            EventsManager.OnCrossMidPointWhileScrolling += GetGameObjectToTransferDrag;
            
            _newsHeadlinePiece.BeginDrag();
        }

        protected override void Drag(BaseEventData data)
        {
            base.Drag(data);

            PointerEventData pointerData = (PointerEventData)data;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(pointerData.position);

            if (mousePosition.x >= _midPoint)
            {
                return;
            }

            CrossMidPoint(pointerData, mousePosition);  
        }
        private void CrossMidPoint(PointerEventData pointerData, Vector2 mousePosition)
        {
            
            _newsHeadlinePiece.SetTransferDrag(true);
            _newsHeadlineToTransferDrag.SetTransferDrag(false);
                
            _newsHeadlinePiece.gameObject.SetActive(false);  
            
            _gameObjectToTransferDrag.transform.position = mousePosition;
            _gameObjectToTransferDrag.gameObject.SetActive(true);
            _newsHeadlineToTransferDrag.SimulateBeginDrag(pointerData);
            
            pointerData.pointerDrag = _gameObjectToTransferDrag;

            EndDrag(pointerData);
        }

        private void SimulateEndDrag(BaseEventData data)
        {
            EndDrag(data);
        }

        protected override void EndDrag(BaseEventData data)
        {
            if (_newsHeadlineToTransferDrag.GetTransferDrag())
            {
                _newsHeadlineToTransferDrag.SimulateEndDrag(data);
                _newsHeadlineToTransferDrag.SetTransferDrag(false);
            }
            
            base.EndDrag(data);
            
            EventsManager.OnCrossMidPointWhileScrolling -= GetGameObjectToTransferDrag;
            
            if (_newsHeadlinePiece.gameObject.activeSelf)
            {
                EventsManager.OnStartEndDrag(false);    
            }
            
            PointerEventData pointerData = (PointerEventData) data;
            _newsHeadlinePiece.EndDrag(this, pointerData.position);
        }

        public void Fade(float alpha) { 

            Color auxColor = _image.color;

            auxColor.a = alpha;

            _image.color = auxColor;            
        }

        public void SetNewsHeadlinePiece(NewsHeadlinePiece newsHeadlinePiece)
        {
            _newsHeadlinePiece = newsHeadlinePiece;
        }

        public void SetCoordinates(Vector2 newCoordinates)
        {
            _coordinates = newCoordinates;
        }

        public Vector2 GetCoordinates()
        {
            return _coordinates;
        }

        public void SetPositionFromCoordinates()
        {
            transform.localPosition = new Vector2(_coordinates.x * SIZE, _coordinates.y * SIZE);
        }

        public void SetGameObjectToTransferDrag(GameObject gameObjectToTransferDrag)
        {
            _gameObjectToTransferDrag = gameObjectToTransferDrag;
        }

        public void SetNewsHeadlineToTransferDrag(NewsHeadline newsHeadlineToTransferDrag)
        {
            _newsHeadlineToTransferDrag = newsHeadlineToTransferDrag;
        }

        public NewsHeadline GetNewsHeadline()
        {
            return _newsHeadlineToTransferDrag;
        }

        private GameObject GetGameObjectToTransferDrag(PointerEventData pointerData)
        {
            Drag(pointerData);
            
            //TODO BUG ON TRANSFER WITH SCROLL
            
            return !_newsHeadlinePiece.GetTransferDrag() ? _newsHeadlinePiece.gameObject : _newsHeadlineToTransferDrag.gameObject;
        }
    }
}
