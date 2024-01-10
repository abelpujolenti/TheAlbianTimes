using Managers;

namespace Workspace.Mail.Content
{
    public class Letter : EnvelopeContent
    {
        private string _letterText;
        
        protected override void ContentAction()
        {
            EventsManager.OnClickLetter(_letterText);
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
