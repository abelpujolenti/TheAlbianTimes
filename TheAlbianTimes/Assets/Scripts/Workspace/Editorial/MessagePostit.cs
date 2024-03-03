using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utility;

public class MessagePostit : ThrowableInteractableRectTransform
{
    public void SetText(string text)
    {
        GetComponentInChildren<TextMeshProUGUI>().text = text;
    }
}
