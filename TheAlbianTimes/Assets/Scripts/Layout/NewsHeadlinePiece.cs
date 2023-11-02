using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Layout
{
    public class NewsHeadlinePiece : MovableRectTransform
    {
        [SerializeField] private NewHeadline _newHeadline;
        
        [SerializeField] private Vector2 _coordinates;

        private Image _image;

        private new void Start()
        {
            base.Start();
            _image = GetComponent<Image>();
        }

        protected override void BeginDrag(BaseEventData data)
        {
            base.BeginDrag(data);
            
            _newHeadline.BeginDrag();
        }

        protected override void EndDrag(BaseEventData data)
        {
            base.EndDrag(data);

            PointerEventData pointerData = (PointerEventData) data;
            
            if (ActionsManager.OnReleaseNewsHeadline == null)
            {
                return;
            }

            ActionsManager.OnReleaseNewsHeadline(this, pointerData.position);
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
