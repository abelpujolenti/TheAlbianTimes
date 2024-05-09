using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Menus
{
    public class PauseInGameButton : InteractableRectTransform
    {
        private const string GRAB_COFFE_SOUND = "Grab Coffee";
        private const string LEAVE_COFFE_SOUND = "Leave Coffee";
        
        [SerializeField] private InGame _inGameMenu;

        private bool _dragged;

        protected override void BeginDrag(BaseEventData data)
        {
            AudioManager.Instance.Play3DSound(GRAB_COFFE_SOUND, 5, 100, transform.position);
            base.BeginDrag(data);
            _dragged = true;
        }

        protected override void Drag(BaseEventData data)
        {
            base.Drag(data);
        }

        protected override void EndDrag(BaseEventData data)
        {
            AudioManager.Instance.Play3DSound(LEAVE_COFFE_SOUND, 5, 100, transform.position);
            base.EndDrag(data);
        }

        protected override void PointerClick(BaseEventData data)
        {
            if (!_dragged)
            {
                _inGameMenu.PauseButton();
            }

            _dragged = false;
        }
    }
}