using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Workspace.Layout
{
    public class MoldLocker : InteractableRectTransform
    {
        [SerializeField] private NewspaperMold _newsPaperMold;

        protected override void PointerClick(BaseEventData data)
        {
            if (!LayoutManager.Instance.IsMoldInPlace())
            {
                return;
            }
            _newsPaperMold.SetDraggable(!_newsPaperMold.IsDraggable());
        }
    }
}