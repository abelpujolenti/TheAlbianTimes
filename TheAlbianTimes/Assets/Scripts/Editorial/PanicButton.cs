using Managers;
using UnityEngine.EventSystems;
using Utility;

namespace Editorial
{
    public class PanicButton : InteractableRectTransform
    {
        protected override void PointerClick(BaseEventData data)
        {
            if (EventsManager.OnPressPanicButton == null)
            {
                return;
            }
            EventsManager.OnPressPanicButton();
        }
    }
}
