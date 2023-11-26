using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    [SerializeField] Animator backgroundAnimator;
    [SerializeField] Animator buttonAnimator;
    private Coroutine startGameCoroutine;
    public void OnClick()
    {
        if (startGameCoroutine != null) return;
        startGameCoroutine = StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine()
    {
        backgroundAnimator.Play("Start", 0);
        buttonAnimator.Play("ButtonsFadeOut", 0);
        yield return new WaitForFixedUpdate();
        AnimatorClipInfo[] currentClipInfo = backgroundAnimator.GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(currentClipInfo[0].clip.length);
        yield return new WaitForSeconds(.2f);
        GameManager.Instance.sceneLoader.SetScene("WorkspaceScene");
    }
}
