using TMPro;
using Utility;

namespace Workspace.Editorial
{
    public class MessagePostit : ThrowableInteractableRectTransform
    {
        public void Setup(MessagePostitData data)
        {
            var tmp = GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = data.text;
            tmp.color = data.color;
        }
    }
}
