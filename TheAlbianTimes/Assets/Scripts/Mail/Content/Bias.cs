using Managers;

namespace Mail.Content
{
    public class Bias : EnvelopeContent
    {
        private int _linkId;
        
        protected override void ContentAction()
        {
            EventsManager.OnLinkFouthBiasWithNewsHeadline(_linkId);
            EditorialManager.Instance.SubtractLinkId(_linkId);
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
