using System;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace CameraController
{
    public class CameraButtonScroll : InteractableRectTransform
    {
        [SerializeField] private bool _toLayout;

        private Action _panCamera;

        private void Start()
        {
            if (_toLayout) 
            {
                _panCamera = () => CameraManager.Instance.PanToLayout(0.5f);
                return;
            }
            _panCamera = () => CameraManager.Instance.PanToEditorial(0.5f);
        }

        protected override void PointerClick(BaseEventData data)
        {        
            _panCamera();
        }
    }
}
