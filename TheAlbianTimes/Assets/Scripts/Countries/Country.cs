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
        CountryEvent ret = null;

        float giftChance = giftCurve.Evaluate(GetReputation());
        float bribeChance = bribeCurve.Evaluate(GetReputation());
        float threatChance = threatCurve.Evaluate(GetReputation());

        foreach (GiftCountryEvent giftEvent in CountryEventManager.Instance.giftCountryEvents[data.countryId])
        {
            if (giftEvent.conditionsFulfilled)
            {

            }
        }

        lastReputationChange = 0;
        return ret;
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
