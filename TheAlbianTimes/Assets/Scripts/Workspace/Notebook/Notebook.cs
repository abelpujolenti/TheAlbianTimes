using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
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

    private Coroutine flipCoroutine;

    protected override void PointerClick(BaseEventData data)
    {
        base.PointerClick(data);
        Open();
    }

    private void OnGUI()
    {
        Event e = Event.current;
        if (!e.isKey || !(e.type == EventType.KeyDown)) return;
        if (!gameObject.activeSelf) return;

        if (e.keyCode == KeyCode.F)
        {
            FlipPage();
        }
        if (e.keyCode == KeyCode.C)
        {
            Close();
        }
    }

    public void Open()
    {
        StartCoroutine(OpenCoroutine());
    }

    private IEnumerator OpenCoroutine()
    {
        EnableCover();
        yield return StartCoroutine(TransformUtility.SetPositionCoroutine(transform, transform.position, new Vector3(transform.position.x, 0f, transform.position.z), pullUpBookTime));
        yield return StartCoroutine(RotatePageCoroutine((RectTransform)leftPage.transform, pageOpenTime, 179.9f, 0f, 0.5f, DisableCover));
    }

    public void Close()
    {
        if (flipCoroutine != null) StopCoroutine(flipCoroutine);
        StartCoroutine(CloseCoroutine());
    }

    private IEnumerator CloseCoroutine()
    {
        DisableCover();
        yield return StartCoroutine(RotatePageCoroutine((RectTransform)leftPage.transform, pageOpenTime, 0.01f, 180f, 2f));
        EnableCover();
        yield return StartCoroutine(RotatePageCoroutine((RectTransform)leftPage.transform, pageOpenTime, 0.01f, 180f, 0.5f));
        yield return StartCoroutine(TransformUtility.SetPositionCoroutine(transform, transform.position, new Vector3(transform.position.x, -8.5f, transform.position.z), pullDownBookTime));
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
                Debug.Log(yRotation + ">" + absDiff + "/2");
                midCallback();
                callbackActivated = true;
            }

            yield return new WaitForFixedUpdate();
            elapsedT += Time.fixedDeltaTime;
        }
        rt.rotation = Quaternion.Euler(new Vector3(0f, end, 0f));
    }
}
