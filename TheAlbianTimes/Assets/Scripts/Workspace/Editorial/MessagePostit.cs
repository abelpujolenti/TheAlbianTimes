using TMPro;
using UnityEngine;
using Utility;

namespace Workspace.Editorial
{
    public class MessagePostit : ThrowableInteractableRectTransform
    {
        [SerializeField] private TextMeshProUGUI _text;
        public void Setup(MessagePostitData data)
        {
            _text.text = data.text;
        }
    }
}
