using System.Collections;
using Managers;
using TMPro;
using UnityEngine;
using Workspace.Editorial;

namespace Overlay
{
    public class RoundStartOverlay : MonoBehaviour
    {
        private const string ENTER_OFFICE = "Enter Office";
        private const string ROOM_TONE = "Room Tone";
        private const string MUSIC = "Music";
        
        [SerializeField] private GameObject fadeOverlay;
        [SerializeField] private GameObject fadeOverlayText;
        private Animator fadeOverlayAnimator;
        private Animator fadeOverlayTextAnimator;
        private Coroutine roundStartCoroutine;
        private void Start()
        {
            SoundManager.Instance.ChangeAudioMixerSnapshot(AudioMixerSnapshots.TRANSITION, 0.5f);
            SoundManager.Instance.Play2DSound(ENTER_OFFICE);
            fadeOverlayAnimator = fadeOverlay.GetComponent<Animator>();
            fadeOverlayTextAnimator = fadeOverlayText.GetComponent<Animator>();

            roundStartCoroutine = StartCoroutine(RoundStartCoroutine(1f));
        }

        private void OnGUI()
        {
            Event e = Event.current;
            if (!e.isKey || !(e.type == EventType.KeyDown)) return;
            if (!fadeOverlay.activeSelf) return;

            if (e.keyCode == KeyCode.S)
            {
                StopCoroutine(roundStartCoroutine);
                fadeOverlay.SetActive(false);
                fadeOverlayText.SetActive(false);
            }
        }

        private IEnumerator RoundStartCoroutine(float fadeDuration)
        {
            fadeOverlay.gameObject.SetActive(true);
            fadeOverlayText.gameObject.SetActive(true);
            UpdateText();
            FadeTextIn();
            yield return new WaitForSeconds(EditorialNewsLoader.loadDelay - 1f - fadeDuration);
            FadeTextOut();
            yield return new WaitForSeconds(fadeDuration);
            FadeIn();
            yield return new WaitForFixedUpdate();
            AnimatorClipInfo[] currentClipInfo = fadeOverlayAnimator.GetCurrentAnimatorClipInfo(0);
            yield return new WaitForSeconds(currentClipInfo[0].clip.length);
            fadeOverlay.gameObject.SetActive(false);
            fadeOverlayText.gameObject.SetActive(false);
            SoundManager.Instance.ChangeAudioMixerSnapshot(AudioMixerSnapshots.DEFAULT, 1f);
            SoundManager.Instance.Play3DLoopSound(ROOM_TONE, 10, 10000, new Vector2(7f, 0f));
            SoundManager.Instance.Play2DLoopSound(MUSIC);
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
}
