using System.Collections;
using System.Collections.Generic;
using Countries;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Dialogue
{
    public class DialogueOptionButton : MonoBehaviour
    {
        [SerializeField] DialogueManager dialogueManager;
        private TextMeshProUGUI buttonText;
        private TextMeshProUGUI moodText;
        private Image buttonBackground;
        private DialogueOption data;
        private bool conditionsFulfilled = true;

        private Color backgroundColor;

        private void OnEnable()
        {
            buttonText = transform.Find("text").GetComponent<TextMeshProUGUI>();
            moodText = transform.Find("mood").GetComponent<TextMeshProUGUI>();
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

        public void Setup(DialogueOption data, float duration, float delay)
        {
            this.data = data;
            SetEnabledOption();
            CheckConditions();
            if (!conditionsFulfilled)
            {
                SetDisabledOption();
            }
            StartCoroutine(SetupCoroutine(duration, delay));
        }

        private IEnumerator SetupCoroutine(float t, float delay)
        {
            SetButtonText("");
            SetMoodText("");
            float finalHeight = buttonBackground.rectTransform.sizeDelta.y;
            buttonBackground.rectTransform.sizeDelta = new Vector2(buttonBackground.rectTransform.sizeDelta.x, 0f);

            yield return new WaitForSeconds(delay);

            float elapsedT = 0f;
            while (elapsedT <= t)
            {
                float h = Mathf.Lerp(0f, finalHeight, Mathf.Pow((elapsedT / t), .5f));
                buttonBackground.rectTransform.sizeDelta = new Vector2(buttonBackground.rectTransform.sizeDelta.x, h);
                yield return new WaitForFixedUpdate();
                elapsedT += Time.fixedDeltaTime;
            }
            buttonBackground.rectTransform.sizeDelta = new Vector2(buttonBackground.rectTransform.sizeDelta.x, finalHeight);

            SetButtonText(data.text);
            SetMoodText(data.mood);
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
            float scale = 10f;
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

        public void SetMoodText(string text)
        {
            if (text == null) text = "";
            moodText.text = text;
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
}
