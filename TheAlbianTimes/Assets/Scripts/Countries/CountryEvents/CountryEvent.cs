using System;
using Managers;

[Serializable]
public class CountryEventCondition
{
    public enum Predicate { EQUAL, LESS, GREATER, LESSEQUAL, GREATEREQUAL }
    public Country.Id country;
    public Predicate predicate = Predicate.EQUAL;
    public string field;
    public float value;
}


[Serializable]
public abstract class CountryEvent
{
    public string id = "";
    public int priority = 0;
    public Country.Id triggerCountry;
    public Country.Id[] countriesInvolved;
    public CountryEventCondition[] conditions;

    public bool conditionsFulfilled => IsFulfilled();
    private bool IsFulfilled()
    {
        if (conditions == null) return true;
        bool ret = true;
        foreach(CountryEventCondition condition in conditions)
        {
            if (condition.predicate == CountryEventCondition.Predicate.EQUAL &&
                GameManager.Instance.gameState.countries[(int)condition.country].data.values[condition.field] != condition.value
                )
            {
                ret = false;
                break;
            }
            else if (condition.predicate == CountryEventCondition.Predicate.LESS &&
                GameManager.Instance.gameState.countries[(int)condition.country].data.values[condition.field] >= condition.value
                )
            {
                ret = false;
                break;
            }
            else if (condition.predicate == CountryEventCondition.Predicate.GREATER &&
                GameManager.Instance.gameState.countries[(int)condition.country].data.values[condition.field] <= condition.value
                )
            {
                ret = false;
                break;
            }
            else if (condition.predicate == CountryEventCondition.Predicate.LESSEQUAL &&
                GameManager.Instance.gameState.countries[(int)condition.country].data.values[condition.field] > condition.value
                )
            {
                ret = false;
                break;
            }
            else if (condition.predicate == CountryEventCondition.Predicate.GREATEREQUAL &&
                GameManager.Instance.gameState.countries[(int)condition.country].data.values[condition.field] < condition.value
                )
            {
                ret = false;
                break;
            }
        }
        return ret;
    }
}
