using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Workspace.Notebook.Pages.Map
{
    public abstract class NotebookMapPage : NotebookContentPage
    {
        private const float TIME_TO_FADE_IN = 0.7f;
        private const float TIME_TO_FADE_OUT = 0.3f;
        private const float MAX_ALPHA_BACKGROUND = 0.65f;
        private const float MAX_ALPHA_BUTTON = 1f;
        
        [SerializeField] private Image[] _backgroundsToFade;
        [SerializeField] private Image[] _buttonsToFade;

        private void Start()
        {
            EventsManager.OnOpenMapPages += StartButtonsFadeIn;
            EventsManager.OnCloseMapPages += StartButtonsFadeOut;
        }

        private void OnDestroy()
        {
            EventsManager.OnOpenMapPages -= StartButtonsFadeIn;
            EventsManager.OnCloseMapPages -= StartButtonsFadeOut;
        }
        
        private void StartButtonsFadeIn()
        {
            StartCoroutine(ButtonsFade(0, MAX_ALPHA_BACKGROUND, 0, MAX_ALPHA_BUTTON, TIME_TO_FADE_IN));
        }

        private void StartButtonsFadeOut()
        {
            StartCoroutine(ButtonsFade(MAX_ALPHA_BACKGROUND, 0, MAX_ALPHA_BUTTON, 0, TIME_TO_FADE_OUT));
        }

        private IEnumerator ButtonsFade(float startAlphaBackground, float endAlphaBackground,
            float startAlphaButton, float endAlphaButton, float timeToFade)
        {
            float timer = 0;

            Color backgroundColor = Color.white;
            Color buttonColor = Color.white;

            while (timer < timeToFade)
            {
                timer += Time.deltaTime;

                backgroundColor.a = Mathf.Lerp(startAlphaBackground, endAlphaBackground, timer / timeToFade);
                
                foreach (Image background in _backgroundsToFade)
                {
                    background.color = backgroundColor;
                }
                
                buttonColor.a = Mathf.Lerp(startAlphaButton, endAlphaButton, timer / timeToFade);

                foreach (Image button in _buttonsToFade)
                {
                    button.color = buttonColor;
                }

                yield return null;
            }

            backgroundColor.a = endAlphaBackground;
            buttonColor.a = endAlphaButton;
                
            foreach (Image background in _backgroundsToFade)
            {
                background.color = backgroundColor;
            }

            foreach (Image button in _buttonsToFade)
            {
                button.color = buttonColor;
            }
        }
    }
}
