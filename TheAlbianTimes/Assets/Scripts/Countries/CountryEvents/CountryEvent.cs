using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CountryEventCondition
{
    public enum Predicate { EQUAL, LESS, GREATER, LESSEQUAL, GREATEREQUAL }
    public Predicate predicate = Predicate.EQUAL;
    public string field;
    public float value;
}


[System.Serializable]
public abstract class CountryEvent
{
    public string id = "";
    public int priority = 0;
    public Country.Id triggerCountry;
    public Country.Id[] countriesInvolved;
    public Dictionary<Country.Id, CountryEventCondition> conditions;

    public bool conditionsFulfilled => IsFulfilled();
    private bool IsFulfilled()
    {
        bool ret = true;
        foreach(KeyValuePair<Country.Id, CountryEventCondition> condition in conditions)
        {
            if (condition.Value.predicate == CountryEventCondition.Predicate.EQUAL &&
                GameManager.Instance.gameState.countries[(int)condition.Key].data.values[condition.Value.field] != condition.Value.value
                )
            {
                ret = false;
                break;
            }
            else if (condition.Value.predicate == CountryEventCondition.Predicate.LESS &&
                GameManager.Instance.gameState.countries[(int)condition.Key].data.values[condition.Value.field] >= condition.Value.value
                )
            {
                ret = false;
                break;
            }
            else if (condition.Value.predicate == CountryEventCondition.Predicate.GREATER &&
                GameManager.Instance.gameState.countries[(int)condition.Key].data.values[condition.Value.field] <= condition.Value.value
                )
            {
                ret = false;
                break;
            }
            else if (condition.Value.predicate == CountryEventCondition.Predicate.LESSEQUAL &&
                GameManager.Instance.gameState.countries[(int)condition.Key].data.values[condition.Value.field] > condition.Value.value
                )
            {
                ret = false;
                break;
            }
            else if (condition.Value.predicate == CountryEventCondition.Predicate.GREATEREQUAL &&
                GameManager.Instance.gameState.countries[(int)condition.Key].data.values[condition.Value.field] < condition.Value.value
                )
            {
                ret = false;
                break;
            }
        }
        return ret;
    }
}
