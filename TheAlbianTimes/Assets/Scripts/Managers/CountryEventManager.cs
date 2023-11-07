using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CountryEventManager : MonoBehaviour
{
    private static CountryEventManager _instance;
    public static CountryEventManager Instance => _instance;

    public Dictionary<Country.Id, List<BribeCountryEvent>> bribeCountryEvents = new Dictionary<Country.Id, List<BribeCountryEvent>>();
    public Dictionary<Country.Id, List<GiftCountryEvent>> giftCountryEvents = new Dictionary<Country.Id, List<GiftCountryEvent>>();
    public Dictionary<Country.Id, List<ThreatCountryEvent>> threatCountryEvents = new Dictionary<Country.Id, List<ThreatCountryEvent>>();

    public SortedList<int, CountryEvent> currentEvents = new SortedList<int, CountryEvent>(new DuplicateKeyComparer<int>());

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
    }

    public void AddEventToQueue(CountryEvent newEvent)
    {
        if (newEvent == null) return;
        currentEvents.Add(newEvent.priority, newEvent);
    }

    public CountryEvent PopFirstEvent()
    {
        CountryEvent e;
        e = currentEvents.Last().Value;
        currentEvents.RemoveAt(currentEvents.Count - 1);
        return e;
    }

    private void AddThreatEventFromJson(string json)
    {
        ThreatCountryEvent e = JsonUtility.FromJson<ThreatCountryEvent>(json);
        threatCountryEvents[e.triggerCountry].Add(e);
    }
    private void AddBribeEventFromJson(string json)
    {
        BribeCountryEvent e = JsonUtility.FromJson<BribeCountryEvent>(json);
        bribeCountryEvents[e.triggerCountry].Add(e);
    }
    private void AddGiftEventFromJson(string json)
    {
        GiftCountryEvent e = JsonUtility.FromJson<GiftCountryEvent>(json);
        giftCountryEvents[e.triggerCountry].Add(e);
    }

}
