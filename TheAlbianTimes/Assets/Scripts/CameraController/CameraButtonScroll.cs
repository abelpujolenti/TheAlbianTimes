using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

public class CameraButtonScroll : InteractableRectTransform
{
    [SerializeField] private bool _toLayout;

    private Action _panCamera;

    private void Start()
    {
        if (_toLayout) 
        {
            _panCamera = () => CameraManager.Instance.PanToLayout(0.5f, false);
            return;
        }
        _panCamera = () => CameraManager.Instance.PanToEditorial(0.5f, false);
    }

    protected override void PointerClick(BaseEventData data)
    {        
        _panCamera();
    }
}
