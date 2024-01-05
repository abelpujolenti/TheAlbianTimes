using System;
using System.Collections.Generic;
using System.Linq;
using Editorial;
using Layout;
using Managers;
using UnityEngine;

public class EditorialNewsLoader : MonoBehaviour
{
    private const String NEWS_PATH = "News";
    private const float SPAWN_Y_COORDINATE = 1000;
    
    [SerializeField] private GameObject _newsHeadline;
    [SerializeField] private GameObject _newsHeadlinePiece;
    [SerializeField] private GameObject _newsHeadlinesPiecesContainer;
    [SerializeField] private NewsFolder _newsFolder;
    private PieceGenerator pieceGenerator = new PieceGenerator();

    SortedList<float, NewsData> news;

    private void Start()
    {
        LoadLevelNews();
    }

    private void LoadLevelNews()
    {
        string path = NEWS_PATH;
        news = new SortedList<float, NewsData>(new DuplicateKeyComparer<float>());

        FileManager.LoadAllJsonFiles(path, LoadNewsFromFile);
        if (news.Count == 0) return;

        int newsCount = CalculateMaxArticles(GameManager.Instance.gameState.playerData.staff);
        newsCount = 2;
        for (int i = 0; news.Count > 0 && (i < newsCount || news.Last().Key == 1); i++)
        {
            CreateNewsObject(news.Last().Value);
            news.RemoveAt(news.Count - 1);
        }
    }

    private int CalculateMaxArticles(int staff)
    {
        return Mathf.Min(5, 2 + Mathf.FloorToInt(1.5f * Mathf.Sqrt(staff / 10f)));
    }

    private int CalculateRerolls(int staff)
    {
        return Mathf.FloorToInt(1.5f * Mathf.Sqrt(Mathf.Max(0f, staff / 10f - 1f)));
    }

    private void LoadNewsFromFile(string json)
    {
        NewsData newsData = JsonUtility.FromJson<NewsData>(json);

        int round = GameManager.Instance.GetRound();

        if (!newsData.ConditionsFulfilled(round)) return;
        float priority = round < newsData.firstRound + newsData.duration ? 
            newsData.priority :
            newsData.priority - Mathf.Pow(round - (newsData.firstRound + newsData.duration - 1), 2f) / (newsData.leniency + 1);

        if (priority > 0)
        {
            news.Add(priority, newsData);
        }
    }

    private void CreateNewsObject(NewsData newsData)
    {
        GameManager.Instance.gameState.viewedArticles.Add(newsData.name);

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
