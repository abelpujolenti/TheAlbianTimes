using System;
using NoMonoBehavior;

[Serializable]
public class NewsConsequenceData
{
    public Country.Id country;
    public float reputationChange;
}
[Serializable]
public class BiasData
{
    public string name;
    public string description;

    public string headline;
    public string text;

    public NewsConsequenceData[] additionalConsequences;
}
[Serializable]
public class NewsData
{
    public NewsType type;

    public string imagePath;

    public NewsConsequenceData[] consequences;

    public BiasData[] biases;
}
