using System;

namespace Mail.Content
{
    public enum EnvelopeContentType
    {
        AD,
        BIAS,
        BRIBE,
        LETTER,
        AMOUNT
    }

    public abstract class BaseContainer
    {
        public abstract string GetContainerPath();
        public abstract void SetContent(BaseContent[] baseContents);
        public abstract BaseContent[] GetContent();
        protected BaseContainer() {}
    }


    [Serializable]
    public class EnvelopesContainer : BaseContainer
    {
        public EnvelopeData[] contentEnvelopes;

        public override string GetContainerPath()
        {
            return "/Json/Mail/EnvelopesContainer.json";
        }

        public override void SetContent(BaseContent[] baseContents)
        {
            contentEnvelopes = (EnvelopeData[])baseContents;
        }

        public override BaseContent[] GetContent()
        {
            return contentEnvelopes;
        }
    }

    [Serializable]
    public class AdsContainer : BaseContainer
    {
        public ContentAd[] contentAds;

        public override string GetContainerPath()
        {
            return "/Json/Mail/AdsContainer.json";
        }

        public override void SetContent(BaseContent[] baseContents)
        {
            contentAds = (ContentAd[])baseContents;
        }

        public override BaseContent[] GetContent()
        {
            return contentAds;
        }
    }

    [Serializable]
    public class BribesContainer : BaseContainer
    {
        public ContentBribe[] contentBribes;

        public override string GetContainerPath()
        {
            return "/Json/Mail/BribesContainer.json";
        }

        public override void SetContent(BaseContent[] baseContents)
        {
            contentBribes = (ContentBribe[])baseContents;
        }

        public override BaseContent[] GetContent()
        {
            return contentBribes;
        }
    }

    [Serializable]
    public class BiasesContainer : BaseContainer
    {
        public ContentBias[] contentBiases;

        public override string GetContainerPath()
        {
            return "/Json/Mail/BiasesContainer.json";
        }

        public override void SetContent(BaseContent[] baseContents)
        {
            contentBiases = (ContentBias[])baseContents;
        }

        public override BaseContent[] GetContent()
        {
            return contentBiases;
        }
    }

    [Serializable]
    public class LettersContainer : BaseContainer
    {
        public ContentLetter[] contentLetters;

        public override string GetContainerPath()
        {
            return "/Json/Mail/LettersContainer.json";
        }

        public override void SetContent(BaseContent[] baseContents)
        {
            contentLetters = (ContentLetter[])baseContents;
        }

        public override BaseContent[] GetContent()
        {
            return contentLetters;
        }
    }
    
    
    public abstract class BaseContent
    {
        public int jointId;
        protected BaseContent() {}
    }

    [Serializable]
    public class EnvelopeData : BaseContent
    {
        public EnvelopeContentType envelopeContentType;
    }

    [Serializable]
    public class ContentAd : BaseContent
    {
        
    }

    [Serializable]
    public class ContentBribe : BaseContent
    {
        public float totalMoney;
    }

    [Serializable]
    public class ContentBias : BaseContent
    {
        
    }

    [Serializable]
    public class ContentLetter : BaseContent
    {
        public string letterText;
    }
}