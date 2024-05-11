using System;
using System.Collections;
using Countries;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;
using Workspace.Mail.Content;

namespace Workspace.Mail
{
    public class Envelope : InteractableRectTransform
    {
        private const float TIME_TO_SLIDE = 0.7f;
        
        private const String GRAB_ENVELOPE_SOUND = "Grab Envelope";
        private const String DROP_ENVELOPE_SOUND = "Drop Envelope";
        private const String OPEN_ENVELOPE_SOUND = "Open Envelope";

        [SerializeField] private RectTransform _rectTransform;

        [SerializeField] private GameObject _envelopeContentGameObject;
        [SerializeField] private GameObject _envelopeCoverGameObject;
        [SerializeField] private Image _sealIcon;

        private MailContainer _mailContainer;

        private EnvelopeContentType _envelopeContentType;

        private EnvelopeContent _envelopeContent;

        private int _country;
        private int _jointId;

        private bool _hover;
        private bool _canHover = true;

        private Vector3 _dragPosition;
        private Vector3 _size;

        [SerializeField] private float openCoverTime = .2f;
        [SerializeField] private float closeCoverTime = .12f;

        private Coroutine _openCoverCoroutine;
        private Coroutine _closeCoverCoroutine;

        private void Start()
        {
            string countryName = Country.names[_country];
            Sprite s = Resources.Load<Sprite>("Images/Icons/" + countryName);
            _sealIcon.sprite = s;
            
            Vector3 sizeDelta = _rectTransform.sizeDelta;
            Vector3 lossyScale = _rectTransform.lossyScale;
            _size = new Vector3(lossyScale.x * sizeDelta.x, lossyScale.y * sizeDelta.y, 0);
        }

        public void SetMailContainer(MailContainer mailContainer)
        {
            _mailContainer = mailContainer;
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

            Vector3 position = transform.position;
            AudioManager.Instance.Play3DSound(OPEN_ENVELOPE_SOUND, 5, 100, position);
            MailManager.Instance.SubtractJointId(_jointId);
            _envelopeContent.SetJointId(0);
            EventsManager.OnAddEnvelopeContent(gameObject, _envelopeContentGameObject);
            _envelopeContentGameObject.transform.position = position;
            Destroy(gameObject);
        }

        protected override void BeginDrag(BaseEventData data)
        {
            base.BeginDrag(data);
            AudioManager.Instance.Play3DSound(GRAB_ENVELOPE_SOUND, 5, 100, transform.position);
            transform.SetAsLastSibling();
            CloseCover();
            _canHover = false;
        }

        protected override void Drag(BaseEventData data)
        {
            base.Drag(data);
            Vector3 position = transform.position;
            Vector2[] corners = _mailContainer.GetCorners();

            float positionX = Mathf.Max(corners[0].x + _size.x / 2 , Mathf.Min(corners[1].x - _size.x / 2, position.x));
            float positionY = Mathf.Max(corners[0].y + _size.y / 2, Mathf.Min(corners[1].y - _size.y / 2, position.y));

            transform.position = new Vector3(positionX, positionY, position.z);

            //_dragPosition = transform.position;
        }

        protected override void EndDrag(BaseEventData data)
        {
            base.EndDrag(data);
            Vector3 position = transform.position;
            /*Vector3 directionVector = position - _dragPosition;
            Vector3 sizeDelta = _rectTransform.sizeDelta;
            Vector3 lossyScale = _rectTransform.lossyScale;
            Vector3 size = new Vector3(lossyScale.x * sizeDelta.x, lossyScale.y * sizeDelta.y, 0);
            bool positionToSlide = ;
            if (positionToSlide)
            {
                StartCoroutine(Slide(MailManager.Instance.DecideWhereToStay(position, directionVector, size)));    
            }*/
            
            AudioManager.Instance.Play3DSound(DROP_ENVELOPE_SOUND, 5, 100, position);
            _canHover = true;
        }

        /*private IEnumerator Slide(Vector3 destination)
        {
            float time = 0;
            Vector3 initialPosition = transform.position;

            while (time < TIME_TO_SLIDE)
            {
                time += Time.deltaTime;

                transform.position = Vector3.Lerp(initialPosition, destination, Mathf.Sqrt(time / TIME_TO_SLIDE));
                
                yield return null;
            }

            transform.position = destination;
        }*/

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
            Transform gameObjectToDragTransform = gameObjectToDrag.transform;
            Vector3 startRotation = gameObjectToDragTransform.localRotation.eulerAngles * Mathf.Rad2Deg;
            while (elapsedT <= t)
            {
                float x = Mathf.LerpAngle(startRotation.x, xRotation, elapsedT / t);
                gameObjectToDragTransform.localRotation = Quaternion.Euler(new Vector3(x, gameObjectToDragTransform.localRotation.y, gameObjectToDragTransform.localRotation.z));
                yield return new WaitForFixedUpdate();
                elapsedT += Time.fixedDeltaTime;
            }
            gameObjectToDragTransform.localRotation = Quaternion.Euler(new Vector3(xRotation, gameObjectToDragTransform.localRotation.y, gameObjectToDragTransform.localRotation.z));
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
