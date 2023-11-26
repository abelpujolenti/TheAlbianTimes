namespace Mail.Content
{
    public struct EnvelopesContainer
    {
        public ContentEnvelope[] contentEnvelopes;
    }

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

    public struct AdsContainer
    {
        public ContentAd[] contentAds;
    }

    public struct ContentAd
    {
        public int jointId;
    }

    public struct BribesContainer
    {
        public ContentBribe[] contentBribes;
    }

    public struct ContentBribe
    {
        public int jointId;
        public int totalMoney;
    }

    public struct BiasesContainer
    {
        public ContentBias[] contentBiases;
    }

    public struct ContentBias
    {
        public int jointId;
    }

    public struct LettersContainer
    {
        public ContentLetter[] contentLetters;
    }

    public struct ContentLetter
    {
        public int jointId;
        public string letterText;
    }

}