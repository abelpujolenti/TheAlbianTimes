using System;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

namespace Workspace.Editorial
{
    public class PanicButton : InteractableRectTransform
    {
        private const String PRESS_PANIC_BUTTON_SOUND = "Press Panic Button";

        [SerializeField] private Image _image;

        private Color _unpressedColor = Color.white;
        private Color _pressedColor = Color.gray;

        private bool _pressed;

        //private AudioSource _audioSourcePressPanicButton;

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
            /*_audioSourcePressPanicButton = gameObject.AddComponent<AudioSource>();
            (AudioSource, String)[] tuples =
            {
                (_audioSourcePressPanicButton, PRESS_PANIC_BUTTON_SOUND)
            };
            SoundManager.Instance.SetMultipleAudioSourcesComponents(tuples);*/
            gameObject.SetActive(false);
        }

        protected override void PointerDown(BaseEventData data)
        {
            _pressed = true;
            _image.color = _pressedColor;
            base.PointerDown(data);
        }

        protected override void PointerClick(BaseEventData data)
        {
            EventsManager.OnThowSomething = () => {
                gameObject.SetActive(true);
                EventsManager.OnThowSomething = null;
            };
            //_audioSourcePressPanicButton.Play();
            SoundManager.Instance.Play2DSound(PRESS_PANIC_BUTTON_SOUND);
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

        protected override void PointerEnter(BaseEventData data)
        {
            if (_pressed)
            {
                _image.color = _pressedColor;
            }
            base.PointerEnter(data);
        }

        protected override void PointerExit(BaseEventData data)
        {
            if (_pressed)
            {
                _image.color = _unpressedColor;
            }
            base.PointerExit(data);
        }

        protected override void PointerUp(BaseEventData data)
        {
            if (_pressed)
            {
                _pressed = false;
                _image.color = _unpressedColor;
            }
            base.PointerUp(data);
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
