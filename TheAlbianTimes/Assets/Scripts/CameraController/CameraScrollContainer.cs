using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace CameraController
{
    public class CameraScrollContainer : MonoBehaviour
    {
        [SerializeField] private GameObject _rightCameraSideScroll;
        [SerializeField] private GameObject _leftCameraSideScroll;

        private Dictionary<CameraLocation, Action<bool>> _modifyActiveCameraSideScrolls;

        private void Start()
        {
            _modifyActiveCameraSideScrolls = new Dictionary<CameraLocation, Action<bool>>
            {
                { CameraLocation.EDITORIAL , active => _rightCameraSideScroll.SetActive(active)},
                { CameraLocation.LAYOUT , active => _leftCameraSideScroll.SetActive(active)},
                { CameraLocation.CENTER, active =>
                {
                    _rightCameraSideScroll.SetActive(active);
                    _leftCameraSideScroll.SetActive(active);
                } }
            };
        }

        private void OnEnable()
        {
            EventsManager.OnStartEndDrag += ModifyActiveCameraSideScrolls;
        }

        private void OnDisable()
        {
            EventsManager.OnStartEndDrag -= ModifyActiveCameraSideScrolls;
        }

        private void ModifyActiveCameraSideScrolls(bool active)
        {
            if (CameraManager.Instance.IsScrolling())
            {
                return;
            }

            _modifyActiveCameraSideScrolls[CameraManager.Instance.GetCameraLocation()](active);
        }

        public void FlipSideScroll(RectTransform cameraSideScrollRectTransform)
        {
            Vector2 size = cameraSideScrollRectTransform.sizeDelta;
            cameraSideScrollRectTransform.sizeDelta = new Vector2(-size.x, size.y);
        }
    }
}