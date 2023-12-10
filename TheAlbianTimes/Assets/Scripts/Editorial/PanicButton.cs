using Managers;
using UnityEngine.EventSystems;
using Utility;

namespace Editorial
{
    public class PanicButton : InteractableRectTransform
    {
        protected override void PointerClick(BaseEventData data)
        {
            EventsManager.OnPressPanicButton();
        }
    }
}
