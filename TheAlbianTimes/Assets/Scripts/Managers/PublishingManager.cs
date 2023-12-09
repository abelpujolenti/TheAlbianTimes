using Layout;
using Managers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PublishingManager : MonoBehaviour
{
    private static PublishingManager _instance;
    public static PublishingManager Instance => _instance;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Publish()
    {
        //this is bad
        NewsHeadlinePiece[] pieces = FindObjectsByType<NewsHeadlinePiece>(FindObjectsSortMode.None);
        List<NewsData> articles = new List<NewsData>();
        foreach (NewsHeadlinePiece piece in pieces)
        {
            articles.Add(piece.GetNewsHeadlinesSubPieces()[0].GetNewsHeadline().GetNewsData());
        }

        foreach (Country country in GameManager.Instance.gameState.countries)
        {
            country.SaveRoundData();
        }

        NewsConsequenceManager.Instance.ApplyNewsConsequences(articles.ToArray());

        float income = PlayerDataManager.Instance.CalculateRevenue() - PlayerDataManager.Instance.CalculateCosts();
        PlayerDataManager.Instance.UpdateMoney(income);

        float currentGlobalReputation = PlayerDataManager.Instance.CalculateGlobalReputation();
        float diff = currentGlobalReputation - GameManager.Instance.gameState.playerData.reputation;
        PlayerDataManager.Instance.UpdateReputation(currentGlobalReputation);

        GenerateCountryEvents();
        Debug.Log("Events:");
        foreach (KeyValuePair<int, CountryEvent> ev in CountryEventManager.Instance.currentEvents)
        {
            Debug.Log(ev.Value.id);
        }

        GameManager.Instance.sceneLoader.SetScene("StatsScene");
    }

    private void GenerateCountryEvents()
    {
        foreach (Country country in GameManager.Instance.gameState.countries)
        {
            CountryEventManager.Instance.AddEventToQueue(country.GenerateEvent());
        }
    }
}
