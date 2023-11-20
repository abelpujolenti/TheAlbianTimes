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
        private readonly float _midPoint = MIN_X_POSITION_CAMERA + (MAX_X_POSITION_CAMERA - MIN_X_POSITION_CAMERA) / 2;
        
        private GameObject _gameObjectToTransferDrag;

        [SerializeField] private NewsHeadline _newsHeadlineToTransferDrag;
        
        [SerializeField] private NewsHeadlinePiece _newsHeadlinePiece;
        
        [SerializeField] private Vector2 _coordinates;

        private float size = 61.66667f; //lmao

        private Image _image;

        private new void Awake()
        {
            draggable = true;
            base.Awake();
            _image = GetComponent<Image>();
            _image.rectTransform.sizeDelta = new Vector2(size, size);
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
            if (_newsHeadlinePiece.GetTransferDrag()) 
            {
                return;
            }

            PointerEventData pointerData = (PointerEventData)data;

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(pointerData.position);
            
            if (mousePosition.x < _midPoint)
            {
                CrossMidPoint(pointerData, mousePosition);  
                
                return;
            }
            
            base.Drag(data);
        }
        private void CrossMidPoint(PointerEventData pointerData, Vector2 mousePosition)
        {
            if (!_newsHeadlineToTransferDrag.GetTransferDrag())
            {
                _newsHeadlinePiece.SetTransferDrag(true);
            }
            else
            {
                _newsHeadlineToTransferDrag.SetTransferDrag(false);
            }
                
            _gameObjectToTransferDrag.transform.position = mousePosition;
            _gameObjectToTransferDrag.gameObject.SetActive(true);
            _newsHeadlineToTransferDrag.SimulateBeginDrag(pointerData);
            
            pointerData.pointerDrag = _gameObjectToTransferDrag;
            
            transform.parent.gameObject.SetActive(false);  

            SimulateEndDrag(pointerData);
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
            
            EventsManager.OnCrossMidPointWhileScrolling -= GetGameObjectToTransferDrag;
            
            base.EndDrag(data);
            
            if (_newsHeadlinePiece.gameObject.activeSelf)
            {
                EventsManager.OnStartEndDrag(false);    
            }
            
            PointerEventData pointerData = (PointerEventData) data;
            
            if (EventsManager.OnDropNewsHeadlinePiece == null)
            {
                return;
            }

            EventsManager.OnDropNewsHeadlinePiece(this, pointerData.position);
        }

        public void Fade(float alpha) { 

            Color auxColor = _image.color;

            auxColor.a = alpha;

            _image.color = auxColor;            
        }

        public NewsHeadline GetNewsHeadline()
        {
            return _newsHeadlineToTransferDrag;
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

        public void SetPositionFromCoordinates(Vector2 pieceSize)
        {
            transform.localPosition = new Vector2(_coordinates.x * size, _coordinates.y * size);
        }

        public void SetGameObjectToTransferDrag(GameObject gameObjectToTransferDrag)
        {
            _gameObjectToTransferDrag = gameObjectToTransferDrag;
        }

        public void SetNewsHeadlineToTransferDrag(NewsHeadline newsHeadlineToTransferDrag)
        {
            _newsHeadlineToTransferDrag = newsHeadlineToTransferDrag;
        }

        private GameObject GetGameObjectToTransferDrag(PointerEventData pointerData)
        {
            Drag(pointerData);
            
            //TODO BUG ON TRANSFER WITH SCROLL
            
            return !_newsHeadlinePiece.GetTransferDrag() ? _newsHeadlinePiece.gameObject : _newsHeadlineToTransferDrag.gameObject;
        }
    }
}
