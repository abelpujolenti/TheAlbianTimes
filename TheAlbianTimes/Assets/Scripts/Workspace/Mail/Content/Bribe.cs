using System;
using Managers;

namespace Workspace.Mail.Content
{
    public class Bribe : EnvelopeContent
    {
        private const string COINS_SOUND = "Coins";
        private const int TOTAL_COINS_SOUNDS = 5;

        private string[] _coinsSounds;
        
        private float _totalMoney;

        private void Start()
        {
            _coinsSounds = new string[TOTAL_COINS_SOUNDS];
            
            for (int i = 0; i < TOTAL_COINS_SOUNDS; i++)
            {
                _coinsSounds[i] = COINS_SOUND + i;
            }
        }

        protected override void ContentAction()
        {
            EventsManager.OnUseEnvelopeContent(gameObject);
            GameManager.Instance.UpdateStatsDisplayMoney(_totalMoney);
            AudioManager.Instance.Play3DRandomSound(_coinsSounds, 5, 100, 1, 1, 0.7f, 1f, transform.position);
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
