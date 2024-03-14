using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace CameraController
{
    public class CameraScrollRectTransform : InteractableRectTransform
    {
        [SerializeField] private float snapThreshold = 6f;
        [SerializeField] private float snapTime = .3f;
        private float _initialMouseXPosition;
        private Coroutine moveCameraCoroutine;
        private Transform camTransform;

        protected override void Setup()
        {
            base.Setup();
            camTransform = Camera.main.transform;
        }

        protected override void Drag(BaseEventData data)
        {
            if (!held) return;

            PointerEventData pointerData = (PointerEventData)data;

            float mouseXPosition = Camera.main.ScreenToWorldPoint(new Vector3(pointerData.position.x, 0, 0)).x;

            float difference = mouseXPosition - camTransform.position.x;

            float nextXPosition = _initialMouseXPosition - difference;

            if (nextXPosition < MIN_X_POSITION_CAMERA || nextXPosition > MAX_X_POSITION_CAMERA)
            {
                return;
            }

            camTransform.position = new Vector3(_initialMouseXPosition - difference, 0, -10);
        }
        protected override void BeginDrag(BaseEventData data)
        {
            PointerEventData pointerData = (PointerEventData)data;

            _initialMouseXPosition = Camera.main.ScreenToWorldPoint(pointerData.position).x;

            if (moveCameraCoroutine != null)
            {
                StopCoroutine(moveCameraCoroutine);
            }

            base.BeginDrag(data);
        }
        protected override void EndDrag(BaseEventData data)
        {
            base.EndDrag(data);
            CheckSnap();
        }
        private void CheckSnap()
        {
            if (camTransform.position.x > MAX_X_POSITION_CAMERA - snapThreshold)
            {
                CameraManager.Instance.PanToLayout(snapTime);
            }
            else if (camTransform.position.x < MIN_X_POSITION_CAMERA + snapThreshold)
            {
                CameraManager.Instance.PanToEditorial(snapTime);
                return;
            }

            CameraManager.Instance.SetCameraOnCenter();
        }
    }
}
