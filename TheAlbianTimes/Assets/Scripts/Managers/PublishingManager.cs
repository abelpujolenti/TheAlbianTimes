using System.Collections.Generic;
using Layout;
using Managers;
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
        NewsHeadlinePiece[] pieces = FindObjectsByType<NewsHeadlinePiece>(FindObjectsSortMode.None);
        List<NewsData> articles = new List<NewsData>();
        foreach (NewsHeadlinePiece piece in pieces)
        {
            articles.Add(piece.GetNewsHeadlinesSubPieces()[0].GetNewsHeadline().GetNewsData());
        }
        NewsConsequenceManager.Instance.ApplyNewsConsequences(articles.ToArray());

        float income = PlayerDataManager.Instance.CalculateRevenue() - PlayerDataManager.Instance.CalculateCosts();
        PlayerDataManager.Instance.UpdateMoney(income);

        float currentGlobalReputation = PlayerDataManager.Instance.CalculateGlobalReputation();
        float diff = currentGlobalReputation - GameManager.Instance.gameState.playerData.reputation;
        PlayerDataManager.Instance.UpdateReputation(currentGlobalReputation);

        GameManager.Instance.sceneLoader.SetScene("StatsScene");
    } 
}
