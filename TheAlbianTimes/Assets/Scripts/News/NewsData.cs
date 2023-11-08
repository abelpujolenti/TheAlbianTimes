using System;
using NoMonoBehavior;

[Serializable]
public class NewsConsequenceData
{
    public Country.Id country;
    float reputationChange;
}
[Serializable]
public class BiasData
{
    public string name;
    public string description;
    NewsConsequenceData[] additionalConsequences;
}
[Serializable]
public class NewsData
{
    public NewsType type;

    public string headline;
    public string text;
    public string imagePath;

    NewsConsequenceData[] consequences;

    BiasData[] biases;
}
