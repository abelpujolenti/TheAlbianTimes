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
        public EnvelopeData[] contentEnvelopes = Array.Empty<EnvelopeData>();

        public override string GetContainerPath()
        {
            return "/Json/Mail/EnvelopesContainer.json";
        }

        public override void SetContent(BaseContent[] baseContents)
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

        public override BaseContent[] GetContent()
        {
            return contentEnvelopes;
        }
    }

    [Serializable]
    public class AdsContainer : BaseContainer
    {
        public ContentAd[] contentAds = Array.Empty<ContentAd>();

        public override string GetContainerPath()
        {
            return "/Json/Mail/AdsContainer.json";
        }

        public override void SetContent(BaseContent[] baseContents)
        {
            try
            {
                contentAds = (ContentAd[])baseContents;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public override BaseContent[] GetContent()
        {
            return contentAds;
        }
    }

    [Serializable]
    public class BribesContainer : BaseContainer
    {
        public ContentBribe[] contentBribes = Array.Empty<ContentBribe>();

        public override string GetContainerPath()
        {
            return "/Json/Mail/BribesContainer.json";
        }

        public override void SetContent(BaseContent[] baseContents)
        {
            try
            {
                contentBribes = (ContentBribe[])baseContents;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public override BaseContent[] GetContent()
        {
            return contentBribes;
        }
    }

    [Serializable]
    public class BiasesContainer : BaseContainer
    {
        public ContentBias[] contentBiases = Array.Empty<ContentBias>();

        public override string GetContainerPath()
        {
            return "/Json/Mail/BiasesContainer.json";
        }

        public override void SetContent(BaseContent[] baseContents)
        {
            try
            {
                contentBiases = (ContentBias[])baseContents;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public override BaseContent[] GetContent()
        {
            return contentBiases;
        }
    }

    [Serializable]
    public class LettersContainer : BaseContainer
    {
        public ContentLetter[] contentLetters = Array.Empty<ContentLetter>();

        public override string GetContainerPath()
        {
            return "/Json/Mail/LettersContainer.json";
        }

        public override void SetContent(BaseContent[] baseContents)
        {
            try
            {
                contentLetters = (ContentLetter[])baseContents;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
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
        public int linkId;
    }

    [Serializable]
    public class ContentLetter : BaseContent
    {
        public string letterText;
    }
}