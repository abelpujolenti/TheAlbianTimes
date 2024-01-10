using Managers;

namespace Workspace.Mail.Content
{
    public class Bribe : EnvelopeContent
    {
        private float _totalMoney;
        
        protected override void ContentAction()
        {
            GameManager.Instance.UpdateStatsDisplayMoney(_totalMoney);
            Destroy(gameObject);
        }

        public void SetTotalMoney(float totalMoney)
        {
            _totalMoney = totalMoney;
        }

        public float GetTotalMoney()
        {
            return _totalMoney;
        }
    }
}
