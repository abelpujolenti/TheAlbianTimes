using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CountryEventManager : MonoBehaviour
{
    private static CountryEventManager _instance;
    public static CountryEventManager Instance => _instance;

    List<BribeCountryEvent> bribeCountryEvents = new List<BribeCountryEvent>();
    List<GiftCountryEvent> giftCountryEvents = new List<GiftCountryEvent>();
    List<ThreatCountryEvent> threatCountryEvents = new List<ThreatCountryEvent>();

    private void Awake()
    {
        if (_instance == null )
        {
            _instance = this;
        }
        else
        {
            Destroy( _instance );
        }
        DontDestroyOnLoad(gameObject); 
    }

    private void Start()
    {
        LoadThreatCountryEvents();
    }

    private void LoadThreatCountryEvents()
    {
        System.Object[] files = Resources.LoadAll("Json/CountryEvents/ThreatCountryEvent", typeof(TextAsset));
        for (int i = 0; i < files.Length; i++)
        {
            threatCountryEvents.Add(JsonUtility.FromJson<ThreatCountryEvent>(((TextAsset)files[i]).text));
            Debug.Log(threatCountryEvents[i].id);
        }
    }

}
