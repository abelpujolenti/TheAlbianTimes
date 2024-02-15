using System.Collections.Generic;
using System.Linq;
using Countries;
using UnityEngine;
using UnityEngine.SceneManagement;
using Workspace.Editorial;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;
        
        public GameState gameState;
        private SaveManager saveManager;
        public SceneLoader sceneLoader = new SceneLoader();

        private StatsDisplay _statsDisplay;

        private int _round = 1;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                InitData();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitScenes();
        }

        private void InitData()
        {
            gameState = new GameState();
            saveManager = new SaveManager();
            if (saveManager.SaveFileExists())
            {
                saveManager.LoadFromJson();
            }
            LoadCountries();
            LoadCharacters();
            LoadPlayerData();
            LoadViewedArticles();
        }

        private void InitScenes()
        {
            if (SceneManager.GetSceneByName("WorkspaceScene").isLoaded)
            {
                sceneLoader.SetScene("WorkspaceScene");
                return;
            }
            else if (SceneManager.GetSceneByName("StatsScene").isLoaded)
            {
                sceneLoader.SetScene("StatsScene");
                return;
            }
            sceneLoader.SetScene("MainMenu");
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

        private void LoadCharacters()
        {
            Character[] characterObjects = transform.Find("Characters").GetComponentsInChildren<Character>();
            gameState.characters = new Character[(int)Character.Id.AMOUNT];
            foreach (Character character in characterObjects)
            {
                int index = (int)character.data.characterId;
                if (saveManager.SaveFileExists() && saveManager.save.characterData[index] != null)
                {
                    character.data = saveManager.save.characterData[index];
                }
                gameState.characters[index] = character;
            }
        }

        private void LoadViewedArticles()
        {
            if (saveManager.SaveFileExists())
            {
                gameState.viewedArticles = saveManager.save.viewedArticles.ToHashSet();
                gameState.viewedDialogue = saveManager.save.publishedArticles.ToHashSet();
            }
            else
            {
                gameState.viewedArticles = new HashSet<string>();
                gameState.viewedDialogue = new HashSet<string>();
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

        public void SetStatsDisplay(StatsDisplay statsDisplay)
        {
            _statsDisplay = statsDisplay;
        }

        public void UpdateStatsDisplayMoney(float money)
        {
            _statsDisplay.UpdateMoney(gameState.playerData.money += money);
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

    

   