using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
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
    [SerializeField] protected string[] events;
    #endregion

    #region Properties
    public CountryData data = new CountryData();
    protected float lastReputationChange = 0f;
    #endregion

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
        lastReputationChange = 0;
        return null;
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
