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
        private GameObject _gameObjectToTransferDrag;

        [SerializeField]private NewsHeadline _newsHeadlineToTransferDrag;
        
        [SerializeField] private NewsHeadlinePiece _newsHeadlinePiece;
        
        [SerializeField] private Vector2 _coordinates;

        private Image _image;

        private new void Awake()
        {
            base.Awake();
            _image = GetComponent<Image>();
            gameObjectToDrag = transform.parent.gameObject;
            
        }

        public void SimulateBeginDrag(BaseEventData data)
        {
            BeginDrag(data);
        }

        protected override void BeginDrag(BaseEventData data)
        {
            base.BeginDrag(data);
            
            _newsHeadlinePiece.BeginDrag();
        }

        protected override void Drag(BaseEventData data)
        {
            PointerEventData pointerData = (PointerEventData)data;

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(pointerData.position);

            if (mousePosition.x < 17.8 / 2)
            {
                Transform objectToDragTransform = _gameObjectToTransferDrag.transform;
                objectToDragTransform.position = mousePosition;
                objectToDragTransform.gameObject.SetActive(true);
                _newsHeadlineToTransferDrag.SimulateBeginDrag(data);
            
                pointerData.pointerDrag = _gameObjectToTransferDrag;
            
                transform.parent.gameObject.SetActive(false);
                
                SimulateEndDrag(data);

                if (!_newsHeadlineToTransferDrag.GetTransferDrag())
                {
                    _newsHeadlinePiece.SetTransferDrag(true);
                }
                else
                {
                    _newsHeadlineToTransferDrag.SetTransferDrag(false);
                }
                
                return;
            }
            
            base.Drag(data);
        }

        public void SimulateEndDrag(BaseEventData data)
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

        public void SetGameObjectToTransferDrag(GameObject gameObjectToTransferDrag)
        {
            _gameObjectToTransferDrag = gameObjectToTransferDrag;
        }

        public void SetNewsHeadlineToTransferDrag(NewsHeadline newsHeadlineToTransferDrag)
        {
            _newsHeadlineToTransferDrag = newsHeadlineToTransferDrag;
        }
    }
}
