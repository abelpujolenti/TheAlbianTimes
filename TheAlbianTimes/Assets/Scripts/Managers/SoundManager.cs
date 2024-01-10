using System;
using System.Collections.Generic;
using NoMonoBehavior;
using UnityEngine;
using UnityEngine.Audio;

namespace Managers
{
    public class SoundManager : MonoBehaviour
    {
        private static SoundManager _instance;
        public static SoundManager Instance => _instance;
        
        private const String PLAYERS_PREFS_MASTER_VOLUME_VALUE = "Player Prefs Master Volume Value";
        private const String PLAYERS_PREFS_SFX_VOLUME_VALUE = "Player Prefs SFX Volume Value";
        private const String PLAYERS_PREFS_MUSIC_VOLUME_VALUE = "Player Prefs Music Volume Value";
        
        private const String PLAYERS_PREFS_MASTER_MUTE = "Player Prefs Master Mute";
        private const String PLAYERS_PREFS_SFX_MUTE = "Player Prefs SFX Mute";
        private const String PLAYERS_PREFS_MUSIC_MUTE = "Player Prefs Music Mute";

        private const String AMBIENT = "Ambiente";
        private const String BOTON_MENU = "BotonMenu";

        private const int MUTE_VOLUME_VALUE = -80;

        [SerializeField] private AudioMixer _audioMixer;
        
        [SerializeField] private String _masterVolumeMixer;
        [SerializeField] private String _SFXVolumeMixer;
        [SerializeField] private String _musicVolumeMixer;
        
        [SerializeField] private Sound[] _sounds;

        private Dictionary<String, Sound> _soundsDictionary;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                
                FillSoundsDictionary();
                
                if (!PlayerPrefs.HasKey(PLAYERS_PREFS_MASTER_VOLUME_VALUE))
                {
                    PlayerPrefs.SetFloat(PLAYERS_PREFS_MASTER_VOLUME_VALUE, 1);
                    PlayerPrefs.SetFloat(PLAYERS_PREFS_SFX_VOLUME_VALUE, 1);
                    PlayerPrefs.SetFloat(PLAYERS_PREFS_MUSIC_VOLUME_VALUE, 1);
                }
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(_instance);
            }
        }

        private void FillSoundsDictionary()
        {
            _soundsDictionary = new Dictionary<string, Sound>();
            foreach (Sound sound in _sounds)
            {
                _soundsDictionary.Add(sound.GetName(), sound);
            }
        }

        private void Start()
        {
            SetInitialVolumeValue(PLAYERS_PREFS_MASTER_MUTE, PLAYERS_PREFS_MASTER_VOLUME_VALUE, _masterVolumeMixer);
            SetInitialVolumeValue(PLAYERS_PREFS_SFX_MUTE, PLAYERS_PREFS_SFX_VOLUME_VALUE, _SFXVolumeMixer);
            SetInitialVolumeValue(PLAYERS_PREFS_MUSIC_MUTE, PLAYERS_PREFS_MUSIC_VOLUME_VALUE, _musicVolumeMixer);
        }

        private void SetInitialVolumeValue(String volumeMuteName, String volumeValueName, String mixerGroupName)
        {
            float volumeValue = PlayerPrefs.GetInt(volumeMuteName) == 1 
                ? MUTE_VOLUME_VALUE 
                : PlayerPrefs.GetFloat(volumeValueName);
            _audioMixer.SetFloat(mixerGroupName, volumeValue);
        }

        public void SetMultipleAudioSourcesComponents((AudioSource, String)[] tuples)
        {
            foreach ((AudioSource, String) tuple in tuples)
            {
                SetAudioSourceComponent(tuple.Item1, tuple.Item2);
            }
        }

        private void SetAudioSourceComponent(AudioSource audioSource, string name)
        {
            Sound sound = _soundsDictionary[name];
            
            sound.SetAudioSource(audioSource);
            sound.GetAudioSource().outputAudioMixerGroup = sound.GetAudioMixerGroup();
            sound.GetAudioSource().clip = sound.GetClip();
            sound.GetAudioSource().volume = sound.GetVolume();
            sound.GetAudioSource().loop = sound.GetLoop();
            sound.GetAudioSource().spatialBlend = Convert.ToSingle(sound.GetSound3D());
            if (sound.GetSound3D())
            {
                sound.GetAudioSource().rolloffMode = AudioRolloffMode.Linear;
                sound.GetAudioSource().minDistance = 0;
                sound.GetAudioSource().maxDistance = 30;    
            }

            sound.GetAudioSource().playOnAwake = false;

            if (sound.GetPlay())
            {
                sound.GetAudioSource().Play();
            }
        }

        public void SetVolumePrefs(String playerPrefsVolumeName, float volumeValue)
        {
            PlayerPrefs.SetFloat(playerPrefsVolumeName, volumeValue);

            String audioMixerGroupName;

            switch (playerPrefsVolumeName)
            {
                case PLAYERS_PREFS_MASTER_VOLUME_VALUE:
                    audioMixerGroupName = _masterVolumeMixer;
                    break;
                case PLAYERS_PREFS_SFX_VOLUME_VALUE:
                    audioMixerGroupName = _SFXVolumeMixer;
                    break;
                default:
                    audioMixerGroupName = _musicVolumeMixer;
                    break;
            }

            _audioMixer.SetFloat(audioMixerGroupName, volumeValue);
        }

        public void SetVolumeValue(String audioMixerGroupName, float volumeValue)
        {
            _audioMixer.SetFloat(audioMixerGroupName, volumeValue);
        }
    }
}