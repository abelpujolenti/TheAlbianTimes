using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

class RotationCallback
{
    float value;
    Action func;
}

public class Notebook : InteractableRectTransform
{
    [SerializeField] private GameObject rightPage;
    [SerializeField] private GameObject leftPage;
    [SerializeField] private GameObject flipPage;
    [SerializeField] private float pageOpenTime = .4f;
    [SerializeField] private float pageCloseTime = .4f;
    [SerializeField] private float pageFlipTime = .3f;
    [SerializeField] private float pullUpBookTime = .5f;
    [SerializeField] private float pullDownBookTime = .5f;

    [SerializeField] private float autoCloseThreshold = -8f;
    private float boundX = 10f;

    private Vector3 initialPosition;

    private Coroutine flipCoroutine;

    Vector3 dragVector;

    private bool open = false;

    protected override void Setup()
    {
        base.Setup();
        initialPosition = transform.position;
    }

    protected override void BeginDrag(BaseEventData data)
    {
        base.BeginDrag(data);
    }

    protected override void Drag(BaseEventData data)
    {
        Vector3 prevPosition = transform.position;
        base.Drag(data);

        float x = Mathf.Max(-boundX, Mathf.Min(boundX, transform.position.x));
        float y = Mathf.Max(initialPosition.y, Mathf.Min(6.33f, transform.position.y));
        transform.position = new Vector3(x, y, transform.position.z);

        Vector3 currPosition = transform.position;
        dragVector = currPosition - prevPosition;

    }

    protected override void EndDrag(BaseEventData data)
    {
        base.EndDrag(data);
        if (open && (transform.position.y <= autoCloseThreshold || transform.position.y <= autoCloseThreshold * 0.5f && dragVector.y < -0.04f))
        {
            Close();
        }
        else if (!open)
        {
            if (transform.position.y > autoCloseThreshold) Open(false);
            else StartCoroutine(MoveDownCoroutine(pullDownBookTime * (Vector2.Distance(transform.position, initialPosition) / 7f)));
        }
    }

    protected override void PointerClick(BaseEventData data)
    {
        base.PointerClick(data);
        if (held) return;
        Open(true);
    }

    public void Open(bool move)
    {
        if (open) return;
        StartCoroutine(OpenCoroutine(move));
    }

    private IEnumerator OpenCoroutine(bool move)
    {
        EnableCover();
        if (move)
        {
            yield return MoveUpCoroutine(pullUpBookTime);
        }
        yield return StartCoroutine(RotatePageCoroutine((RectTransform)leftPage.transform, pageOpenTime, 179.9f, 0f, 0.5f, DisableCover));
        open = true;
    }

    public void Close()
    {
        if (!open) return;
        if (flipCoroutine != null) StopCoroutine(flipCoroutine);
        StartCoroutine(CloseCoroutine());
    }

    private IEnumerator CloseCoroutine()
    {
        float speed = Mathf.Max(0.4f, (Vector2.Distance(transform.position, initialPosition) / 8f));
        DisableCover();
        StartCoroutine(RotatePageCoroutine((RectTransform)leftPage.transform, pageCloseTime * speed, 0.01f, 180f, 0.5f, EnableCover));
        yield return MoveDownCoroutine(pullDownBookTime * speed);
        open = false;
    }

    private IEnumerator MoveUpCoroutine(float t)
    {
        yield return StartCoroutine(TransformUtility.SetPositionCoroutine(transform, transform.position, new Vector3(transform.position.x, -.5f, transform.position.z), pullUpBookTime));
    }

    private IEnumerator MoveDownCoroutine(float t)
    {
        yield return StartCoroutine(TransformUtility.SetPositionCoroutine(transform, transform.position, initialPosition, t));
    }

    public void FlipPage()
    {
        flipCoroutine = StartCoroutine(FlipPageCoroutine());
    }

    private IEnumerator FlipPageCoroutine()
    {
        flipPage.SetActive(true);

        Image flipImage = flipPage.transform.Find("Background").GetComponent<Image>();
        float imageBrightness = ColorUtil.GetBrightness(flipImage.color);
        yield return StartCoroutine(RotatePageCoroutine((RectTransform)flipPage.transform, pageFlipTime / 2f, 0f, 90f, 0.5f));
        StartCoroutine(ShadePageCoroutine(flipImage, pageFlipTime / 2f, 0.3f, imageBrightness, 2f));
        yield return StartCoroutine(RotatePageCoroutine((RectTransform)flipPage.transform, pageFlipTime / 2f, 90f, 180f, 2f));
        flipPage.SetActive(false);
    }

    private void EnableCover()
    {
        leftPage.transform.Find("Background").GetComponent<Image>().color = new Color(.5f, .25f, .3f);
    }

    private void DisableCover()
    {
        leftPage.transform.Find("Background").GetComponent<Image>().color = new Color(.95f, .95f, .93f);
    }

    private IEnumerator ShadePageCoroutine(Image image, float t, float start, float end, float exp = 0.5f)
    {
        image.color = ColorUtil.SetBrightness(image.color, start);
        float elapsedT = 0f;
        while (elapsedT <= t)
        {
            float b = Mathf.Lerp(start, end, Mathf.Pow((elapsedT / t), exp));
            image.color = ColorUtil.SetBrightness(image.color, b);

            yield return new WaitForFixedUpdate();
            elapsedT += Time.fixedDeltaTime;
        }
        image.color = ColorUtil.SetBrightness(image.color, end);
    }

    private IEnumerator RotatePageCoroutine(RectTransform rt, float t, float start, float end, float exp = 0.5f, Action midCallback = null)
    {
        rt.rotation = Quaternion.Euler(new Vector3(0f, start, 0f));
        float elapsedT = 0f;
        float absDiff = Mathf.Abs(end - start);
        bool callbackActivated = false;
        while (elapsedT <= t)
        {
            float yRotation = Mathf.LerpAngle(start, end, Mathf.Pow((elapsedT / t), exp));
            rt.rotation = Quaternion.Euler(new Vector3(0f, yRotation, 0f));

            if (!callbackActivated && midCallback != null && Mathf.Abs(yRotation - start) > absDiff / 2f)
            {
                midCallback();
                callbackActivated = true;
            }

            yield return new WaitForFixedUpdate();
            elapsedT += Time.fixedDeltaTime;
        }
        rt.rotation = Quaternion.Euler(new Vector3(0f, end, 0f));
    }
}
