using System.Collections;
using Managers;
using TMPro;
using UnityEngine;
using Utility;
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
            AudioManager.Instance.ChangeAudioSnapshot(AudioSnapshots.TRANSITION, 0.5f);
            AudioManager.Instance.Play2DSound(ENTER_OFFICE);
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
            yield return new WaitForSeconds(EditorialNewsLoader.loadDelay - 1.5f - fadeDuration);
            FadeTextOut();
            yield return new WaitForSeconds(fadeDuration);
            FadeIn();
            yield return new WaitForFixedUpdate();
            AnimatorClipInfo[] currentClipInfo = fadeOverlayAnimator.GetCurrentAnimatorClipInfo(0);
            StartCoroutine(PanCoroutine(0.7f, currentClipInfo[0].clip.length * 0.35f));
            yield return new WaitForSeconds(currentClipInfo[0].clip.length);
            fadeOverlay.gameObject.SetActive(false);
            fadeOverlayText.gameObject.SetActive(false);

            AudioManager.Instance.ChangeAudioSnapshot(AudioSnapshots.WORKSPACE, 1f);
            if (GameManager.Instance.roomToneAudioId == -1)
            {
                //GameManager.Instance.roomToneAudioId = AudioManager.Instance.Play3DLoopSound(ROOM_TONE, 10, 10000, new Vector2(7f, 20f), 5000);
            }

            if (GameManager.Instance.musicAudioId == -1)
            {
                GameManager.Instance.musicAudioId = AudioManager.Instance.Play2DLoopSound(MUSIC);    
            }
        }

        private IEnumerator PanCoroutine(float t, float delay)
        {
            Transform ct = Camera.main.transform;
            ct.position = new Vector3(CameraManager.MAX_X_POSITION_CAMERA, ct.position.y, ct.position.z);
            yield return new WaitForSeconds(delay);
            yield return TransformUtility.SetPositionCoroutine(ct, ct.position, new Vector3(CameraManager.MIN_X_POSITION_CAMERA, ct.position.y, ct.position.z), t);
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
