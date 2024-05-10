using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Workspace.Mail.Content
{
    public abstract class EnvelopeContent : InteractableRectTransform
    {
        private const float TIME_TO_SLIDE = 0.7f;

        [SerializeField] private RectTransform _rectTransform;

        private MailContainer _mailContainer;

        private int _country;
        private int _jointId;
        
        private EnvelopeContentType _envelopeContentType;

        private Vector3 _dragPosition;
        private Vector3 _size;

        private bool _dragged;

        private void Start()
        {
            Vector3 sizeDelta = _rectTransform.sizeDelta;
            Vector3 lossyScale = _rectTransform.lossyScale;
            _size = new Vector3(lossyScale.x * sizeDelta.x, lossyScale.y * sizeDelta.y, 0);
        }

        public void SetMailContainer(MailContainer mailContainer)
        {
            _mailContainer = mailContainer;
        }

        protected override void BeginDrag(BaseEventData data)
        {
            _dragged = true;
        }

        protected override void Drag(BaseEventData data)
        {
            base.Drag(data);
            //_dragPosition = transform.position;
            Vector3 position = transform.position;
            Vector2[] corners = _mailContainer.GetCorners();

            float positionX = Mathf.Max(corners[0].x + _size.x / 2 , Mathf.Min(corners[1].x - _size.x / 2, position.x));
            float positionY = Mathf.Max(corners[0].y + _size.y / 2, Mathf.Min(corners[1].y - _size.y / 2, position.y));

            transform.position = new Vector3(positionX, positionY, position.z);
        }

        /*protected override void EndDrag(BaseEventData data)
        {
            base.EndDrag(data);
            Vector3 position = transform.position;
            Vector3 directionVector = position - _dragPosition;
            Vector3 sizeDelta = _rectTransform.sizeDelta;
            Vector3 lossyScale = _rectTransform.lossyScale;
            Vector3 size = new Vector3(lossyScale.x * sizeDelta.x, lossyScale.y * sizeDelta.y, 0);
            StartCoroutine(Slide(MailManager.Instance.DecideWhereToStay(position, directionVector, size)));
        }

        private IEnumerator Slide(Vector3 destination)
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

        protected override void PointerClick(BaseEventData data)
        {
            if (_dragged)
            {
                _dragged = false;
                return;
            }

            ContentAction();
        }

        protected abstract void ContentAction();

        public void SetEnvelopeContentType(EnvelopeContentType envelopeContentType)
        {
            _envelopeContentType = envelopeContentType;
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
