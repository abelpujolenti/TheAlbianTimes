using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

public class Drawer : InteractableRectTransform
{
    [SerializeField] protected float maxX = -10f;
    [SerializeField] protected float minX = -14.7f;
    [SerializeField] protected float isOpenThreshold = 1.2f;
    [SerializeField] protected float openTime = 1f;
    [SerializeField] protected float closeTime = .8f;
    protected Coroutine _moveContainerCoroutine;
    protected override void Drag(BaseEventData data)
    {
        if (!held && !draggable) return;

        if (_moveContainerCoroutine != null)
        {
            StopCoroutine(_moveContainerCoroutine);
        }

        Vector2 mousePosition = GetMousePositionOnCanvas(data);

        Vector2 newPos = (Vector2)canvas.transform.TransformPoint(mousePosition) + _vectorOffset;
        newPos.y = gameObjectToDrag.transform.position.y;
        newPos.x = Mathf.Min(maxX, Mathf.Max(minX, newPos.x));
        gameObjectToDrag.transform.position = newPos;
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
        SoundManager.Instance.OpenDrawerSound();
    }

    protected virtual void CloseContainer()
    {
        Vector3 end = new Vector3(minX, gameObjectToDrag.transform.position.y, gameObjectToDrag.transform.position.z);
        _moveContainerCoroutine = StartCoroutine(SetPositionCoroutine(gameObjectToDrag.transform.position, end, closeTime));
        SoundManager.Instance.CloseDrawerSound();
    }

    public bool IsOpen()
    {
        return gameObjectToDrag.transform.position.x > minX + isOpenThreshold;
    }

}
