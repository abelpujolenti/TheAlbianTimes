using System.Collections.Generic;
using System.IO;
using Editorial;
using Managers.ScriptableObjects;
using NoMonoBehavior;
using UnityEngine;

namespace Managers
{
    public class LayoutManager : MonoBehaviour
    {

        private static LayoutManager _instance;

        public static LayoutManager Instance => _instance;

        [SerializeField] private LayoutManagerData _layoutManagerData;

        private const string PIECES_FILE_PATH = "/Json/NewsHeadline/NewsHeadlineTypes.json";

        private Dictionary<NewsType, Vector2[]> _newsPiecesCoordinates;

        private GameObject _newsHeadlinePiecesCanvas;

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

        void Start()
        {
            _newsHeadlinePiecesCanvas = GameObject.Find("NewsHeadlinePiecesCanvas");
            _newsPiecesCoordinates = _layoutManagerData._newsPiecesCoordinates ?? new Dictionary<NewsType, Vector2[]>();
            LoadPiecesCoordinatesFromFile();
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

        public void SendNewsHeadlinePieceToGameManager(GameObject newsHeadlinePiece)
        {
            GameManager.Instance.SendNewsHeadlineToEditorialManager(newsHeadlinePiece);
        }

        public void SendNewsHeadlineToNewsHeadlinePiecesCanvas(GameObject newsHeadlinePiece)
        {

        }

        public Vector2[] GetPiecesCoordinates(NewsType newsType)
        { 

            return _newsPiecesCoordinates[newsType];
        }
    }
}
