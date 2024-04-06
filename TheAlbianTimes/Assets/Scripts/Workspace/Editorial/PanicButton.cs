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
            SoundManager.Instance.Play3DSound(PRESS_PANIC_BUTTON_SOUND, 5, 100, transform.position);
            if (EventsManager.OnPressPanicButton != null)
            {
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
