using Countries;
using Managers;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
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
            SetDisabledOption();
        }

        DisplayConditions();
    }

    public void DisplayConditions()
    {
        Dictionary<string, CountryEventCondition> conditionIconNames = new Dictionary<string, CountryEventCondition>();
        if (data.countryConditions != null)
        {
            foreach (CountryEventCondition condition in data.countryConditions)
            {
                int id = condition.entityId;
                string name = Country.names[id];
                if (condition.field != "reputation") continue;
                if (conditionIconNames.ContainsKey(name) && condition.IsFulfilled()) return;
                conditionIconNames[name] = condition;
            }
        }

        Sprite bg = Resources.Load<Sprite>("Images/Icons/icon_background");

        int i = 0;
        foreach (KeyValuePair<string, CountryEventCondition> condition in conditionIconNames)
        {
            int id = condition.Value.entityId;
            float xDist = 1f;
            float x = xDist * Mathf.Pow(-1, i) * ((i + 1) / 2) + (xDist * 0.5f * (conditionIconNames.Count + 1) % 2) + transform.position.x;
            Vector2 iconPos = new Vector3(x, transform.position.y + 1f, transform.position.z);
            Color iconColor = condition.Value.IsFulfilled() ? new Color(.5f, .7f, .5f) : new Color(.7f, .5f, .5f);
            string value = GameManager.Instance.gameState.countries[id].GetReputation().ToString("p0");
            CreateConditionIcon(iconPos, condition.Key, value, iconColor, bg);

            i++;
        }
    }

    private void CreateConditionIcon(Vector3 position, string iconName, string value, Color iconColor, Sprite bg)
    {
        Sprite s = Resources.Load<Sprite>("Images/Icons/" + iconName);
        GameObject iconObject = FakeInstantiate.Instantiate(transform, position);
        Image iconImage = iconObject.AddComponent<Image>();
        iconImage.sprite = s;
        float iconSize = 12f;
        float scale = iconSize / s.pixelsPerUnit;
        iconImage.rectTransform.sizeDelta = new Vector2(s.textureRect.width * scale, s.textureRect.height * scale);
        iconImage.color = iconColor;

        GameObject backgroundObject = FakeInstantiate.Instantiate(transform, position);
        Image background = backgroundObject.AddComponent<Image>();
        background.sprite = bg;
        background.transform.SetAsFirstSibling();
        background.rectTransform.sizeDelta = new Vector2(60f, 60f);
        background.color = new Color(background.color.r, background.color.g, background.color.b, 0.8f);

        GameObject textObject = FakeInstantiate.Instantiate(iconImage.transform);
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.color = Color.white;
        text.fontSize = 12f;
        text.alignment = TextAlignmentOptions.MidlineGeoAligned;
        text.text = "<font=Poppins-Regular SDF><b>" + value;
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

    private void SetDisabledOption()
    {
        buttonBackground.color = ColorUtil.SetBrightness(ColorUtil.SetSaturation(backgroundColor, .05f), .4f);
    }
}
