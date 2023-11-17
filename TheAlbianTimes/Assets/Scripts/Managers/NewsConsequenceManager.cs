using Managers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        foreach (Country c in GameManager.Instance.gameState.countries)
        {
            c.DebugValues();
        }
    }
    private void ApplyNewsConsequencesToCountry(NewsConsequenceData consequence)
    {
        GameManager.Instance.gameState.countries[(int)consequence.country].ModifyValue(consequence.key, consequence.value);
    }

}
