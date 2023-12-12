using Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueOptionButton : MonoBehaviour
{
    [SerializeField] DialogueManager dialogueManager;
    public int optionId;
    private TextMeshProUGUI buttonText;

    private void Start()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Click()
    {
        dialogueManager.SelectOption(optionId, transform.GetSiblingIndex());
    }
    public void SetButtonText(string text)
    {
        if (buttonText == null) buttonText = GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = text;
    }
}
