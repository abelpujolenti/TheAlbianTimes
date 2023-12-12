using System;
using Managers;

[Serializable]
public class CountryCondition
{
    public enum Predicate { EQUAL, LESS, GREATER, LESSEQUAL, GREATEREQUAL }
    public Country.Id country;
    public Predicate predicate = Predicate.EQUAL;
    public string field;
    public float value;
    public bool IsFulfilled()
    {
        if (predicate == Predicate.EQUAL &&
        GameManager.Instance.gameState.countries[(int)country].data.values[field] != value
            )
        {
            return false;
        }
        else if (predicate == Predicate.LESS &&
            GameManager.Instance.gameState.countries[(int)country].data.values[field] >= value
            )
        {
            return false;
        }
        else if (predicate == Predicate.GREATER &&
            GameManager.Instance.gameState.countries[(int)country].data.values[field] <= value
            )
        {
            return false;
        }
        else if (predicate == Predicate.LESSEQUAL &&
            GameManager.Instance.gameState.countries[(int)country].data.values[field] > value
            )
        {
            return false;
        }
        else if (predicate == Predicate.GREATEREQUAL &&
            GameManager.Instance.gameState.countries[(int)country].data.values[field] < value
            )
        {
            return false;
        }
        return true;
    }
}


[Serializable]
public abstract class CountryEvent
{
    public string id = "";
    public int priority = 0;
    public Country.Id triggerCountry;
    public Country.Id[] countriesInvolved;
    public CountryCondition[] conditions;

    public bool ConditionsFulfilled()
    {
        if (conditions == null) return true;
        bool ret = true;
        foreach(CountryCondition condition in conditions)
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
