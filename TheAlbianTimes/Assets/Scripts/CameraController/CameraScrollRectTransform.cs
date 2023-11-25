using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

public class CameraScrollRectTransform : InteractableRectTransform
{
    private float _initialMouseXPosition;

    protected override void Drag(BaseEventData data)
    {
        if (!held) return;

        PointerEventData pointerData = (PointerEventData)data;

        float mouseXPosition = Camera.main.ScreenToWorldPoint(new Vector3(pointerData.position.x, 0, 0)).x;

        float difference = mouseXPosition - Camera.main.transform.position.x;

        float nextXPosition = _initialMouseXPosition - difference;

        if (nextXPosition < MIN_X_POSITION_CAMERA || nextXPosition > MAX_X_POSITION_CAMERA)
        {
            return;
        }

        Camera.main.transform.position = new Vector3(_initialMouseXPosition - difference, 0, -10);
    }
    protected override void BeginDrag(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;

        _initialMouseXPosition = Camera.main.ScreenToWorldPoint(pointerData.position).x;

        base.BeginDrag(data);
    }
    protected override void EndDrag(BaseEventData data)
    {
        base.EndDrag(data);
    }
}
