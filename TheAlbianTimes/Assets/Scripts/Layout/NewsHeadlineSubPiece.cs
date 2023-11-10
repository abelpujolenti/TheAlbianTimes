using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

namespace Layout
{
    public class NewsHeadlineSubPiece : InteractableRectTransform
    {
        [SerializeField] private NewsHeadlinePiece _newsHeadlinePiece;
        [SerializeField] private Vector2 _coordinates;

        private Image _image;

        private void Start()
        {
            _image = GetComponent<Image>();
            _gameObjectToDrag = transform.parent.gameObject;
        }

        protected override void BeginDrag(BaseEventData data)
        {
            base.BeginDrag(data);
            
            _newsHeadlinePiece.BeginDrag();
        }

        protected override void EndDrag(BaseEventData data)
        {
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
    }
}
