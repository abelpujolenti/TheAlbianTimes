using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

public class NotebookBookmark : InteractableRectTransform
{
    public int page = 0;
    [SerializeField] private Notebook notebook;
    [SerializeField] private Transform pageMarkerActiveParent;

    private Coroutine moveUpCoroutine;
    private Coroutine moveDownCoroutine;

    protected override void PointerClick(BaseEventData data)
    {
        base.PointerClick(data);
        notebook.ClickFromBooknote(page);
        SetToActive();
        MoveDown();
    }
    protected override void PointerEnter(BaseEventData data)
    {
        base.PointerEnter(data);
        if (transform.parent != pageMarkerActiveParent)
        {
            MoveUp();
        }
    }
    protected override void PointerExit(BaseEventData data)
    {
        base.PointerExit(data);
        MoveDown();
    }

    private void SetToActive()
    {
        if (transform.parent == pageMarkerActiveParent) return;
        
        for (int i = 0; i < pageMarkerActiveParent.childCount; i++)
        {
            pageMarkerActiveParent.GetChild(i).SetParent(transform.parent);
        }
        transform.SetParent(pageMarkerActiveParent);
    }

    private void MoveUp()
    {
        if (moveUpCoroutine != null) StopCoroutine(moveUpCoroutine);
        if (moveDownCoroutine != null) StopCoroutine(moveDownCoroutine);
        moveUpCoroutine = StartCoroutine(MoveCoroutine(5f, 0.15f));
    }

    private void MoveDown()
    {
        if (transform.localPosition.y == 0f) return;
        if (moveUpCoroutine != null) StopCoroutine(moveUpCoroutine);
        if (moveDownCoroutine != null) StopCoroutine(moveDownCoroutine);
        moveDownCoroutine = StartCoroutine(MoveCoroutine(0f, 0.2f));
    }

    public IEnumerator MoveCoroutine(float end, float t)
    {
        float start = transform.localPosition.y;
        float elapsedT = 0f;
        while (elapsedT <= t)
        {
            Vector3 newPos = transform.localPosition;
            float sst = Mathf.Pow(Mathf.Min(1f, elapsedT / t), 2);
            newPos.y = Mathf.SmoothStep(start, end, sst);
            transform.localPosition = newPos;
            yield return new WaitForFixedUpdate();
            elapsedT += Time.fixedDeltaTime;
        }
        gameObjectToDrag.transform.localPosition = new Vector3(transform.localPosition.x, end, transform.localPosition.z);
    }
}
