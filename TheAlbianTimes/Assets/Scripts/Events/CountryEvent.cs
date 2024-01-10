using System;

[Serializable]
public abstract class CountryEvent
{
    public string id = "";
    public float priority = 0;
    public Country.Id triggerCountry;
    public Country.Id[] countriesInvolved;
    public CountryEventCondition[] conditions;

    public virtual void Run()
    {
        
    }

    public bool ConditionsFulfilled()
    {
        if (conditions == null) return true;
        bool ret = true;
        foreach(CountryEventCondition condition in conditions)
        {
            if (!condition.IsFulfilled())
            {
                ret = false;
                break;
            }
        }
        return ret;
    }
}
