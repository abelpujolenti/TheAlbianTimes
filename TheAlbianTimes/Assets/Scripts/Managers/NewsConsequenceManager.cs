using Managers;
using UnityEngine;

public class NewsConsequenceManager : MonoBehaviour
{
    private static NewsConsequenceManager _instance;
    public static NewsConsequenceManager Instance => _instance;
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
    public void ApplyNewsConsequences(NewsData[] newsData)
    {
        foreach (NewsData article in newsData)
        {
            foreach (NewsConsequenceData consequence in article.consequences)
            {
                ApplyNewsConsequencesToCountry(consequence);
            }
            foreach (NewsConsequenceData consequence in article.biases[article.currentBias].additionalConsequences)
            {
                ApplyNewsConsequencesToCountry(consequence);
            }
        }
    }
    private void ApplyNewsConsequencesToCountry(NewsConsequenceData consequence)
    {
        GameManager.Instance.gameState.countries[(int)consequence.country].ModifyValue(consequence.key, consequence.value);
    }

}
