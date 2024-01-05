using System;
using System.Collections;
using Managers;
using UnityEngine;

namespace MainMenu
{
    public class PlayButton : MonoBehaviour
    {
        private const String INTRO_SOUND = "Intro";
        private const String CHANGE_BIAS_SOUND = "Change Bias";
        
        [SerializeField] Animator backgroundAnimator;
        [SerializeField] Animator buttonAnimator;
        private Coroutine startGameCoroutine;

        private AudioSource _audioSourceIntro;
        private AudioSource _audioSourceChangeBias;
        
        public void OnClick()
        {
            _audioSourceIntro = gameObject.AddComponent<AudioSource>();
            SoundManager.Instance.SetAudioSourceComponent(_audioSourceIntro, INTRO_SOUND);
            _audioSourceChangeBias = gameObject.AddComponent<AudioSource>();
            SoundManager.Instance.SetAudioSourceComponent(_audioSourceChangeBias, CHANGE_BIAS_SOUND);
            if (startGameCoroutine != null) return;
            startGameCoroutine = StartCoroutine(StartGameCoroutine());
        }

        private IEnumerator StartGameCoroutine()
        {
            _audioSourceChangeBias.Play();
            backgroundAnimator.Play("Start", 0);
            buttonAnimator.Play("ButtonsFadeOut", 0);
            yield return new WaitForFixedUpdate();
            _audioSourceIntro.Play();
            AnimatorClipInfo[] currentClipInfo = backgroundAnimator.GetCurrentAnimatorClipInfo(0);
            yield return new WaitForSeconds(currentClipInfo[0].clip.length);
            yield return new WaitForSeconds(2.5f);
            GameManager.Instance.sceneLoader.SetScene("WorkspaceScene");
        }
    }
}
