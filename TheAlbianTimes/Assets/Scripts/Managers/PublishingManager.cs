using System.Collections.Generic;
using System.Linq;
using Countries;
using Managers;
using UnityEngine;
using Workspace.Editorial;

public class PublishingManager : MonoBehaviour
{
    private static PublishingManager _instance;
    public static PublishingManager Instance => _instance;

    public Dictionary<Vector3, NewsData> currentPublishedArticles;

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
    public void Publish(List<NewsHeadline> publishedNewsaper)
    {
        currentPublishedArticles = new Dictionary<Vector3, NewsData>();
        foreach (NewsHeadline headline in publishedNewsaper)
        {
            //Debug.Log(headline.GetNewsData().biases[0].name);
            currentPublishedArticles.Add(headline.GetPiecePosition() - new Vector3(-112.82f, 0.00f, 0.00f), headline.GetNewsData());
        }

        foreach (Country country in GameManager.Instance.gameState.countries)
        {
            country.SaveRoundData();
        }

        NewsConsequenceManager.Instance.ApplyNewsConsequences(currentPublishedArticles.Values.ToArray());

        float income = PlayerDataManager.Instance.CalculateRevenue() - PlayerDataManager.Instance.CalculateCosts();
        PlayerDataManager.Instance.UpdateMoney(income);

        float currentGlobalReputation = PlayerDataManager.Instance.CalculateGlobalReputation();
        float diff = currentGlobalReputation - GameManager.Instance.gameState.playerData.reputation;
        PlayerDataManager.Instance.UpdateReputation(currentGlobalReputation);

        GenerateCountryEvents();
    }

    private void GenerateCountryEvents()
    {
        foreach (Country country in GameManager.Instance.gameState.countries)
        {
            CountryEventManager.Instance.AddEventToQueue(country.GenerateEvent());
        }
    }
}
