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

        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RectTransform _limits;

        private Vector3 _size;
        
        private Vector2 _containerMinCoordinates;
        private Vector2 _containerMaxCoordinates;

        private bool _dragged;

        private void Start()
        {
            Vector3[] corners = new Vector3[4];
            
            _limits.GetWorldCorners(corners);
            
            SetContainerLimiters(corners);
            
            Vector3 sizeDelta = _rectTransform.sizeDelta;
            Vector3 lossyScale = _rectTransform.lossyScale;
            _size = new Vector3(lossyScale.x * sizeDelta.x, lossyScale.y * sizeDelta.y, 0);
        }
        
        private void SetContainerLimiters(Vector3[] corners)
        {
            _containerMinCoordinates.x = corners[0].x;
            _containerMaxCoordinates.y = corners[1].y;
            _containerMaxCoordinates.x = corners[2].x;
            _containerMinCoordinates.y = corners[3].y;
        }

        protected override void BeginDrag(BaseEventData data)
        {
            AudioManager.Instance.Play3DSound(GRAB_COFFE_SOUND, 5, 100, transform.position);
            base.BeginDrag(data);
            _dragged = true;
        }

        protected override void Drag(BaseEventData data)
        {
            base.Drag(data);
            Vector3 position = transform.position;

            float positionX = Mathf.Max(_containerMinCoordinates.x + _size.x / 2 , Mathf.Min(_containerMaxCoordinates.x - _size.x / 2, position.x));
            float positionY = Mathf.Max(_containerMinCoordinates.y + _size.y / 2, Mathf.Min(_containerMaxCoordinates.y - _size.y / 2, position.y));

            transform.position = new Vector3(positionX, positionY, position.z);
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
                InGame.Instance.PauseButton();
            }

            _dragged = false;
        }
    }
}