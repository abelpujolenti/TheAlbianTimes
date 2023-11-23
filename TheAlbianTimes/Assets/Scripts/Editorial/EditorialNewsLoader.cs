using System;
using System.IO;
using System.Security.Cryptography;
using Editorial;
using Layout;
using Managers;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

public class EditorialNewsLoader : MonoBehaviour
{
    private const String NEWS_PATH = "News/Round";
    private const float SPAWN_Y_COORDINATE = 1000;
    
    [SerializeField] private GameObject _newsHeadline;
    [SerializeField] private GameObject _newsHeadlinePiece;
    [SerializeField] private GameObject _newsHeadlinesPiecesContainer;
    [SerializeField] private NewsFolder _newsFolder;
    private PieceGenerator pieceGenerator = new PieceGenerator();

    private void Start()
    {
        LoadLevelNews();
    }

    private void LoadLevelNews()
    {
        string path = NEWS_PATH + GameManager.Instance.GetRound();
        FileManager.LoadAllJsonFiles(path, LoadNewsFromFile);
    }

    private void LoadNewsFromFile(string json)
    {
        NewsData newsData = JsonUtility.FromJson<NewsData>(json);
        GameObject newsHeadlineGameObject = Instantiate(_newsHeadline, _newsFolder.transform);
        GameObject newsHeadlinePieceGameObject = pieceGenerator.Generate(newsData, _newsHeadlinesPiecesContainer.transform).gameObject;

        newsHeadlineGameObject.transform.localPosition = new Vector2(0, SPAWN_Y_COORDINATE);
        
        newsHeadlinePieceGameObject.SetActive(false);
        
        NewsHeadlinePiece newsHeadlinePieceComponent = newsHeadlinePieceGameObject.GetComponent<NewsHeadlinePiece>();
        
        NewsHeadlineSubPiece[] newsHeadlineSubPieces = newsHeadlinePieceComponent.GetNewsHeadlinesSubPieces();
        
        SetNewsHeadlineData(newsHeadlineGameObject, newsHeadlinePieceComponent, newsHeadlineSubPieces, newsData);
        
        SetNewsHeadlinePieceData(newsHeadlineGameObject, newsHeadlinePieceGameObject, newsHeadlineSubPieces);

        EventsManager.OnAddNewsHeadlineToFolder(newsHeadlineGameObject);
    }

    private void SetNewsHeadlineData(GameObject newsHeadlineGameObject, NewsHeadlinePiece newsHeadlinePiece,
        NewsHeadlineSubPiece[] newsHeadlineSubPieces, NewsData newsData)
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

        newsHeadlineComponent.SetNewsData(newsData);

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
