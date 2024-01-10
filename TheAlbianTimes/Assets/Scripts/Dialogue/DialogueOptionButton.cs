using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueOptionButton : MonoBehaviour
{
    [SerializeField] DialogueManager dialogueManager;
    private TextMeshProUGUI buttonText;
    private Image buttonBackground;
    private DialogueOption data;
    private bool conditionsFulfilled = true;

    private Color backgroundColor;

    private void OnEnable()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        buttonBackground = GetComponent<Image>();
        if (backgroundColor == new Color(0f, 0f, 0f, 0f))
        {
            backgroundColor = buttonBackground.color;
        }
    }

    public void Click()
    {
        if (conditionsFulfilled)
        {
            dialogueManager.SelectOption(data.id, transform.GetSiblingIndex());
        }
    }

    public void Setup(DialogueOption data)
    {
        this.data = data;
        SetButtonText(data.text);
        SetEnabledOption();
        CheckConditions();
        if (!conditionsFulfilled)
        {
            SetDisabledoption();
        }
    }

    public void SetButtonText(string text)
    {
        buttonText.text = text;
    }

    private void CheckConditions()
    {
        conditionsFulfilled = true;
        if (data.countryConditions != null)
        {
            foreach (CountryEventCondition condition in data.countryConditions)
            {
                if (!condition.IsFulfilled())
                {
                    conditionsFulfilled = false;
                    break;
                }
            }
        }
        if (data.characterConditions != null)
        {
            foreach (CharacterEventCondition condition in data.characterConditions)
            {
                if (!condition.IsFulfilled())
                {
                    conditionsFulfilled = false;
                    break;
                }
            }
        }
    }

    private void SetEnabledOption()
    {
        buttonBackground.color = backgroundColor;
    }

    private void SetDisabledoption()
    {
        buttonBackground.color = ColorUtil.SetBrightness(ColorUtil.SetSaturation(backgroundColor, .05f), .4f);
    }
}
