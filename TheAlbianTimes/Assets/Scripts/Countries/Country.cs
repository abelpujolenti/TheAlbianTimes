using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class CountryData
{
    public Country.Id countryId;
    public Dictionary<string, float> values = new Dictionary<string, float>();
    public CountryData() { }
    public CountryData(CountryData d)
    {
        countryId = d.countryId;
        foreach (KeyValuePair<string, float> v in d.values)
        {
            values.Add(v.Key, v.Value);
        }
    }
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
    private CountryData previousData = new CountryData();
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

    public virtual CountryEvent GenerateEvent()
    {
        float giftChance = giftCurve.Evaluate(GetReputation());
        float bribeChance = bribeCurve.Evaluate(GetReputation());
        float threatChance = threatCurve.Evaluate(GetReputation());

        //TODO: SOS PUTO
        bribeChance = 100f;

        float totalChance = Math.Max(1f, giftChance + bribeChance + threatChance);
        SortedList<float, CountryEvent> events = new SortedList<float, CountryEvent>(new DuplicateKeyComparer<float>());

        float random = Random.Range(0f, 1f);
        if (random < (giftChance) / totalChance) 
        {
            foreach (GiftCountryEvent giftEvent in CountryEventManager.Instance.giftCountryEvents[data.countryId])
            {
                if (giftEvent.ConditionsFulfilled())
                {
                    GenerateEventAddToList(giftEvent, events);
                }
            }
        }
        else if (random < (giftChance + bribeChance) / totalChance)
        {
            foreach (BribeCountryEvent bribeEvent in CountryEventManager.Instance.bribeCountryEvents[data.countryId])
            {
                if (bribeEvent.ConditionsFulfilled())
                {
                    GenerateEventAddToList(bribeEvent, events);
                }
            }
        }
        else if (random < (giftChance + bribeChance + threatChance) / totalChance)
        {
            foreach (ThreatCountryEvent threatEvent in CountryEventManager.Instance.threatCountryEvents[data.countryId])
            {
                if (threatEvent.ConditionsFulfilled())
                {
                    GenerateEventAddToList(threatEvent, events);
                }
            }
        }

        CountryEvent ret = null;
        if (events.Count > 0) 
        {
            ret = events.Values[Random.Range(Math.Max(0, events.Count - 3), events.Count)];
        }

        return ret;
    }

    private void GenerateEventAddToList(CountryEvent ev, SortedList<float, CountryEvent> events)
    {
        if (ev.ConditionsFulfilled())
        {
            events.Add(ev.priority, ev);
        }
    }

    public string DisplayValues()
    {
        string d = "";
        d += "<b>" + GetName() + ":</b>\n";
        foreach (KeyValuePair<string, float> value in data.values)
        {
            if (value.Value <= 1f && value.Value >= 0f)
            {
                d += value.Key + ": " + value.Value.ToString("p0") + " " + StatFormat.FormatValueChange(GetValueChange(value.Key)) + "  ";
            }
            else
            {

            }
        }
        Debug.Log(d);
        return d;
    }

    public void SaveRoundData()
    {
        previousData = new CountryData(data);
    }

    public float GetValueChange(string key)
    {
        if (!data.values.ContainsKey(key) || !previousData.values.ContainsKey(key)) return 0f;
        return data.values[key] - previousData.values[key];
    }

    #region Getters/Setters
    public void AddToValue(string key, float value)
    {
        if (!data.values.ContainsKey(key))
        {
            SetValue(key, value);
        }
        else
        {
            SetValue(key, value + data.values[key]);
        }
    }
    public void SetValue(string key, float value)
    {
        data.values[key] = value;
    }
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
        SetValue("purchasingPower", v);
    }
    public int GetPopulation()
    {
        return (int)data.values["population"];
    }
    public void SetPopulation(int v)
    {
        SetValue("population", v);
    }
    public float GetCensorship()
    {
        return data.values["censorship"];
    }
    public void SetCensorship(float v)
    {
        SetValue("censorship", v);
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
