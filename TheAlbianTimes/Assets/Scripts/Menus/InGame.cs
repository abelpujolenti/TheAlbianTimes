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

        private void Start()
        {
            _currentActivePanel = _mainMenu;
        }

        public void PauseButton()
        {
            //Time.timeScale = 0;
        }

        public void ResumeButton()
        {
            //Time.timeScale = 1;
        }

        public void MainMenuButton()
        {
            //Time.timeScale = 1;
            AudioManager.Instance.StopLoopingAudio(GameManager.Instance.roomToneAudioId);
            GameManager.Instance.roomToneAudioId = -1;
            AudioManager.Instance.StopLoopingAudio(GameManager.Instance.musicAudioId);
            GameManager.Instance.musicAudioId = -1;
            //TODO LOAD MAIN MENU
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
    }
}
