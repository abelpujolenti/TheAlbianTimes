using System;
using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;
using Workspace.Mail.Content;

namespace Workspace.Mail
{
    public class Envelope : ThrowableInteractableRectTransform
    {
        private const String GRAB_ENVELOPE_SOUND = "Grab Envelope";
        private const String OPEN_ENVELOPE_SOUND = "Open Envelope";
        
        [SerializeField] private RectTransform _rectTransform;

        [SerializeField] private GameObject _envelopeContentGameObject;
        [SerializeField] private GameObject _envelopeCoverGameObject;

        private EnvelopeContentType _envelopeContentType;

        private EnvelopeContent _envelopeContent;

        private int _country;
        private int _jointId;

        private bool _hover;
        private bool _canHover = true;

        [SerializeField] private float openCoverTime = .2f;
        [SerializeField] private float closeCoverTime = .12f;

        private Coroutine _openCoverCoroutine;
        private Coroutine _closeCoverCoroutine;

        private AudioSource _audioSourceGrabEnvelope;
        private AudioSource _audioSourceOpenEnvelope;

        private void Start()
        {
            _audioSourceGrabEnvelope = gameObject.AddComponent<AudioSource>();
            _audioSourceOpenEnvelope = gameObject.AddComponent<AudioSource>();
            (AudioSource, String)[] tuples =
            {
                (_audioSourceGrabEnvelope, GRAB_ENVELOPE_SOUND),
                (_audioSourceOpenEnvelope, OPEN_ENVELOPE_SOUND)
            };
        }

        protected override void PointerEnter(BaseEventData data)
        {
            if (!_canHover)
            {
                return;
            }

            OpenCover();

            _hover = true;
        }

        protected override void PointerExit(BaseEventData data)
        {
            if (!_canHover)
            {
                return;
            }

            CloseCover();

            _hover = false;
        }

        protected override void PointerClick(BaseEventData data)
        {
            if (!_canHover)
            {
                return;
            }
            
            MailManager.Instance.SubtractJointId(_jointId);
            _envelopeContent.SetJointId(0);
            EventsManager.OnAddEnvelopeContent(gameObject, _envelopeContentGameObject);
            _envelopeContentGameObject.transform.position = transform.position;
            Destroy(gameObject);
        }

        protected override void BeginDrag(BaseEventData data)
        {
            base.BeginDrag(data);
            transform.SetAsLastSibling();
            CloseCover();
            _canHover = false;
        }

        protected override void EndDrag(BaseEventData data)
        {
            base.EndDrag(data);
            _canHover = true;
        }

        private void OpenCover()
        {
            if (_hover) return;
            if (_openCoverCoroutine != null) StopCoroutine(_openCoverCoroutine);
            if (_closeCoverCoroutine != null) StopCoroutine(_closeCoverCoroutine);
            _openCoverCoroutine = StartCoroutine(SetRotationCoroutine(_envelopeCoverGameObject.transform, 170f, openCoverTime));
        }

        private void CloseCover()
        {
            if (!_hover) return;
            if (_openCoverCoroutine != null) StopCoroutine(_openCoverCoroutine);
            if (_closeCoverCoroutine != null) StopCoroutine(_closeCoverCoroutine);
            _closeCoverCoroutine = StartCoroutine(SetRotationCoroutine(_envelopeCoverGameObject.transform, 0f, closeCoverTime));
        }

        private IEnumerator SetRotationCoroutine(Transform gameObjectToDrag, float xRotation, float t)
        {
            float elapsedT = 0f;
            Vector3 startRotation = gameObjectToDrag.transform.localRotation.eulerAngles * Mathf.Rad2Deg;
            while (elapsedT <= t)
            {
                float x = Mathf.LerpAngle(startRotation.x, xRotation, elapsedT / t);
                gameObjectToDrag.transform.localRotation = Quaternion.Euler(new Vector3(x, gameObjectToDrag.transform.localRotation.y, gameObjectToDrag.transform.localRotation.z));
                yield return new WaitForFixedUpdate();
                elapsedT += Time.fixedDeltaTime;
            }
            gameObjectToDrag.transform.localRotation = Quaternion.Euler(new Vector3(xRotation, gameObjectToDrag.transform.localRotation.y, gameObjectToDrag.transform.localRotation.z));
        }

        public void SetEnvelopeContent(GameObject envelopContentGameObject)
        {
            _envelopeContentGameObject = envelopContentGameObject;
            _envelopeContent = _envelopeContentGameObject.GetComponent<EnvelopeContent>();
        }

        public GameObject GetEnvelopeGameObject()
        {
            return _envelopeContentGameObject;
        }

        public void SetEnvelopeContentType(EnvelopeContentType mailContent)
        {
            _envelopeContentType = mailContent;
        }

        public EnvelopeContentType GetEnvelopeContentType()
        {
            return _envelopeContentType;
        }

        public void SetCountry(int country)
        {
            _country = country;
        }

        public int GetCountry()
        {
            return _country;
        }

        public void SetJointId(int jointId)
        {
            _jointId = jointId;
        }

        public int GetJointId()
        {
            return _jointId;
        }
    }
}
