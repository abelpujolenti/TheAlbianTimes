using System.Collections.Generic;
using Dialogue;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public enum TextDialoguesSpeed
    {
        LOW,
        MEDIUM,
        HIGH
    }

    public class Settings : MonoBehaviour
    {
        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private Slider _SFXVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;
        //[SerializeField] private Slider _brightnessSlider;

        [SerializeField] private GameObject _masterAudioMuteToggleCheckMark;
        [SerializeField] private GameObject _SFXAudioMuteToggleCheckMark;
        [SerializeField] private GameObject _musicAudioMuteToggleCheckMark;
        [SerializeField] private GameObject _enableTutorialPromptsToggleCheckMark;

        [SerializeField] private float _lowTextDialogueSpeed;
        [SerializeField] private float _mediumTextDialogueSpeed;
        [SerializeField] private float _highTextDialogueSpeed;
        
        private Dictionary<TextDialoguesSpeed, float> _textDialogueSpeeds;

        private void Start()
        {
            FillDictionaries();
        }

        private void FillDictionaries()
        {
            _textDialogueSpeeds = new Dictionary<TextDialoguesSpeed, float>
            {
                { TextDialoguesSpeed.LOW , _lowTextDialogueSpeed},
                { TextDialoguesSpeed.MEDIUM , _mediumTextDialogueSpeed},
                { TextDialoguesSpeed.HIGH , _highTextDialogueSpeed},
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
            
            _enableTutorialPromptsToggleCheckMark.SetActive(GameManager.Instance.areTutorialPromptsEnabled);

            //_brightnessSlider.value = PlayerPrefs.GetFloat(PLAYER_PREFS_BRIGHTNESS);
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
            bool isMasterMuted = AudioManager.Instance.GetGroupMute(AudioGroups.MASTER);
            _masterAudioMuteToggleCheckMark.SetActive(isMasterMuted);
            AudioManager.Instance.SetMasterMute(!isMasterMuted);
        }

        public void MuteSFX()
        {
            bool isSfxMuted = AudioManager.Instance.GetGroupMute(AudioGroups.SFX);
            _SFXAudioMuteToggleCheckMark.SetActive(isSfxMuted);
            Mute(!isSfxMuted, AudioGroups.SFX);
        }

        public void MuteMusic()
        {
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
            TextArchitect.baseSpeed = _textDialogueSpeeds[textDialoguesSpeed];
        }

        public void LowDialogueSpeed() => ChangeTextDialogueSpeed(TextDialoguesSpeed.LOW);
        public void MediumDialogueSpeed() => ChangeTextDialogueSpeed(TextDialoguesSpeed.MEDIUM);
        public void HighDialogueSpeed() => ChangeTextDialogueSpeed(TextDialoguesSpeed.HIGH);

        public void ToggleTutorialPrompts()
        {
            bool areEnabled = GameManager.Instance.areTutorialPromptsEnabled;
            _enableTutorialPromptsToggleCheckMark.SetActive(!areEnabled);
            GameManager.Instance.areTutorialPromptsEnabled = !areEnabled;
        }

        public void ResetProgress()
        {
            Debug.Log("Adri fes-li el aquello a l'arxiu de desat");
        }
    }
}
