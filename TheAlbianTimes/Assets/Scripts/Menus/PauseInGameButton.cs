using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Menus
{
    public class PauseInGameButton : InteractableRectTransform
    {

        [SerializeField] private InGame _inGameMenu;

        private bool _dragged;

        protected override void BeginDrag(BaseEventData data)
        {
            _dragged = true;
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