using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        FileManager.LoadAllJsonFiles("CountryEvents/ThreatCountryEvent", AddThreatEventFromJson);
        FileManager.LoadAllJsonFiles("CountryEvents/BribeCountryEvent", AddBribeEventFromJson);
        FileManager.LoadAllJsonFiles("CountryEvents/GiftCountryEvent", AddGiftEventFromJson);
        Debug.Log("Threats: " + threatCountryEvents.Count);
        Debug.Log("Bribes: " + bribeCountryEvents.Count);
        Debug.Log("Gift: " + giftCountryEvents.Count);
    }

    private void AddThreatEventFromJson(string json)
    {
        threatCountryEvents.Add(JsonUtility.FromJson<ThreatCountryEvent>(json));
    }
    private void AddBribeEventFromJson(string json)
    {
        bribeCountryEvents.Add(JsonUtility.FromJson<BribeCountryEvent>(json));
    }
    private void AddGiftEventFromJson(string json)
    {
        giftCountryEvents.Add(JsonUtility.FromJson<GiftCountryEvent>(json));
    }

}
