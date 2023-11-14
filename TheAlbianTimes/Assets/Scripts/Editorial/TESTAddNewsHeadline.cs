using System;
using System.IO;
using Editorial;
using Layout;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

public class TESTAddNewsHeadline : InteractableRectTransform
{
    private const String NEWS_PATH = "/Json/News/";
    private const float SPAWN_Y_COORDINATE = 1000;
    
    [SerializeField] private GameObject _newsHeadline;
    [SerializeField] private GameObject _newsHeadlinePiece;
    [SerializeField] private GameObject _newsHeadlinesPiecesContainer;

    [SerializeField] private NewsFolder _newsFolder;
    

    protected override void PointerClick(BaseEventData data)
    {
        string filePath = Application.streamingAssetsPath + NEWS_PATH + "test.json";

        if (!File.Exists(filePath))
        {
            return;
        }
        
        GameObject newsHeadlineGameObject = Instantiate(_newsHeadline, _newsFolder.transform);
        GameObject newsHeadlinePieceGameObject =
            Instantiate(_newsHeadlinePiece, _newsHeadlinesPiecesContainer.transform);

        newsHeadlineGameObject.transform.localPosition = new Vector2(0, SPAWN_Y_COORDINATE);
        
        newsHeadlinePieceGameObject.SetActive(false);
        
        NewsHeadlinePiece newsHeadlinePieceComponent = newsHeadlinePieceGameObject.GetComponent<NewsHeadlinePiece>();
        
        NewsHeadlineSubPiece[] newsHeadlineSubPieces = newsHeadlinePieceComponent.GetNewsHeadlinesSubPieces();
        
        SetNewsHeadlineData(newsHeadlineGameObject, newsHeadlinePieceComponent, newsHeadlineSubPieces, filePath);
        
        SetNewsHeadlinePieceData(newsHeadlineGameObject, newsHeadlinePieceGameObject, newsHeadlineSubPieces);

        EventsManager.OnAddNewsHeadlineToFolder(newsHeadlineGameObject);
    }

    private void SetNewsHeadlineData(GameObject newsHeadlineGameObject, NewsHeadlinePiece newsHeadlinePiece,
        NewsHeadlineSubPiece[] newsHeadlineSubPieces, String filePath)
    {
        NewsHeadline newsHeadlineComponent = newsHeadlineGameObject.GetComponent<NewsHeadline>();

        Vector2 rootCoordinate = new Vector2(0, 0);
        
        for (int i = 0; i < newsHeadlineSubPieces.Length; i++)
        {
            newsHeadlineSubPieces[i].SetNewsHeadlineToTransferDrag(newsHeadlineComponent);
            if (newsHeadlineSubPieces[i].GetCoordinates() != rootCoordinate)
            {
                continue;
            }
            newsHeadlineComponent.SetGameObjectToTransferDrag(newsHeadlineSubPieces[i].gameObject);      
        }
        newsHeadlineComponent.SetNewsHeadlineSubPieceToTransferDrag(newsHeadlinePiece);
        
        string json = File.ReadAllText(filePath);
        
        NewsData newsData = JsonUtility.FromJson<NewsData>(json);
        
        newsHeadlineComponent.SetNewsType(newsData.type);
        
        newsHeadlineComponent.SetImagePath(newsData.imagePath);

        NewsConsequenceData[] newsConsequencesData = new NewsConsequenceData[newsData.consequences.Length];

        newsConsequencesData = newsData.consequences;
        
        newsHeadlineComponent.SetNewsConsequencesData(newsConsequencesData);
        
        SetBiasesData(newsHeadlineComponent, newsData);
    }

    private void SetNewsHeadlinePieceData(GameObject newsHeadlineGameObject, GameObject newsHeadlinePieceGameObject, 
        NewsHeadlineSubPiece[] newsHeadlineSubPieces)
    {
        for (int i = 0; i < newsHeadlineSubPieces.Length; i++)
        {
            newsHeadlineSubPieces[i].SetGameObjectToTransferDrag(newsHeadlineGameObject);
        }
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
