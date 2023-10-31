using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Windows;

public class Country : MonoBehaviour
{
    
    public float reputation = .5f;
    public float censorship = 0f;

    [SerializeField] protected AnimationCurve censorshipCurve;
    [SerializeField] protected AnimationCurve giftCurve;
    [SerializeField] protected AnimationCurve bribeCurve;
    [SerializeField] protected AnimationCurve threatCurve;
    [SerializeField] protected string[] events;

    private float lastReputationChange = 0f;

    private void Start()
    {
        Setup();
    }

    protected virtual void Setup()
    {
        LoadCountryEvents();
    }

    public void AffectReputation(float change)
    {
        float prevRep = reputation;
        reputation = Mathf.Max(0f, Mathf.Min(1f, reputation + change));
        lastReputationChange += reputation - prevRep;

        censorship = censorshipCurve.Evaluate(reputation);
    }

    public virtual CountryEvent GenerateEvent()
    {
        return null;
    }

    protected void LoadCountryEvents()
    {

    }
}
