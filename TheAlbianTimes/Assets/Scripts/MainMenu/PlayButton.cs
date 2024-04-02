using System;
using System.Collections;
using Managers;
using UnityEngine;

namespace MainMenu
{
    public class PlayButton : MonoBehaviour
    {
        private const String INTRO_SOUND = "Intro";
        private const String CLICK_BUTTON_SOUND = "Click Button";
        
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
            SoundManager.Instance.Play2DSound(CLICK_BUTTON_SOUND);
            backgroundAnimator.Play("Start", 0);
            buttonAnimator.Play("ButtonsFadeOut", 0);
            yield return new WaitForFixedUpdate();
            SoundManager.Instance.Play2DSound(INTRO_SOUND);
            AnimatorClipInfo[] currentClipInfo = backgroundAnimator.GetCurrentAnimatorClipInfo(0);
            yield return new WaitForSeconds(currentClipInfo[0].clip.length);
            yield return new WaitForSeconds(2.5f);
            GameManager.Instance.sceneLoader.SetScene("WorkspaceScene");
        }
    }
}