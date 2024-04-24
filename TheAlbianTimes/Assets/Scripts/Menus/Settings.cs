using Dialogue;
using Managers;
using System.Collections.Generic;
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

        [SerializeField] private Toggle _masterAudioMuteToggle;
        [SerializeField] private Toggle _SFXAudioMuteToggle;
        [SerializeField] private Toggle _musicAudioMuteToggle;

        [SerializeField] private float _lowTextDialogueSpeed;
        [SerializeField] private float _mediumTextDialogueSpeed;
        [SerializeField] private float _highTextDialogueSpeed;
        
        private Dictionary<AudioGroups, Toggle> _audioMuteToggles;
        private Dictionary<TextDialoguesSpeed, float> _textDialogueSpeeds;

        private void Start()
        {
            FillDictionaries();
        }

        private void FillDictionaries()
        {
            _audioMuteToggles = new Dictionary<AudioGroups, Toggle>
            {
                {AudioGroups.MASTER, _masterAudioMuteToggle},  
                {AudioGroups.SFX, _SFXAudioMuteToggle},  
                {AudioGroups.MUSIC, _musicAudioMuteToggle}  
            };

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

            _masterAudioMuteToggle.isOn = audioManager.GetGroupMute(AudioGroups.MASTER);
            _SFXAudioMuteToggle.isOn = audioManager.GetGroupMute(AudioGroups.SFX);
            _musicAudioMuteToggle.isOn = audioManager.GetGroupMute(AudioGroups.MUSIC);

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

        private void SetMuteToggle(bool mute, AudioGroups audioGroup)
        {
            _audioMuteToggles[audioGroup].isOn = mute;
        }

        public void MuteMaster(bool mute)
        {
            AudioManager.Instance.SetMasterMute(mute);
            _SFXAudioMuteToggle.isOn = mute;
            _musicAudioMuteToggle.isOn = mute;
        }

        public void MuteSFX(bool mute)
        {
            if (_masterAudioMuteToggle.isOn)
            {
                return;
            }
            Mute(mute, AudioGroups.SFX);
        }

        public void MuteMusic(bool mute)
        {
            if (_masterAudioMuteToggle.isOn)
            {
                return;
            }
            Mute(mute, AudioGroups.MUSIC);
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

        public void ResetProgress()
        {
            Debug.Log("Adri fes-li el aquello a l'arxiu de desat");
        }
    }
}
