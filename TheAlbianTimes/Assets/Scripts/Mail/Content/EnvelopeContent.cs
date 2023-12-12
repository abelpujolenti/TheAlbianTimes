using UnityEngine.EventSystems;
using Utility;

namespace Mail.Content
{
    public abstract class EnvelopeContent : InteractableRectTransform      
    {
        private int _jointId;
        
        private EnvelopeContentType _envelopeContentType;
        
        protected override void BeginDrag(BaseEventData data)
        {
            
        }

        protected override void PointerClick(BaseEventData data)
        {
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
