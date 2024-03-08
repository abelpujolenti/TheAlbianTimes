using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [SerializeField] Image fadeImage;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float initialDelay = 0.2f;
    void Start()
    {
        fadeImage.gameObject.SetActive(true);
        StartCoroutine(FadeCoroutine(duration));
    }

    private IEnumerator FadeCoroutine(float t)
    {
        fadeImage.color = new Color(0f, 0f, 0f, 1f);

        yield return new WaitForSeconds(initialDelay);

        float elapsedT = 0f;
        while (elapsedT <= t)
        {
            fadeImage.color = ColorUtil.Alpha(Color.black, 1f - Mathf.Pow(elapsedT / t, 2f));

            yield return new WaitForFixedUpdate();
            elapsedT += Time.fixedDeltaTime;
        }

        fadeImage.gameObject.SetActive(false);
    }
}
