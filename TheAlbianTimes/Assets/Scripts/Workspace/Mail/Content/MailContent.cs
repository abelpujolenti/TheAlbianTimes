using System;
using UnityEngine;

namespace Workspace.Mail.Content
{
    public enum EnvelopeContentType
    {
        AD,
        BIAS,
        BRIBE,
        LETTER,
        ENUM_SIZE
    }

    public abstract class BaseMailContainer
    {
        public abstract string GetContainerPath();
        public abstract void SetContent(BaseMailContent[] baseContents);
        public abstract BaseMailContent[] GetContent();
        protected BaseMailContainer() {}
    }


    [Serializable]
    public class EnvelopesMailContainer : BaseMailContainer
    {
        public EnvelopeData[] contentEnvelopes = Array.Empty<EnvelopeData>();

        public override string GetContainerPath()
        {
            return "/Json/Mail/EnvelopesContainer.json";
        }

        public override void SetContent(BaseMailContent[] baseContents)
        {
            try
            {
                contentEnvelopes = (EnvelopeData[])baseContents;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public override BaseMailContent[] GetContent()
        {
            return contentEnvelopes;
        }
    }

    [Serializable]
    public class AdsMailContainer : BaseMailContainer
    {
        public MailContentAd[] contentAds = Array.Empty<MailContentAd>();

        public override string GetContainerPath()
        {
            return "/Json/Mail/AdsContainer.json";
        }

        public override void SetContent(BaseMailContent[] baseContents)
        {
            try
            {
                contentAds = (MailContentAd[])baseContents;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public override BaseMailContent[] GetContent()
        {
            return contentAds;
        }
    }

    [Serializable]
    public class BribesMailContainer : BaseMailContainer
    {
        public MailContentBribe[] contentBribes = Array.Empty<MailContentBribe>();

        public override string GetContainerPath()
        {
            return "/Json/Mail/BribesContainer.json";
        }

        public override void SetContent(BaseMailContent[] baseContents)
        {
            try
            {
                contentBribes = (MailContentBribe[])baseContents;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public override BaseMailContent[] GetContent()
        {
            return contentBribes;
        }
    }

    [Serializable]
    public class BiasesMailContainer : BaseMailContainer
    {
        public MailContentBias[] contentBiases = Array.Empty<MailContentBias>();

        public override string GetContainerPath()
        {
            return "/Json/Mail/BiasesContainer.json";
        }

        public override void SetContent(BaseMailContent[] baseContents)
        {
            try
            {
                contentBiases = (MailContentBias[])baseContents;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public override BaseMailContent[] GetContent()
        {
            return contentBiases;
        }
    }

    [Serializable]
    public class LettersMailContainer : BaseMailContainer
    {
        public MailContentLetter[] contentLetters = Array.Empty<MailContentLetter>();

        public override string GetContainerPath()
        {
            return "/Json/Mail/LettersContainer.json";
        }

        public override void SetContent(BaseMailContent[] baseContents)
        {
            try
            {
                contentLetters = (MailContentLetter[])baseContents;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public override BaseMailContent[] GetContent()
        {
            return contentLetters;
        }
    }
    
    
    public abstract class BaseMailContent
    {
        public int jointId;
        protected BaseMailContent() {}
    }

    [Serializable]
    public class EnvelopeData : BaseMailContent
    {
        public EnvelopeContentType envelopeContentType;
    }

    [Serializable]
    public class MailContentAd : BaseMailContent
    {
        
    }

    [Serializable]
    public class MailContentBribe : BaseMailContent
    {
        public float totalMoney;
    }

    [Serializable]
    public class MailContentBias : BaseMailContent
    {
        public int linkId;
    }

    [Serializable]
    public class MailContentLetter : BaseMailContent
    {
        public string letterText;
    }
}