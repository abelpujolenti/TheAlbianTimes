using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MovableRectTransform : MonoBehaviour
{
    #region config
    public bool draggable = true;
    public bool hoverable = true;
    #endregion
    #region variables
    public bool held { get; protected set; }
    protected Vector2 direction;
    protected RectTransform rectTransform;
    protected Canvas canvas;
    protected RectTransform canvasRect;
    protected EventTrigger eventTrigger;
    #endregion
    private void Start()
    {
        Setup();
    }
    protected virtual void Setup()
    {
        rectTransform = (RectTransform)transform;
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = transform.parent.GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();

        eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        }
        if (draggable)
        {
            AddEventTrigger(EventTriggerType.Drag, Drag);
            AddEventTrigger(EventTriggerType.BeginDrag, BeginDrag);
            AddEventTrigger(EventTriggerType.EndDrag, EndDrag);
        }
        if (hoverable)
        {
            AddEventTrigger(EventTriggerType.PointerEnter, PointerEnter);
            AddEventTrigger(EventTriggerType.PointerExit, PointerExit);
        }
    }
    protected void AddEventTrigger(EventTriggerType triggerType, Action<BaseEventData> func)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = triggerType;
        entry.callback.AddListener((eventData) => { func(eventData); });
        eventTrigger.triggers.Add(entry);
    }

    protected virtual void Drag(BaseEventData data)
    {
        if (!held || !draggable) return;

        PointerEventData pointerData = (PointerEventData)data;
        Vector2 canvasTopRight = new Vector2(canvasRect.rect.width / 2, canvasRect.rect.height / 2);
        Vector2 canvasBottomLeft = new Vector2(-canvasRect.rect.width / 2, -canvasRect.rect.height / 2);
        
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            pointerData.position,
            Camera.main,
            out mousePosition
            );
        mousePosition.x = Math.Max(Math.Min(mousePosition.x, canvasTopRight.x), canvasBottomLeft.x);
        mousePosition.y = Math.Max(Math.Min(mousePosition.y, canvasTopRight.y), canvasBottomLeft.y);

        transform.position = canvas.transform.TransformPoint(mousePosition);
    }
    protected virtual void BeginDrag(BaseEventData data)
    {
        held = true;
    }
    protected virtual void EndDrag(BaseEventData data)
    {
        held = false;
    }

    public virtual void PointerEnter(BaseEventData data)
    {
    }
    public virtual void PointerExit(BaseEventData data)
    {
    }
}
