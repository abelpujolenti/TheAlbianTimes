using Managers;

namespace Workspace.Mail.Content
{
    public class Bias : EnvelopeContent
    {
        private int _linkId;
        
        protected override void ContentAction()
        {
            if (EventsManager.OnLinkFouthBiasWithNewsHeadline == null)
            {
                return;
            }
            
            EventsManager.OnUseEnvelopeContent(gameObject);
            EventsManager.OnLinkFouthBiasWithNewsHeadline(_linkId);
            EditorialManager.Instance.SubtractLinkId(_linkId);
            Destroy(gameObject);
        }

        public void SetLinkId(int linkId)
        {
            _linkId = linkId;
        }

        public int GetLinkId()
        {
            return _linkId;
        }
    }
}
