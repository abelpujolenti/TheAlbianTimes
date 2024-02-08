using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Notebook : MonoBehaviour
{
    [SerializeField] private GameObject rightPage;
    [SerializeField] private GameObject leftPage;
    [SerializeField] private GameObject flipPage;
    [SerializeField] private float pageOpenTime = .4f;
    [SerializeField] private float pageFlipTime = .3f;

    private void OnEnable()
    {
        Open();
    }

    public void Open()
    {
        StartCoroutine(RotatePageCoroutine((RectTransform)rightPage.transform, pageOpenTime, -90f, 0));
        StartCoroutine(RotatePageCoroutine((RectTransform)leftPage.transform, pageOpenTime, 90f, 0));

        FlipPage();
    }

    public void FlipPage()
    {
        StartCoroutine(FlipPageCoroutine());
    }

    private IEnumerator FlipPageCoroutine()
    {
        yield return new WaitForSeconds(2f);
        flipPage.SetActive(true);

        Image flipImage = flipPage.transform.Find("Background").GetComponent<Image>();
        float imageBrightness = ColorUtil.GetBrightness(flipImage.color);
        yield return StartCoroutine(RotatePageCoroutine((RectTransform)flipPage.transform, pageFlipTime, 0f, 179.9f));
        flipPage.SetActive(false);
    }

    private IEnumerator ShadePageCoroutine(Image image, float t, float start, float end)
    {
        image.color = ColorUtil.SetBrightness(image.color, start);
        float elapsedT = 0f;
        while (elapsedT <= t)
        {
            float b = Mathf.Lerp(start, end, elapsedT / t);
            image.color = ColorUtil.SetBrightness(image.color, b);

            yield return new WaitForFixedUpdate();
            elapsedT += Time.fixedDeltaTime;
        }
        image.color = ColorUtil.SetBrightness(image.color, end);
    }

    private IEnumerator RotatePageCoroutine(RectTransform rt, float t, float start, float end)
    {
        rt.rotation = Quaternion.Euler(new Vector3(0f, start, 0f));
        float elapsedT = 0f;
        while (elapsedT <= t)
        {
            float yRotation = Mathf.LerpAngle(start, end, Mathf.Pow((elapsedT / t), .5f));
            rt.rotation = Quaternion.Euler(new Vector3(0f, yRotation, 0f));

            yield return new WaitForFixedUpdate();
            elapsedT += Time.fixedDeltaTime;
        }
        rt.rotation = Quaternion.Euler(new Vector3(0f, end, 0f));
    }
}
