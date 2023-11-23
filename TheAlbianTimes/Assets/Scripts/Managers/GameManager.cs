using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;
        
        public GameState gameState;
        private SaveManager saveManager;
        public SceneLoader sceneLoader = new SceneLoader();

        private int _round = 1;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            gameState = new GameState();
            saveManager = new SaveManager();
            if (saveManager.SaveFileExists())
            {
                saveManager.LoadFromJson();
            }
            LoadCountries();
            LoadPlayerData();

            sceneLoader.SetScene("WorkspaceScene");
        }

        private void GenerateCountryEvents()
        {
            foreach (Country country in gameState.countries)
            {
                CountryEventManager.Instance.AddEventToQueue(country.GenerateEvent());
            }
        }

        private void LoadCountries()
        {
            Country[] countryObjects = transform.Find("Countries").GetComponentsInChildren<Country>();
            gameState.countries = new Country[(int)Country.Id.AMOUNT];
            foreach (Country country in countryObjects)
            {
                int index = (int)country.data.countryId;
                if (saveManager.SaveFileExists() && saveManager.save.countryData[index] != null)
                {
                    country.data = saveManager.save.countryData[index];
                }
                gameState.countries[index] = country;
            }
        }

        private void LoadPlayerData()
        {
            if (saveManager.SaveFileExists())
            {
                gameState.playerData = saveManager.save.playerData;
            }
            else
            {
                gameState.playerData = new GameStatePlayerData();
            }
        }

        public void AddToRound()
        {
            _round++;
        }

        public int GetRound()
        {
            return _round;
        }
    }
}

    

   