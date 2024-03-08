using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagePostitData
{
    public string text;
    public Color color = new Color(0f, 0f, 0.05f);
    public int round;
    public CountryEventCondition[] conditions;

    public bool ConditionsFulfilled()
    {
        if (conditions == null) return true;
        foreach (var condition in conditions)
        {
            if (!condition.IsFulfilled()) return false;
        }
        return true;
    }
}
