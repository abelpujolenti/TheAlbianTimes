using System;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Workspace.Mail
{
    public class Drawer : InteractableRectTransform
    {
        private const String OPEN_DRAWER_SOUND = "Open Drawer";
        private const String CLOSE_DRAWER_SOUND = "Close Drawer";

        [SerializeField] private RectTransform handleTarget;
        private float baseHandleWidth;

        [SerializeField] protected float maxX = -10f;
        [SerializeField] protected float minX = -14.7f;
        [SerializeField] protected float isOpenThreshold = 1.2f;
        [SerializeField] protected float openTime = 1f;
        [SerializeField] protected float closeTime = .8f;
        protected Coroutine _moveContainerCoroutine;

        private void Start()
        {
            baseHandleWidth = handleTarget.sizeDelta.x;
        }

        protected override void Drag(BaseEventData data)
        {
            if (!held && !draggable) return;

            if (_moveContainerCoroutine != null)
            {
                StopCoroutine(_moveContainerCoroutine);
            }

            Vector2 mousePosition = GetMousePositionOnCanvas(data);

            Vector3 newPos = canvas.transform.TransformPoint(mousePosition) + _vectorOffset;
            newPos.y = gameObjectToDrag.transform.position.y;
            newPos.x = Mathf.Min(maxX, Mathf.Max(minX, newPos.x));
            gameObjectToDrag.transform.position = newPos;

            handleTarget.sizeDelta = new Vector2(Mathf.Min(rectTransform.sizeDelta.x, baseHandleWidth + Mathf.InverseLerp(minX, maxX, gameObjectToDrag.transform.position.x) * rectTransform.sizeDelta.x), handleTarget.sizeDelta.y);
        }

        protected override void PointerUp(BaseEventData data)
        {
            if (held) return;
            base.PointerClick(data);
            if (_moveContainerCoroutine != null)
            {
                StopCoroutine(_moveContainerCoroutine);
            }
            if (IsOpen())
            {
                CloseContainer();
            }
            else
            {
                OpenContainer();
            }
        }

        protected virtual void OpenContainer()
        {
            Vector3 end = new Vector3(maxX, gameObjectToDrag.transform.position.y, gameObjectToDrag.transform.position.z);
            _moveContainerCoroutine = StartCoroutine(SetPositionCoroutine(gameObjectToDrag.transform.position, end, openTime));
            SoundManager.Instance.Play3DSound(OPEN_DRAWER_SOUND, 10, 100, gameObject.transform.position);

            handleTarget.sizeDelta = new Vector2(rectTransform.sizeDelta.x, handleTarget.sizeDelta.y);
        }

        protected virtual void CloseContainer()
        {
            Vector3 end = new Vector3(minX, gameObjectToDrag.transform.position.y, gameObjectToDrag.transform.position.z);
            _moveContainerCoroutine = StartCoroutine(SetPositionCoroutine(gameObjectToDrag.transform.position, end, closeTime));
            SoundManager.Instance.Play3DSound(CLOSE_DRAWER_SOUND, 10, 100, gameObject.transform.position);

            handleTarget.sizeDelta = new Vector2(baseHandleWidth, handleTarget.sizeDelta.y);
        }

        private bool IsOpen()
        {
            return gameObjectToDrag.transform.position.x > minX + isOpenThreshold;
        }
    }
}
