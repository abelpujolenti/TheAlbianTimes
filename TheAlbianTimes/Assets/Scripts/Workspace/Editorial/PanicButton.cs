using System;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Workspace.Editorial
{
    public class PanicButton : InteractableRectTransform
    {
        private const String PRESS_PANIC_BUTTON_SOUND = "Press Panic Button";

        private AudioSource _audioSourcePressPanicButton;

        private void OnEnable()
        {
            EventsManager.OnArrangeSomething = HideIfNecessary;
            EventsManager.OnThowSomething = null;
        }

        private void OnDisable()
        {
            EventsManager.OnArrangeSomething = null;
            EventsManager.OnThowSomething = () => {
                gameObject.SetActive(true);
                EventsManager.OnThowSomething = null;
            };
        }

        private void Start()
        {
            _audioSourcePressPanicButton = gameObject.AddComponent<AudioSource>();
            (AudioSource, String)[] tuples =
            {
                (_audioSourcePressPanicButton, PRESS_PANIC_BUTTON_SOUND)
            };
            SoundManager.Instance.SetMultipleAudioSourcesComponents(tuples);
            gameObject.SetActive(false);
        }

        protected override void PointerClick(BaseEventData data)
        {

            EventsManager.OnThowSomething = () => {
                gameObject.SetActive(true);
                EventsManager.OnThowSomething = null;
            };
            _audioSourcePressPanicButton.Play();
            if (EventsManager.OnPressPanicButton != null)
            {
                EditorialManager.Instance.SetTotalNewsToLoad(EventsManager.OnPressPanicButton.GetInvocationList().Length);
                EventsManager.OnPressPanicButton();
            }
            
            if (EventsManager.OnPressPanicButtonForPieces != null)
            {
                EventsManager.OnPressPanicButtonForPieces();
            }            

            gameObject.SetActive(false);
        }

        private void HideIfNecessary()
        {
            if (EventsManager.OnPressPanicButton != null)
            {
                return;
            }

            if (EventsManager.OnPressPanicButtonForPieces != null)
            {
                return;
            }

            gameObject.SetActive(false);
        }
    }
}
