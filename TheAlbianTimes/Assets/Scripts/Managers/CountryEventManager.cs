using System.Collections.Generic;
using System.Linq;
using Countries;
using Managers;
using UnityEngine;

public class CountryEventManager : MonoBehaviour
{
    private static CountryEventManager _instance;
    public static CountryEventManager Instance => _instance;

    public Dictionary<Country.Id, List<BribeCountryEvent>> bribeCountryEvents = new Dictionary<Country.Id, List<BribeCountryEvent>>();
    public Dictionary<Country.Id, List<GiftCountryEvent>> giftCountryEvents = new Dictionary<Country.Id, List<GiftCountryEvent>>();
    public Dictionary<Country.Id, List<ThreatCountryEvent>> threatCountryEvents = new Dictionary<Country.Id, List<ThreatCountryEvent>>();

    public SortedList<float, CountryEvent> currentEvents = new SortedList<float, CountryEvent>(new DuplicateKeyComparer<float>());

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
    }

    private void Start()
    {
        LoadEvents();
    }

    private void LoadEvents()
    {
        for (Country.Id i = 0; i < Country.Id.AMOUNT; i++)
        {
            bribeCountryEvents.Add(i, new List<BribeCountryEvent>());
            threatCountryEvents.Add(i, new List<ThreatCountryEvent>());
            giftCountryEvents.Add(i, new List<GiftCountryEvent>());
        }
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
        if (currentEvents.Count == 0) return null;
        CountryEvent e;
        e = currentEvents.Last().Value;
        currentEvents.RemoveAt(currentEvents.Count - 1);
        return e;
    }

    private void AddThreatEventFromJson(string json)
    {
        ThreatCountryEvent e = JsonUtility.FromJson<ThreatCountryEvent>(json);
        if (!threatCountryEvents.ContainsKey(e.triggerCountry)) return;
        threatCountryEvents[e.triggerCountry].Add(e);
    }
    private void AddBribeEventFromJson(string json)
    {
        BribeCountryEvent e = JsonUtility.FromJson<BribeCountryEvent>(json);
        if (!bribeCountryEvents.ContainsKey(e.triggerCountry)) return;
        bribeCountryEvents[e.triggerCountry].Add(e);
    }
    private void AddGiftEventFromJson(string json)
    {
        GiftCountryEvent e = JsonUtility.FromJson<GiftCountryEvent>(json);
        if (!giftCountryEvents.ContainsKey(e.triggerCountry)) return;
        giftCountryEvents[e.triggerCountry].Add(e);
    }

}
