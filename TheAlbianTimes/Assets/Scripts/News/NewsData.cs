using System;
using System.Runtime.InteropServices.WindowsRuntime;
using NoMonoBehavior;

[Serializable]
public class NewsConsequenceData
{
    public Country.Id country;
    public string key;
    public float value;
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
    public int firstRound;
    public float duration;
    public float priority;
    public float leniency;
    public CountryEventCondition[] conditions;
    public bool viewed = false;

    public NewsType type;

    public string imagePath;

    public NewsConsequenceData[] consequences;

    public BiasData[] biases;
    public int currentBias;

    public bool ConditionsFulfilled(int round)
    {
        if (viewed || round < firstRound || (leniency == 0f && round > firstRound + duration)) return false;
        if (conditions == null) return true;
        bool ret = true;
        foreach(CountryEventCondition condition in conditions)
        {
            if (!condition.IsFulfilled())
            {
                ret = false;
            }
        }
        return ret;
    }
}
