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

        private float _currentBackgroundAlpha;
        private float _currentButtonAlpha;

        private Coroutine _fadeCoroutine;

        private void OnEnable()
        {
            EventsManager.OnOpenMapPages += StartButtonsFadeIn;
            EventsManager.OnCloseMapPages += StartButtonsFadeOut;
        }

        private void OnDisable()
        {
            EventsManager.OnOpenMapPages -= StartButtonsFadeIn;
            EventsManager.OnCloseMapPages -= StartButtonsFadeOut;
        }
        
        private void StartButtonsFadeIn()
        {
            _fadeCoroutine = StartCoroutine(ButtonsFade(0, MAX_ALPHA_BACKGROUND, 0, MAX_ALPHA_BUTTON, TIME_TO_FADE_IN));
        }

        private void StartButtonsFadeOut()
        {
            EraseClickers();
            if (_fadeCoroutine == null)
            {
                StartCoroutine(ButtonsFade(MAX_ALPHA_BACKGROUND, 0, MAX_ALPHA_BUTTON, 0, TIME_TO_FADE_OUT));
                return;
            }
            StopCoroutine(_fadeCoroutine);
            StartCoroutine(ButtonsFade(_currentBackgroundAlpha, 0, _currentButtonAlpha, 0, TIME_TO_FADE_OUT));
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
                
                _currentBackgroundAlpha = Mathf.Lerp(startAlphaBackground, endAlphaBackground, timer / timeToFade);

                backgroundColor.a = _currentBackgroundAlpha;
                
                foreach (Image background in _backgroundsToFade)
                {
                    background.color = backgroundColor;
                }
                
                _currentButtonAlpha = Mathf.Lerp(startAlphaButton, endAlphaButton, timer / timeToFade);

                buttonColor.a = _currentButtonAlpha;

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

            _fadeCoroutine = null;
        }

        protected abstract void EraseClickers();
    }
}
