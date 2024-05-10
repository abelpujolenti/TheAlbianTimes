using System;
using System.Collections.Generic;
using Dialogue;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class Settings : MonoBehaviour
    {
        private const string CLICK_BUTTON_SOUND = "Click Button";
        
        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private Slider _SFXVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;
        //[SerializeField] private Slider _brightnessSlider;

        [SerializeField] private GameObject _masterAudioMuteToggleCheckMark;
        [SerializeField] private GameObject _SFXAudioMuteToggleCheckMark;
        [SerializeField] private GameObject _musicAudioMuteToggleCheckMark;
        [SerializeField] private GameObject _enableTutorialPromptsToggleCheckMark;

        [SerializeField] private Button _lowDialogueSpeedButton;
        [SerializeField] private Button _mediumDialogueSpeedButton;
        [SerializeField] private Button _highDialogueSpeedButton;

        [SerializeField] private bool _isInGame;
        
        private Dictionary<TextDialoguesSpeed, Button> _textDialogueSpeedButtons = new Dictionary<TextDialoguesSpeed, Button>();

        private Button _selectedButton;

        private Color _selectedColor = new Color(0.4811321f, 0.4811321f, 0.4811321f);
        private Color _unselectedColor = new Color(0.1137255f, 0.1137255f, 0.1137255f);

        private void Awake()
        {
            FillDictionaries();
        }

        private void FillDictionaries()
        {
            _textDialogueSpeedButtons = new Dictionary<TextDialoguesSpeed, Button>
            {
                { TextDialoguesSpeed.LOW , _lowDialogueSpeedButton},
                { TextDialoguesSpeed.MEDIUM , _mediumDialogueSpeedButton},
                { TextDialoguesSpeed.HIGH , _highDialogueSpeedButton},
            };
        }

        private void OnEnable()
        {
            AudioManager audioManager = AudioManager.Instance;
            
            _masterVolumeSlider.value = audioManager.GetGroupVolumeValue(AudioGroups.MASTER);
            _SFXVolumeSlider.value = audioManager.GetGroupVolumeValue(AudioGroups.SFX);
            _musicVolumeSlider.value = audioManager.GetGroupVolumeValue(AudioGroups.MUSIC);
            
            _masterAudioMuteToggleCheckMark.SetActive(!audioManager.GetGroupMute(AudioGroups.MASTER));
            
            _SFXAudioMuteToggleCheckMark.SetActive(!audioManager.GetGroupMute(AudioGroups.SFX));
            
            _musicAudioMuteToggleCheckMark.SetActive(!audioManager.GetGroupMute(AudioGroups.MUSIC));

            if (_isInGame)
            {
                return;
            }
            
            ChangeSelectedButton(GameManager.Instance.textDialogueSpeed);
            
            _enableTutorialPromptsToggleCheckMark.SetActive(GameManager.Instance.areTutorialPromptsEnabled);
        }

        public void SetMasterVolume(float volume)
        {
            AudioManager.Instance.SetMasterVolumeValue(volume);
        }
        
        public void SetSFXVolume(float volume)
        {
            SetVolume(volume, AudioGroups.SFX);
        }
        
        public void SetMusicVolume(float volume)
        {
            SetVolume(volume, AudioGroups.MUSIC);
        }

        private void SetVolume(float volume, AudioGroups audioGroup)
        {
            AudioManager.Instance.SetVolumeValue(audioGroup, volume);
        }

        public void MuteMaster()
        {
            PlayClickButtonSound();
            bool isMasterMuted = AudioManager.Instance.GetGroupMute(AudioGroups.MASTER);
            _masterAudioMuteToggleCheckMark.SetActive(isMasterMuted);
            AudioManager.Instance.SetMasterMute(!isMasterMuted);
        }

        public void MuteSFX()
        {
            PlayClickButtonSound();
            bool isSfxMuted = AudioManager.Instance.GetGroupMute(AudioGroups.SFX);
            _SFXAudioMuteToggleCheckMark.SetActive(isSfxMuted);
            Mute(!isSfxMuted, AudioGroups.SFX);
        }

        public void MuteMusic()
        {
            PlayClickButtonSound();
            bool isMusicMuted = AudioManager.Instance.GetGroupMute(AudioGroups.MUSIC);
            _musicAudioMuteToggleCheckMark.SetActive(isMusicMuted);
            Mute(!isMusicMuted, AudioGroups.MUSIC);
        }

        private void Mute(bool mute, AudioGroups audioGroup)
        {
            AudioManager.Instance.SetMute(audioGroup, mute);
        }

        private void ChangeTextDialogueSpeed(TextDialoguesSpeed textDialoguesSpeed)
        {
            PlayClickButtonSound();
            
            ChangeSelectedButton(textDialoguesSpeed);

            GameManager.Instance.textDialogueSpeed = textDialoguesSpeed;
        }

        private void ChangeSelectedButton(TextDialoguesSpeed textDialoguesSpeed)
        {
            if (_selectedButton != null)
            {
                ChangeButtonColor(_selectedButton, _unselectedColor);
            }
            
            _selectedButton = _textDialogueSpeedButtons[textDialoguesSpeed];
            
            ChangeButtonColor(_selectedButton, _selectedColor);
        }

        private void ChangeButtonColor(Button button, Color color)
        {
            ColorBlock colorBlock = button.colors;

            colorBlock.normalColor = color;

            _selectedButton.colors = colorBlock;
        }

        public void LowDialogueSpeed() => ChangeTextDialogueSpeed(TextDialoguesSpeed.LOW);
        public void MediumDialogueSpeed() => ChangeTextDialogueSpeed(TextDialoguesSpeed.MEDIUM);
        public void HighDialogueSpeed() => ChangeTextDialogueSpeed(TextDialoguesSpeed.HIGH);

        public void ToggleTutorialPrompts()
        {
            PlayClickButtonSound();
            bool areEnabled = GameManager.Instance.areTutorialPromptsEnabled;
            _enableTutorialPromptsToggleCheckMark.SetActive(!areEnabled);
            GameManager.Instance.areTutorialPromptsEnabled = !areEnabled;
        }

        public void PlayClickButtonSound()
        {
            AudioManager.Instance.Play2DSound(CLICK_BUTTON_SOUND);
        }

        public void ResetProgress()
        {
            Debug.Log("Adri fes-li el aquello a l'arxiu de desat");
        }
    }
}
