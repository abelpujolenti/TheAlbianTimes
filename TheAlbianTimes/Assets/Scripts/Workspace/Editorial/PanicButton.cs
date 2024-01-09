using Managers;
using UnityEngine.EventSystems;
using Utility;

namespace Workspace.Editorial
{
    public class PanicButton : InteractableRectTransform
    {
        protected override void PointerClick(BaseEventData data)
        {
            if (EventsManager.OnPressPanicButton != null)
            {
                EventsManager.OnPressPanicButton(true);
            }
            
            if (EventsManager.OnPressPanicButtonForPieces == null)
            {
                return;
            }

            EventsManager.OnPressPanicButtonForPieces();
        }
    }
}
