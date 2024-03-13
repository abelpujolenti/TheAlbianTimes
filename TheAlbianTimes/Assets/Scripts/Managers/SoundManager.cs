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

        private const int NUM_SFX_AUDIO_SOURCES = 6;
        private const int NUM_MUSIC_AUDIO_SOURCES = 3;
        private const int MUTE_VOLUME_VALUE = -80;

        [SerializeField] private AudioMixer _audioMixer;
        
        [SerializeField] private string _masterVolumeMixer;
        [SerializeField] private string _SFXVolumeMixer;
        [SerializeField] private string _musicVolumeMixer;
        
        [SerializeField] private AudioMixerGroup _SFXAudioMixerGroup;
        [SerializeField] private AudioMixerGroup _MusicAudioMixerGroup;
        
        [SerializeField] private Sound[] _sounds;

        private Dictionary<string, Sound> _soundsDictionary;
        private Dictionary<AudioMixerGroup, Action<Sound>> _playSoundFunctions;
        private Dictionary<AudioSource, DateTime> _audioSourcesPlayTimestamps;

        private AudioSource[] _SFXAudioSources;
        private AudioSource[] _MusicAudioSources;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                
                InitializeAudioSourcesArrays();
                
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

        private void InitializeAudioSourcesArrays()
        {
            _SFXAudioSources = new AudioSource[NUM_SFX_AUDIO_SOURCES];
            _MusicAudioSources = new AudioSource[NUM_MUSIC_AUDIO_SOURCES];

            for (int i = 0; i < NUM_SFX_AUDIO_SOURCES; i++)
            {
                _SFXAudioSources[i] = gameObject.AddComponent<AudioSource>();
                _SFXAudioSources[i].outputAudioMixerGroup = _SFXAudioMixerGroup;
            }

            for (int i = 0; i < NUM_MUSIC_AUDIO_SOURCES; i++)
            {
                _MusicAudioSources[i] = gameObject.AddComponent<AudioSource>();
                _MusicAudioSources[i].outputAudioMixerGroup = _MusicAudioMixerGroup;
            }
        }

        private void FillSoundsDictionary()
        {
            _soundsDictionary = new Dictionary<string, Sound>();
            foreach (Sound sound in _sounds)
            {
                _soundsDictionary.Add(sound.GetName(), sound);
            }

            _playSoundFunctions = new Dictionary<AudioMixerGroup, Action<Sound>>
            {
                {_SFXAudioMixerGroup, (sound) => PlayAudioOnOlderOrPausedAudioSource(_SFXAudioSources, sound)},
                {_MusicAudioMixerGroup, (sound) => PlayAudioOnOlderOrPausedAudioSource(_MusicAudioSources, sound)}
            };
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

        public void PlaySound(string soundName)
        {
            Sound sound = _soundsDictionary[soundName];
            _playSoundFunctions[sound.GetAudioMixerGroup()](sound);
        }

        private void PlayAudioOnOlderOrPausedAudioSource(AudioSource[] audioSources, Sound sound)
        {
            AudioSource audioSourceToPlaySound = audioSources[0];

            if (audioSourceToPlaySound.isPlaying)
            {
                AudioSource currentAudioSource;
                
                DateTime oldestAudioSourcePlayTime = _audioSourcesPlayTimestamps[audioSourceToPlaySound];
                DateTime currentAudioSourcePlayTime;

                for (int i = 1; i < audioSources.Length; i++)
                {
                    currentAudioSource = audioSources[i];
                    if (!currentAudioSource.isPlaying)
                    {
                        audioSourceToPlaySound = currentAudioSource;
                        break;
                    }
                    
                    currentAudioSourcePlayTime = _audioSourcesPlayTimestamps[currentAudioSource];
                    if (currentAudioSourcePlayTime < oldestAudioSourcePlayTime)
                    {
                        oldestAudioSourcePlayTime = currentAudioSourcePlayTime;
                    }
                }
            }

            audioSourceToPlaySound.clip = sound.GetClip();
            audioSourceToPlaySound.volume = sound.GetVolume();
            audioSourceToPlaySound.loop = sound.GetLoop();
            audioSourceToPlaySound.Play();
            _audioSourcesPlayTimestamps[audioSourceToPlaySound] = DateTime.Now;
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