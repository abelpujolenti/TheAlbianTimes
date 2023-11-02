using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance;

    public static GameManager Instance => _instance;

    private const string PIECES_FILE_PATH = "/Json/NewsHeadline/NewsHeadlineTypes.json";

    private Dictionary<NewsType, Vector2[]> _newsPiecesCoordinates;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _newsPiecesCoordinates = new Dictionary<NewsType, Vector2[]>();
            LoadPiecesCoordinatesFromFile();
        }
        else 
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void LoadPiecesCoordinatesFromFile() 
    {

        if (!File.Exists(Application.streamingAssetsPath + PIECES_FILE_PATH))
        {
            return;
        }

        string json = File.ReadAllText(Application.streamingAssetsPath + PIECES_FILE_PATH);

        NewsTypeObject newsTypeObject = JsonUtility.FromJson<NewsTypeObject>(json);

        foreach (NewsTypeData newObject in newsTypeObject.newsTypeData) {

            Vector2[] coordinates = new Vector2[newObject.piecesCoordinatesFromRootPiece.Length];

            for (int i = 0; i < coordinates.Length; i++) 
            {
                coordinates[i] = newObject.piecesCoordinatesFromRootPiece[i];
            }

            _newsPiecesCoordinates.Add(newObject.type, coordinates);
        }
    }

    public Vector2[] GetPiecesCoordinates(NewsType newsType)
    { 

        return _newsPiecesCoordinates[newsType];
    }
}
