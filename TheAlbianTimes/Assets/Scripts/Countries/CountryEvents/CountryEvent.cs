using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class CountryEvent
{
    public string id = "";
    public int priority = 0;
    public Country.Id[] countriesInvolved;
}
