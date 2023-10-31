using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewsHeadline : MovableRectTransform
{

    [SerializeField] private NewsType newsType;

    protected override void EndDrag(BaseEventData data)
    {
        base.EndDrag(data);

        PointerEventData pointerData = (PointerEventData)data;

        //pointerData.position
    }

}
