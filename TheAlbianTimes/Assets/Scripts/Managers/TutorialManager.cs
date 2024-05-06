using System.Collections;
using UnityEngine;
using Utility;
using Workspace.Editorial;

namespace Managers
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] GameObject notebook;
        [SerializeField] GameObject mail;
        [SerializeField] GameObject statOverlay;
        [SerializeField] GameObject folder;
        [SerializeField] private GameObject _biasContainer;
        [SerializeField] private Transform _cameraTransform;
        
        void Start()
        {
            switch (GameManager.Instance.GetRound())
            {
                case 0:
                    notebook.SetActive(false);
                    mail.SetActive(false);
                    statOverlay.SetActive(false);
                    RoundZero();
                    break;
                case 1:
                    statOverlay.SetActive(false);
                    StartCoroutine(LowerStatOverlay());
                    notebook.SetActive(false);
                    mail.SetActive(false);
                    StartCoroutine(RevealMail());
                    break;
                case 2:
                    notebook.SetActive(false);
                    StartCoroutine(RaiseNotebook());
                    break;
                default:
                    break;
            }
        }

        private void RoundZero()
        {
            EventsManager.OnChangeChosenBiasIndex += LockBiases;
        }

        private void LockBiases()
        {
            if (GameManager.Instance.GetRound() != 0) return;
            _biasContainer.SetActive(false);
            StartCoroutine(CameraNudgeCoroutine(1.3f, 1f, 2f));
            EventsManager.OnChangeChosenBiasIndex -= LockBiases;
        }

        private IEnumerator CameraNudgeCoroutine(float offset, float t, float delay)
        {
            Vector3 cameraPosition = _cameraTransform.position;
            yield return new WaitForSeconds(delay);
            yield return TransformUtility.SetPositionCoroutine(_cameraTransform, cameraPosition, cameraPosition + new Vector3(offset, 0f, 0f), t/2f);
            yield return TransformUtility.SetPositionCoroutine(_cameraTransform, _cameraTransform.position, _cameraTransform.position + new Vector3(-offset, 0f, 0f), t/2f);
        }

        private IEnumerator RevealMail()
        {
            yield return new WaitForSeconds(EditorialNewsLoader.loadDelay + 1.6f);
            mail.SetActive(true);
            Transform mt = mail.transform;
            yield return TransformUtility.SetPositionCoroutine(mt, mt.position + new Vector3(-1f, 0f, 0f), mt.position, 1f);
        }

        private IEnumerator RaiseNotebook()
        {
            yield return new WaitForSeconds(EditorialNewsLoader.loadDelay + 1f);
            notebook.SetActive(true);
            Transform nt = notebook.transform;
            yield return TransformUtility.SetPositionCoroutine(nt, nt.position + new Vector3(0f, -1f, 0f), nt.position + new Vector3(0f, 1f, 0f), .5f);
            yield return TransformUtility.SetPositionCoroutine(nt, nt.position, nt.position + new Vector3(0f, -1f, 0f), .6f);
        }

        private IEnumerator LowerStatOverlay()
        {
            yield return new WaitForSeconds(EditorialNewsLoader.loadDelay + 1f);
            statOverlay.SetActive(true);
            Transform st = statOverlay.transform;
            yield return TransformUtility.SetPositionCoroutine(st, st.position + new Vector3(0f, 4f, 0f), st.position, .75f);
        }

        private void OnDestroy()
        {
            EventsManager.OnChangeChosenBiasIndex -= LockBiases;
        }
    }
}
