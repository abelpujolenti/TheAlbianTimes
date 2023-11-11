using System;
using System.IO;
using Editorial;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

public class TESTAddNewsHeadline : InteractableRectTransform
{
    private const String NEWS_PATH = "/Json/News/"; 
    
    [SerializeField] private GameObject _news;

    [SerializeField] private NewsFolder _newsFolder;

    protected override void PointerClick(BaseEventData data)
    {
        GameObject newsHeadlineGameObject = Instantiate(_news, _newsFolder.transform);

        string path = Application.streamingAssetsPath + NEWS_PATH + "test.json";

        if (!File.Exists(path))
        {
            return;
        }

        string json = File.ReadAllText(path);
        
        NewsData newsData = JsonUtility.FromJson<NewsData>(json);

        NewsHeadline newsHeadlineComponent = newsHeadlineGameObject.GetComponent<NewsHeadline>();
        
        newsHeadlineComponent.SetNewsType(newsData.type);
        
        newsHeadlineComponent.SetImagePath(newsData.imagePath);

        NewsConsequenceData[] newsConsequencesData = new NewsConsequenceData[newsData.consequences.Length];

        newsConsequencesData = newsData.consequences;
        
        newsHeadlineComponent.SetNewsConsequencesData(newsConsequencesData);
        
        SetBiasesData(newsHeadlineComponent, newsData);

        EventsManager.OnAddNewsHeadlineToFolder(newsHeadlineGameObject);
    }

    private void SetBiasesData(NewsHeadline newsHeadline, NewsData newsData)
    {
        BiasData[] biases = newsData.biases;
        int biasesCount = biases.Length;

        String[] biasesTitle = new String[biasesCount];
        for (int i = 0; i < biasesCount; i++)
        {
            biasesTitle[i] = biases[i].name;
        }
        newsHeadline.SetBiasNames(biasesTitle);

        String[] biasesDescription = new string[biasesCount];
        for (int i = 0; i < biasesCount; i++)
        {
            biasesDescription[i] = biases[i].description;
        }
        newsHeadline.SetBiasesDescription(biasesDescription);

        String[] biasesHeadlinesText = new String[biasesCount];
        for (int i = 0; i < biasesCount; i++)
        {
            biasesHeadlinesText[i] = biases[i].headline;
        }
        newsHeadline.SetHeadlinesText(biasesHeadlinesText);

        String[] biasesText = new String[biasesCount];
        for (int i = 0; i < biasesCount; i++)
        {
            biasesText[i] = biases[i].text;
        }
        newsHeadline.SetBiasContent(biasesText);
    }
}
