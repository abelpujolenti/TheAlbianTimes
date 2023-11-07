using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[System.Serializable]
public class CountryData
{
    public Country.Id countryId;
    public Dictionary<string, float> values = new Dictionary<string, float>();
}

public class Country : MonoBehaviour
{
    #region Names
    public enum Id
    {
        HETIA,
        TERKAN,
        XAYA,
        ZUANIA,
        DALME,
        ALBIA,
        MADIA,
        SUOKA,
        REKKA,
        AMOUNT
    }
    public static readonly string[] names = {"Hetia", "Terkán", "Xaya", "Zuania", "Dalme", "Albia", "Madia", "Suoka", "Rekka"};
    #endregion

    #region Stats
    [SerializeField] protected AnimationCurve censorshipCurve;
    [SerializeField] protected AnimationCurve giftCurve;
    [SerializeField] protected AnimationCurve bribeCurve;
    [SerializeField] protected AnimationCurve threatCurve;
    
    [SerializeField] protected int defaultPopulation;
    [SerializeField] protected float defaultReputation;
    [SerializeField] protected float defaultCensorship;
    [SerializeField] protected float defaultPurchasingPower;

    [SerializeField] protected HashSet<string> eventIds;
    #endregion

    #region Properties
    public CountryData data = new CountryData();
    protected float lastReputationChange = 0f;
    #endregion

    private void Awake()
    {
        SetDefaultValues();
    }

    private void SetDefaultValues()
    {
        SetPopulation(defaultPopulation);
        SetReputation(defaultReputation);
        SetCensorship(defaultCensorship);
        SetPurchasingPower(defaultPurchasingPower);
    }

    public void AffectReputation(float change)
    {
        float prevRep = GetReputation();
        float newRep = Mathf.Max(0f, Mathf.Min(1f, prevRep + change));
        SetReputation(newRep);
        lastReputationChange += newRep - prevRep;

        SetCensorship(censorshipCurve.Evaluate(GetReputation()));
    }

    public virtual CountryEvent GenerateEvent()
    {
        float giftChance = giftCurve.Evaluate(GetReputation());
        float bribeChance = bribeCurve.Evaluate(GetReputation());
        float threatChance = threatCurve.Evaluate(GetReputation());

        float totalChance = Math.Max(1f, giftChance + bribeChance + threatChance);
        SortedList<int, CountryEvent> events = new SortedList<int, CountryEvent>(new DuplicateKeyComparer<int>());

        float random = UnityEngine.Random.Range(0f, 1f);
        if (random < (giftChance) / totalChance) 
        {
            foreach (GiftCountryEvent giftEvent in CountryEventManager.Instance.giftCountryEvents[data.countryId])
            {
                GenerateEventAddToList(giftEvent, events);
            }
        }
        else if (random < (giftChance + bribeChance) / totalChance)
        {
            foreach (BribeCountryEvent bribeEvent in CountryEventManager.Instance.bribeCountryEvents[data.countryId])
            {
                if (bribeEvent.conditionsFulfilled)
                {
                    GenerateEventAddToList(bribeEvent, events);
                }
            }
        }
        else if (random < (giftChance + bribeChance + threatChance) / totalChance)
        {
            foreach (ThreatCountryEvent threatEvent in CountryEventManager.Instance.threatCountryEvents[data.countryId])
            {
                if (threatEvent.conditionsFulfilled)
                {
                    GenerateEventAddToList(threatEvent, events);
                }
            }
        }

        lastReputationChange = 0;

        CountryEvent ret = null;
        if (events.Count > 0) 
        {
            ret = events.Values[UnityEngine.Random.Range(Math.Max(0, events.Count - 3), events.Count)];
        }

        return ret;
    }

    private void GenerateEventAddToList(CountryEvent ev, SortedList<int, CountryEvent> events)
    {
        if (ev.conditionsFulfilled)
        {
            events.Add(ev.priority, ev);
        }
    }

    #region Getters/Setters
    public float GetReputation()
    {
        return data.values["reputation"];
    }
    public void SetReputation(float v)
    {
        data.values["reputation"] = v;
    }
    public float GetPurchasingPower()
    {
        return data.values["purchasingPower"];
    }
    public void SetPurchasingPower(float v)
    {
        data.values["purchasingPower"] = v;
    }
    public int GetPopulation()
    {
        return (int)data.values["population"];
    }
    public void SetPopulation(int v)
    {
        data.values["population"] = v;
    }
    public float GetCensorship()
    {
        return data.values["censorship"];
    }
    public void SetCensorship(float v)
    {
        data.values["censorship"] = v;
    }
    public Id GetId()
    {
        return data.countryId;
    }
    public string GetName()
    {
        return names[(int)GetId()];
    }
    #endregion
}
