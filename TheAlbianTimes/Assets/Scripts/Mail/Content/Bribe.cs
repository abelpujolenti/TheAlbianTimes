namespace Mail.Content
{
    public class Bribe : EnvelopeContent
    {

        private float _totalMoney;
        
        protected override void ContentAction()
        {
            //TODO Add to money currency
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
