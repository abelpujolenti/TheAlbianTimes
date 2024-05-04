using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class InGame : MonoBehaviour
    {
        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private Button _backButton;

        private GameObject _currentActivePanel;

        private bool _paused;

        private void Start()
        {
            _currentActivePanel = _mainMenu;
            DontDestroyOnLoad(gameObject);
        }

        public void PauseButton()
        {
            _paused = true;
            _mainMenu.SetActive(true);
            //Time.timeScale = 0;
        }

        public void ResumeButton()
        {
            _paused = false;
            ChangeCurrentActivePanel(_mainMenu);
            _mainMenu.SetActive(false);
            //Time.timeScale = 1;
        }

        public void MainMenuButton()
        {
            //Time.timeScale = 1;
            ResumeButton();
            AudioManager.Instance.StopLoopingAudio(GameManager.Instance.musicAudioId);
            GameManager.Instance.musicAudioId = -1;
            Destroy(gameObject);
            GameManager.Instance.LoadScene(ScenesName.MAIN_MENU);
        }

        public void ChangeCurrentActivePanel(GameObject panelToActive) 
        {
            _currentActivePanel.SetActive(false);
            panelToActive.SetActive(true);
            _currentActivePanel = panelToActive;
            _backButton.gameObject.SetActive(_currentActivePanel != _mainMenu);
        }

        public void GoBack()
        {
            ChangeCurrentActivePanel(_mainMenu);
        }

        public void OnGUI()
        {
            Event currentEvent = Event.current;
            if (!currentEvent.isKey || currentEvent.type != EventType.KeyDown)
            {
                return;
            }

            if (currentEvent.keyCode != KeyCode.Escape)
            {
                return;
            }

            if (!_paused)
            {
                PauseButton();
                return;
            }
            ResumeButton();
        }
    }
}
