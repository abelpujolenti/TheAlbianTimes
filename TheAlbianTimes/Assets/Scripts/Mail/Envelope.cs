using System;
using Mail.Content;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Mail
{
    public class Envelope : InteractableRectTransform
    {
        private const String HOVER_CONDITION = "Hover";

        [SerializeField] private RectTransform _rectTransform;

        [SerializeField] private Animator _animator;

        [SerializeField] private GameObject _envelopeContentGameObject;

        private EnvelopeContentType _envelopeContentType;

        private EnvelopeContent _envelopeContent;

        private int _jointId;

        private bool _hover;
        private bool _canHover = true;

        protected override void PointerEnter(BaseEventData data)
        {
            if (!_canHover)
            {
                return;
            }
            
            _hover = true;
            _animator.SetBool(HOVER_CONDITION, _hover);
            //_rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, 30);
        }

        protected override void PointerExit(BaseEventData data)
        {
            if (!_canHover)
            {
                return;
            }
            
            _hover = false;
            _animator.SetBool(HOVER_CONDITION, _hover);
            //_rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, 20);
        }

        protected override void PointerClick(BaseEventData data)
        {
            Debug.Log("Click");
            MailManager.Instance.SubtractJointId(_jointId);
            _envelopeContent.SetJointId(0);
            EventsManager.OnAddEnvelopeContent(gameObject, _envelopeContentGameObject);
            _envelopeContentGameObject.transform.position = transform.position;
            Destroy(gameObject);
        }

        protected override void BeginDrag(BaseEventData data)
        {
            //base.BeginDrag(data);
            _animator.SetBool(HOVER_CONDITION, false);
            _canHover = false;
        }

        protected override void EndDrag(BaseEventData data)
        {
            base.EndDrag(data);
            _canHover = true;
        }

        public void SetAnimatorController(RuntimeAnimatorController animatorController)
        {
            _animator.runtimeAnimatorController = animatorController;
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
