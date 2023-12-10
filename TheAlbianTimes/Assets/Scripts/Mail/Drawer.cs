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
        _moveContainerCoroutine = StartCoroutine(MoveContainerEnum(gameObjectToDrag.transform.position.x, maxX, openTime));
        SoundManager.Instance.OpenDrawerSound();
    }

    protected virtual void CloseContainer()
    {
        _moveContainerCoroutine = StartCoroutine(MoveContainerEnum(gameObjectToDrag.transform.position.x, minX, closeTime));
        SoundManager.Instance.CloseDrawerSound();
    }

    private IEnumerator MoveContainerEnum(float start, float end, float t)
    {
        float elapsedT = 0f;
        while (elapsedT <= t)
        {
            Vector3 newPos = gameObjectToDrag.transform.position;
            newPos.x = Mathf.SmoothStep(start, end, Mathf.Pow(Mathf.Min(1f, elapsedT / t), 2));
            gameObjectToDrag.transform.position = newPos;
            yield return new WaitForFixedUpdate();
            elapsedT += Time.fixedDeltaTime;
        }
        Vector3 endPos = gameObjectToDrag.transform.position;
        endPos.x = end;
        gameObjectToDrag.transform.position = endPos;
    }

    public bool IsOpen()
    {
        return gameObjectToDrag.transform.position.x > minX + isOpenThreshold;
    }

}
