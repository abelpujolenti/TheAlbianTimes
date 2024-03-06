using TMPro;
using Utility;

public class MessagePostit : ThrowableInteractableRectTransform
{
    public void SetText(string text)
    {
        GetComponentInChildren<TextMeshProUGUI>().text = text;
    }
}
