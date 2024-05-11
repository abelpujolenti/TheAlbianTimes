using System.Collections.Generic;
using System.Linq;
using Characters;
using Countries;
using Dialogue;
using UnityEngine;
using Workspace.Editorial;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;

        private const string PLAYER_PREFS_TUTORIAL_PROMPTS_ENABLE = "Player Prefs Tutorial Prompts Enable";
        private const string PLAYER_PREFS_TEXT_DIALOGUE_SPEED = "Player Prefs Text Dialogue Speed";
        
        public GameState gameState;
        public GameState prevGameState;
        private SaveManager saveManager;
        public SceneLoader sceneLoader = new SceneLoader();
        public TextDialoguesSpeed textDialogueSpeed;
        public bool areTutorialPromptsEnabled;

        private StatsDisplay _statsDisplay;

        [SerializeField] private GameObject _audioSpawnerPrefab;

        private GameObject _audioSpawner;

        private bool _isAudioSpawnerActive;

        public int musicAudioId = -1;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                InitData();
                if (!PlayerPrefs.HasKey(PLAYER_PREFS_TEXT_DIALOGUE_SPEED))
                {
                    PlayerPrefs.SetInt(PLAYER_PREFS_TEXT_DIALOGUE_SPEED, 1);
                }

                textDialogueSpeed = (TextDialoguesSpeed)PlayerPrefs.GetInt(PLAYER_PREFS_TEXT_DIALOGUE_SPEED);
                areTutorialPromptsEnabled = PlayerPrefs.GetInt(PLAYER_PREFS_TUTORIAL_PROMPTS_ENABLE) == 1;
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
                gameState.currRound = 1;
                LoadScene(ScenesName.WORKSPACE_SCENE);
                return;
            }
            
            if (currentEvent.keyCode == KeyCode.F2)
            {
                gameState.currRound = 2;
                LoadScene(ScenesName.WORKSPACE_SCENE);
                return;
            }
            
            if (currentEvent.keyCode == KeyCode.F3)
            {
                gameState.currRound = 3;
                LoadScene(ScenesName.WORKSPACE_SCENE);
                return;
            }
            
            if (currentEvent.keyCode == KeyCode.F4)
            {
                gameState.currRound = 4;
                LoadScene(ScenesName.WORKSPACE_SCENE);
                return;
            }
            
            if (currentEvent.keyCode == KeyCode.F5)
            {
                gameState.currRound = 5;
                LoadScene(ScenesName.WORKSPACE_SCENE);
                return;
            }

            if (currentEvent.keyCode != KeyCode.F6)
            {
                return;
            }

            gameState.currRound = 6;
            LoadScene(ScenesName.WORKSPACE_SCENE);
        }

        public void InitData()
        {
            gameState = new GameState();
            saveManager = new SaveManager();
            if (saveManager.SaveFileExists())
            {
                saveManager.LoadFromJson();
                gameState.currRound = saveManager.save.currRound;
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
            _statsDisplay.ReceiveMoney(money);
        }

        public void AddToRound()
        {
            gameState.currRound++;
            prevGameState = gameState.Clone();
            SaveGame();
        }

        public int GetRound()
        {
            return gameState.currRound;
        }

        public void SaveGame()
        {
            saveManager.SaveToJson(prevGameState);
        }

        private void OnDestroy()
        {
            PlayerPrefs.SetInt(PLAYER_PREFS_TEXT_DIALOGUE_SPEED, (int)textDialogueSpeed);
            PlayerPrefs.SetInt(PLAYER_PREFS_TUTORIAL_PROMPTS_ENABLE, areTutorialPromptsEnabled ? 1 : 0);
        }

        public SaveManager GetSaveManager()
        {
            return saveManager;
        }
    }
}

    

   