using System;

namespace Mail.Content
{
    public enum EnvelopeContentType
    {
        AD,
        BIAS,
        BRIBE,
        LETTER
    }

    public abstract class BaseContainer
    {
        public abstract BaseContent[] GetContent();
        public abstract string GetContainerPath();

        protected BaseContainer() {}
    }


    [Serializable]
    public class EnvelopesContainer : BaseContainer
    {
        public EnvelopeData[] contentEnvelopes;
        public override BaseContent[] GetContent()
        {
            return contentEnvelopes;
        }

        public override string GetContainerPath()
        {
            return "/Json/Mail/EnvelopesContainer.json";
        }
    }

    [Serializable]
    public class AdsContainer : BaseContainer
    {
        public ContentAd[] contentAds;
        public override BaseContent[] GetContent()
        {
            return contentAds;
        }

        public override string GetContainerPath()
        {
            return "/Json/Mail/AdsContainer.json";
        }
    }

    [Serializable]
    public class BribesContainer : BaseContainer
    {
        public ContentBribe[] contentBribes;
        public override BaseContent[] GetContent()
        {
            return contentBribes;
        }

        public override string GetContainerPath()
        {
            return "/Json/Mail/BribesContainer.json";
        }
    }

    [Serializable]
    public class BiasesContainer : BaseContainer
    {
        public ContentBias[] contentBiases;
        public override BaseContent[] GetContent()
        {
            return contentBiases;
        }

        public override string GetContainerPath()
        {
            return "/Json/Mail/BiasesContainer.json";
        }
    }

    [Serializable]
    public class LettersContainer : BaseContainer
    {
        public ContentLetter[] contentLetters;
        public override BaseContent[] GetContent()
        {
            return contentLetters;
        }

        public override string GetContainerPath()
        {
            return "/Json/Mail/LettersContainer.json";
        }
    }
    
    
    public abstract class BaseContent
    {
        public int jointId;
        public abstract String GetPath();

        protected BaseContent() {}
    }

    [Serializable]
    public class EnvelopeData : BaseContent
    {
        public EnvelopeContentType envelopeContentType;
        public override string GetPath()
        {
            return "/Json/Mail/EnvelopesContainer.json";
        }
    }

    [Serializable]
    public class ContentAd : BaseContent
    {
        public override string GetPath()
        {
            return "/Json/Mail/AdsContainer.json";
        }
    }

    [Serializable]
    public class ContentBribe : BaseContent
    {
        public float totalMoney;
        public override string GetPath()
        {
            return "/Json/Mail/BribesContainer.json";
        }
    }

    [Serializable]
    public class ContentBias : BaseContent
    {
        public override string GetPath()
        {
            return "/Json/Mail/BiasesContainer.json";
        }
    }

    [Serializable]
    public class ContentLetter : BaseContent
    {
        public string letterText;
        public override string GetPath()
        {
            return "/Json/Mail/LettersContainer.json";
        }
    }

}