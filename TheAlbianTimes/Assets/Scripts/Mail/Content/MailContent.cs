using System;

namespace Mail.Content
{
    [Serializable]
    public struct EnvelopesContainer
    {
        public ContentEnvelope[] contentEnvelopes;
    }

    [Serializable]
    public struct ContentEnvelope
    {
        public int jointId;
        public EnvelopeContentType envelopeContentType;
    }

    public enum EnvelopeContentType
    {
        AD,
        BIAS,
        BRIBE,
        LETTER
    }

    [Serializable]
    public struct AdsContainer
    {
        public ContentAd[] contentAds;
    }

    [Serializable]
    public struct ContentAd
    {
        public int jointId;
    }

    [Serializable]
    public struct BribesContainer
    {
        public ContentBribe[] contentBribes;
    }

    [Serializable]
    public struct ContentBribe
    {
        public int jointId;
        public float totalMoney;
    }

    [Serializable]
    public struct BiasesContainer
    {
        public ContentBias[] contentBiases;
    }

    [Serializable]
    public struct ContentBias
    {
        public int jointId;
    }

    [Serializable]
    public struct LettersContainer
    {
        public ContentLetter[] contentLetters;
    }

    [Serializable]
    public struct ContentLetter
    {
        public int jointId;
        public string letterText;
    }

}