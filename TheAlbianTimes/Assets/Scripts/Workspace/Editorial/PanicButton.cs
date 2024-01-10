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

        private void Start()
        {
            _audioSourcePressPanicButton = gameObject.AddComponent<AudioSource>();
            (AudioSource, String)[] tuples =
            {
                (_audioSourcePressPanicButton, PRESS_PANIC_BUTTON_SOUND)
            };
            SoundManager.Instance.SetMultipleAudioSourcesComponents(tuples);
        }

        protected override void PointerClick(BaseEventData data)
        {
            _audioSourcePressPanicButton.Play();
            if (EventsManager.OnPressPanicButton != null)
            {
                EventsManager.OnPressPanicButton(true);
            }
            
            if (EventsManager.OnPressPanicButtonForPieces == null)
            {
                return;
            }

            EventsManager.OnPressPanicButtonForPieces();
        }
    }
}
