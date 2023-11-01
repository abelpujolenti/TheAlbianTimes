using UnityEngine;
using UnityEngine.EventSystems;

namespace Layout
{
    public class NewsHeadlinePiece : MovableRectTransform
    {
        [SerializeField] private NewHeadline _newHeadline;
        
        [SerializeField] private Vector2 _coordinates;

        protected override void BeginDrag(BaseEventData data)
        {
            base.BeginDrag(data);
            
            _newHeadline.BeginDrag();
        }

        protected override void EndDrag(BaseEventData data)
        {
            base.EndDrag(data);

            PointerEventData pointerData = (PointerEventData) data;
            
            if (ActionsManager.OnReleaseNewsHeadline != null)
            {
                ActionsManager.OnReleaseNewsHeadline(this, pointerData.position);
            }
        }

        public Vector2 GetCoordinates()
        {
            return _coordinates;
        }
    }
}
