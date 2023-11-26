using Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundStartOverlay : MonoBehaviour
{
    [SerializeField] private GameObject fadeOverlay;
    [SerializeField] private GameObject fadeOverlayText;
    private Animator fadeOverlayAnimator;
    private Animator fadeOverlayTextAnimator;
    private void Start()
    {
        fadeOverlayAnimator = fadeOverlay.GetComponent<Animator>();
        fadeOverlayTextAnimator = fadeOverlayText.GetComponent<Animator>();

        StartCoroutine(RoundStartCoroutine());
    }

    private IEnumerator RoundStartCoroutine()
    {
        fadeOverlay.gameObject.SetActive(true);
        fadeOverlayText.gameObject.SetActive(true);
        UpdateText();
        FadeTextIn();
        yield return new WaitForSeconds(4);
        FadeTextOut();
        yield return new WaitForSeconds(1);
        FadeIn();
        yield return new WaitForFixedUpdate();
        AnimatorClipInfo[] currentClipInfo = fadeOverlayAnimator.GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(currentClipInfo[0].clip.length);
        fadeOverlay.gameObject.SetActive(false);
        fadeOverlayText.gameObject.SetActive(false);
    }

    private void UpdateText()
    {
        fadeOverlayText.GetComponent<TextMeshProUGUI>().text = "Day " + GameManager.Instance.GetRound();
    }

    private void FadeTextIn()
    {
        fadeOverlayTextAnimator.Play("WorkspaceOverlayTextFadeIn");
    }

    private void FadeTextOut()
    {
        fadeOverlayTextAnimator.Play("WorkspaceOverlayTextFadeOut");
    }

    private void FadeIn()
    {
        fadeOverlayAnimator.Play("WorkspaceOverlayFadeIn");
    }
}
