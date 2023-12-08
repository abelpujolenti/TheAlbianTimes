namespace Mail.Content
{
    public class Letter : EnvelopeContent
    {
        private string _letterText;
        
        protected override void ContentAction()
        {
            //TODO Popup letter
        }

        public void SetLetterText(string letterText)
        {
            _letterText = letterText;
        }

        public string GetLetterText()
        {
            return _letterText;
        }
    }
}
