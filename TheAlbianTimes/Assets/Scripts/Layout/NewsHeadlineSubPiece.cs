using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

namespace Layout
{
    public class NewsHeadlineSubPiece : InteractableRectTransform
    {
        [SerializeField] private NewHeadlinePiece newHeadlinePiece;
        
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
            
            newHeadlinePiece.BeginDrag();
        }

        protected override void EndDrag(BaseEventData data)
        {
            base.EndDrag(data);

            PointerEventData pointerData = (PointerEventData) data;
            
            if (EventsManager.OnReleaseNewsHeadline == null)
            {
                return;
            }

            EventsManager.OnReleaseNewsHeadline(this, pointerData.position);
        }

        public void Fade(float alpha) { 

            Color auxColor = _image.color;

            auxColor.a = alpha;

            _image.color = auxColor;            
        }

        public Vector2 GetCoordinates()
        {
            return _coordinates;
        }
    }
}
