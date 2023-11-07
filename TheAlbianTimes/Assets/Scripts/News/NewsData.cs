using NoMonoBehavior;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NewsConsequenceData
{
    public Country.Id country;
    float reputationChange;
}
[System.Serializable]
public class BiasData
{
    public string name;
    public string description;
    NewsConsequenceData[] additionalConsequences;
}
[System.Serializable]
public class NewsData
{
    public NewsType type;

    public string headline;
    public string text;
    public string imagePath;

    NewsConsequenceData[] consequences;

    BiasData[] biases;
}
