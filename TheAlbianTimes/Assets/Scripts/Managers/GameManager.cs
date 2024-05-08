using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using Countries;
using UnityEngine;
using UnityEngine.UIElements;
using Workspace.Editorial;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;
        
        public GameState gameState;
        public GameState prevGameState;
        private SaveManager saveManager;
        public SceneLoader sceneLoader = new SceneLoader();

        private StatsDisplay _statsDisplay;

        [SerializeField] private UIDocument _uiDocument;

        [SerializeField] private GameObject _audioSpawnerPrefab;

        private GameObject _audioSpawner;

        private bool _isAudioSpawnerActive;

        public int musicAudioId = -1;
        private int _round = 0;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                InitData();
                DontDestroyOnLoad(gameObject);
                return;
            }
            Destroy(gameObject);
        }

        private void Start()
        {
            InitScenes();
        }

        private void OnGUI()
        {
            Event currentEvent = Event.current;
            if (!currentEvent.isKey || currentEvent.type != EventType.KeyDown) return;

            if (currentEvent.keyCode == KeyCode.F1)
            {
                _round = 1;
                LoadScene(ScenesName.WORKSPACE_SCENE);
                return;
            }
            
            if (currentEvent.keyCode == KeyCode.F2)
            {
                _round = 2;
                LoadScene(ScenesName.WORKSPACE_SCENE);
                return;
            }
            
            if (currentEvent.keyCode == KeyCode.F3)
            {
                _round = 3;
                LoadScene(ScenesName.WORKSPACE_SCENE);
                return;
            }
            
            if (currentEvent.keyCode == KeyCode.F4)
            {
                _round = 4;
                LoadScene(ScenesName.WORKSPACE_SCENE);
                return;
            }
            
            if (currentEvent.keyCode == KeyCode.F5)
            {
                _round = 5;
                LoadScene(ScenesName.WORKSPACE_SCENE);
                return;
            }

            if (currentEvent.keyCode != KeyCode.F6)
            {
                return;
            }

            _round = 6;
            LoadScene(ScenesName.WORKSPACE_SCENE);
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
            prevGameState = gameState.Clone();
        }

        private void InitScenes()
        {
            /*if (SceneManager.GetSceneByBuildIndex((int)ScenesName.WORKSPACE_SCENE).isLoaded)
            {
                LoadScene(ScenesName.WORKSPACE_SCENE);
                return;
            }
            if (SceneManager.GetSceneByBuildIndex((int)ScenesName.STATS_SCENE).isLoaded)
            {
                LoadScene(ScenesName.STATS_SCENE);
                return;
            }
            LoadScene(ScenesName.MAIN_MENU);*/
        }

        public void LoadScene(ScenesName sceneName)
        {
            if (sceneName == ScenesName.WORKSPACE_SCENE)
            {
                _audioSpawner = Instantiate(_audioSpawnerPrefab);
                DontDestroyOnLoad(_audioSpawner);
                _isAudioSpawnerActive = true;
            }

            if ((sceneName == ScenesName.DIALOGUE_SCENE || sceneName == ScenesName.MAIN_MENU) && _isAudioSpawnerActive)
            {
                Destroy(_audioSpawner);
                _isAudioSpawnerActive = false;
            }
            sceneLoader.SetScene(sceneName);
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
                return;
            }
            
            gameState.playerData = new GameStatePlayerData();
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
            prevGameState = gameState.Clone();
            _round++;
        }

        public int GetRound()
        {
            return _round;
        }
    }
}

    

   