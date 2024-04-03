using System;
using System.Collections.Generic;
using NoMonoBehavior;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Managers
{
    public enum AudioMixerSnapshots
    {   
        MENU,
        WORKSPACE,
        TRANSITION
    }
    
    public enum AudioMixerGroups
    {
        SFX,
        MUSIC
    }

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

        private const int NUM_2D_SFX_AUDIO_SOURCES = 6;
        private const int NUM_2D_MUSIC_AUDIO_SOURCES = 3;
        private const int NUM_3D_SFX_AUDIO_SOURCES = 8;
        private const int NUM_3D_MUSIC_AUDIO_SOURCES = 3;
        private const int MUTE_VOLUME_VALUE = -80;

        [SerializeField] private AudioMixer _audioMixer;

        private Dictionary<AudioMixerSnapshots, AudioMixerSnapshot> _audioMixerSnapshot;
        
        [SerializeField] private AudioMixerSnapshot _menuSnapshot;
        [SerializeField] private AudioMixerSnapshot _workspaceSnapshot;
        [SerializeField] private AudioMixerSnapshot _transitionSnapshot;
        
        [SerializeField] private string _masterVolumeMixer;
        [SerializeField] private string _SFXVolumeMixer;
        [SerializeField] private string _musicVolumeMixer;
        
        [SerializeField] private AudioMixerGroup _SFXAudioMixerGroup;
        [SerializeField] private AudioMixerGroup _MusicAudioMixerGroup;
        
        [SerializeField] private Sound[] _sounds;

        private Dictionary<string, Sound> _soundsDictionary = new Dictionary<string, Sound>();
        private Dictionary<AudioMixerGroups, Action<Sound>> _play2DSoundFunctions;
        private Dictionary<AudioMixerGroups, Func<Sound, int>> _play2DLoopSoundFunctions;
        private Dictionary<AudioMixerGroups, Action<Sound, float, float, float, float>> _play2DRandomSoundFunctions;
        private Dictionary<AudioMixerGroups, Action<Sound, float, float, Vector2, float>> _play3DSoundFunctions;
        private Dictionary<AudioMixerGroups, Func<Sound, float, float, Vector2, float, int>> _play3DLoopSoundFunctions;
        private Dictionary<AudioMixerGroups, Action<Sound, float, float, float, float, float, float, Vector2, float>> _play3DRandomSoundFunctions;
        private Dictionary<AudioSource, DateTime> _audioSourcesPlayTimestamps = new Dictionary<AudioSource, DateTime>();

        private AudioSource[] _2DSFXAudioSources;
        private AudioSource[] _2DMusicAudioSources;
        
        private AudioSource[] _3DSFXAudioSources;
        private AudioSource[] _3DMusicAudioSources;
        
        private AudioLowPassFilter[] _3DSFXAudioLowPassFilters;
        private AudioLowPassFilter[] _3DMusicAudioLowPassFilters;

        private Dictionary<int, AudioSource> _audiosLooping = new Dictionary<int, AudioSource>();
        
        private GameObject[] _3DSFXGameObjects;
        private GameObject[] _3DMusicGameObjects;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                
                InitializeAudioSourcesArrays();
                
                FillAudioFunctionsDictionaries();
                
                /*if (!PlayerPrefs.HasKey(PLAYERS_PREFS_MASTER_VOLUME_VALUE))
                {
                    PlayerPrefs.SetFloat(PLAYERS_PREFS_MASTER_VOLUME_VALUE, 1);
                    PlayerPrefs.SetFloat(PLAYERS_PREFS_SFX_VOLUME_VALUE, 1);
                    PlayerPrefs.SetFloat(PLAYERS_PREFS_MUSIC_VOLUME_VALUE, 1);
                }*/

                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeAudioSourcesArrays()
        {
            _2DSFXAudioSources = new AudioSource[NUM_2D_SFX_AUDIO_SOURCES];
            
            _3DSFXAudioSources = new AudioSource[NUM_3D_SFX_AUDIO_SOURCES];
            _3DSFXAudioLowPassFilters = new AudioLowPassFilter[NUM_3D_SFX_AUDIO_SOURCES];
            
            _3DSFXGameObjects = new GameObject[NUM_3D_SFX_AUDIO_SOURCES];
            
            Initialize2DAudioSourcesArrays(NUM_2D_SFX_AUDIO_SOURCES, _2DSFXAudioSources, _SFXAudioMixerGroup);
            Initialize3DAudioSourcesArrays(NUM_3D_SFX_AUDIO_SOURCES, _3DSFXAudioSources, _3DSFXGameObjects, 
                _3DSFXAudioLowPassFilters, _SFXAudioMixerGroup);
            
            
            _2DMusicAudioSources = new AudioSource[NUM_2D_MUSIC_AUDIO_SOURCES];
            
            _3DMusicAudioSources = new AudioSource[NUM_3D_MUSIC_AUDIO_SOURCES];
            _3DMusicAudioLowPassFilters = new AudioLowPassFilter[NUM_3D_MUSIC_AUDIO_SOURCES];
            
            _3DMusicGameObjects = new GameObject[NUM_3D_MUSIC_AUDIO_SOURCES];
            
            Initialize2DAudioSourcesArrays(NUM_2D_MUSIC_AUDIO_SOURCES, _2DMusicAudioSources, _MusicAudioMixerGroup);
            Initialize3DAudioSourcesArrays(NUM_3D_MUSIC_AUDIO_SOURCES, _3DMusicAudioSources, _3DMusicGameObjects, 
                _3DMusicAudioLowPassFilters, _MusicAudioMixerGroup);
        }

        private void Initialize2DAudioSourcesArrays(int numAudioSources, AudioSource[] audioSources,
            AudioMixerGroup audioMixerGroup)
        {
            for (int i = 0; i < numAudioSources; i++)
            {
                audioSources[i] = gameObject.AddComponent<AudioSource>();
                audioSources[i].outputAudioMixerGroup = audioMixerGroup;
            }
        }
        
        private void Initialize3DAudioSourcesArrays(int numAudioSources, AudioSource[] audioSources,
            GameObject[] gameObjects, AudioLowPassFilter[] audioLowPassFilters, AudioMixerGroup audioMixerGroup)
        {
            for (int i = 0; i < numAudioSources; i++)
            {
                GameObject obj = new GameObject();
                obj.transform.SetParent(transform);
                gameObjects[i] = obj;
                audioSources[i] = obj.AddComponent<AudioSource>();
                audioSources[i].spatialBlend = 1;
                audioSources[i].outputAudioMixerGroup = audioMixerGroup;
                audioLowPassFilters[i] = obj.AddComponent<AudioLowPassFilter>();
                audioLowPassFilters[i].cutoffFrequency = 22000;
            }
        }

        private void FillAudioFunctionsDictionaries()
        {
            _audioMixerSnapshot = new Dictionary<AudioMixerSnapshots, AudioMixerSnapshot>
            {
                { AudioMixerSnapshots.WORKSPACE, _workspaceSnapshot },
                { AudioMixerSnapshots.MENU, _menuSnapshot },
                { AudioMixerSnapshots.TRANSITION, _transitionSnapshot }
            };
            
            foreach (Sound sound in _sounds)
            {
                _soundsDictionary.Add(sound.GetName(), sound);
            }

            _play2DSoundFunctions = new Dictionary<AudioMixerGroups, Action<Sound>>
            {
                {AudioMixerGroups.SFX, (sound) => Play2DAudioOnAudioSource(_2DSFXAudioSources, sound)},
                {AudioMixerGroups.MUSIC, (sound) => Play2DAudioOnAudioSource(_2DMusicAudioSources, sound)}
            };

            _play2DLoopSoundFunctions = new Dictionary<AudioMixerGroups, Func<Sound, int>>
            {
                {AudioMixerGroups.SFX, (sound) => Play2DLoopAudioOnAudioSource(_2DSFXAudioSources, sound)},
                {AudioMixerGroups.MUSIC, (sound) => Play2DLoopAudioOnAudioSource(_2DMusicAudioSources, sound)}
            };
            
            _play2DRandomSoundFunctions = new Dictionary<AudioMixerGroups, Action<Sound, float, float, float, float>>
            {
                {AudioMixerGroups.SFX, (sound, minVolume, maxVolume, minPitch, maxPitch) => 
                    Play2DRandomAudioOnAudioSource(_2DSFXAudioSources, sound, minVolume, maxVolume, minPitch, maxPitch)},
                
                {AudioMixerGroups.MUSIC, (sound, minVolume, maxVolume, minPitch, maxPitch) => 
                    Play2DRandomAudioOnAudioSource(_2DMusicAudioSources, sound, minVolume, maxVolume, minPitch, maxPitch)}
            };

            _play3DSoundFunctions = new Dictionary<AudioMixerGroups, Action<Sound, float, float, Vector2, float>>
            {
                {AudioMixerGroups.SFX, (sound, minDistance, maxDistance, position, lowPassCutoff) => 
                    Play3DAudioOnAudioSource(_3DSFXAudioSources, _3DSFXAudioLowPassFilters, sound, minDistance, maxDistance, position, lowPassCutoff)},
                
                {AudioMixerGroups.MUSIC, (sound, minDistance, maxDistance, position, lowPassCutoff) => 
                    Play3DAudioOnAudioSource(_3DMusicAudioSources, _3DMusicAudioLowPassFilters, sound, minDistance, maxDistance, position, lowPassCutoff)}
            };

            _play3DLoopSoundFunctions = new Dictionary<AudioMixerGroups, Func<Sound, float, float, Vector2, float, int>>
            {
                {AudioMixerGroups.SFX, (sound, minDistance, maxDistance, position, lowPassCutoff) => 
                    Play3DLoopAudioOnAudioSource(_3DSFXAudioSources, _3DSFXAudioLowPassFilters, sound, minDistance, maxDistance, position, lowPassCutoff)},
                
                {AudioMixerGroups.MUSIC, (sound, minDistance, maxDistance, position, lowPassCutoff) => 
                    Play3DLoopAudioOnAudioSource(_3DMusicAudioSources, _3DMusicAudioLowPassFilters, sound, minDistance, maxDistance, position, lowPassCutoff)}
            };

            _play3DRandomSoundFunctions = new Dictionary<AudioMixerGroups, Action<Sound, float, float, float, float, float, float, Vector2, float>>
            {
                {AudioMixerGroups.SFX, (sound, minDistance, maxDistance, minVolume, maxVolume, minPitch, maxPitch, position, lowPassCutoff) => 
                    Play3DRandomAudioOnAudioSource(_3DSFXAudioSources, _3DSFXAudioLowPassFilters, sound, 
                        minDistance, maxDistance, minVolume, maxVolume, minPitch, maxPitch, position, lowPassCutoff)},
                
                {AudioMixerGroups.MUSIC, (sound, minDistance, maxDistance, minVolume, maxVolume, minPitch, maxPitch, position, lowPassCutoff) => 
                    Play3DRandomAudioOnAudioSource(_3DMusicAudioSources, _3DMusicAudioLowPassFilters, sound, 
                        minDistance, maxDistance, minVolume, maxVolume, minPitch, maxPitch, position, lowPassCutoff)}
            };

            foreach (AudioSource audioSource in _2DSFXAudioSources)
            {
                _audioSourcesPlayTimestamps.Add(audioSource, DateTime.Now);
            }
            
            foreach (AudioSource audioSource in _2DMusicAudioSources)
            {
                _audioSourcesPlayTimestamps.Add(audioSource, DateTime.Now);
            }
            
            foreach (AudioSource audioSource in _3DSFXAudioSources)
            {
                _audioSourcesPlayTimestamps.Add(audioSource, DateTime.Now);
            }
            
            foreach (AudioSource audioSource in _3DMusicAudioSources)
            {
                _audioSourcesPlayTimestamps.Add(audioSource, DateTime.Now);
            }
        }

        private void Start()
        {
            /*SetInitialVolumeValue(PLAYERS_PREFS_MASTER_MUTE, PLAYERS_PREFS_MASTER_VOLUME_VALUE, _masterVolumeMixer);
            SetInitialVolumeValue(PLAYERS_PREFS_SFX_MUTE, PLAYERS_PREFS_SFX_VOLUME_VALUE, _SFXVolumeMixer);
            SetInitialVolumeValue(PLAYERS_PREFS_MUSIC_MUTE, PLAYERS_PREFS_MUSIC_VOLUME_VALUE, _musicVolumeMixer);*/
        }

        private void SetInitialVolumeValue(String volumeMuteName, String volumeValueName, String mixerGroupName)
        {
            float volumeValue = PlayerPrefs.GetInt(volumeMuteName) == 1 
                ? MUTE_VOLUME_VALUE 
                : PlayerPrefs.GetFloat(volumeValueName);
            _audioMixer.SetFloat(mixerGroupName, volumeValue);
        }

        public void ChangeAudioMixerSnapshot(AudioMixerSnapshots audioMixerSnapshot, float timeToTransition)
        {
            _audioMixerSnapshot[audioMixerSnapshot].TransitionTo(timeToTransition);
        }

        public void Play2DSound(string soundName)
        {
            Sound sound = _soundsDictionary[soundName];
            _play2DSoundFunctions[sound.GetAudioMixerGroup()](sound);
        }

        public int Play2DLoopSound(string soundName)
        {
            Sound sound = _soundsDictionary[soundName];
            return _play2DLoopSoundFunctions[sound.GetAudioMixerGroup()](sound);
        }

        public void Play2DRandomSound(string[] soundsNames, float minVolume, float maxVolume, 
            float minPitch, float maxPitch)
        {
            Sound sound = _soundsDictionary[soundsNames[Random.Range(0, soundsNames.Length)]];
            _play2DRandomSoundFunctions[sound.GetAudioMixerGroup()](sound, minVolume, maxVolume, 
                minPitch, maxPitch);
        }

        public void Play3DSound(string soundName, float minDistance, float maxDistance, 
            Vector2 position, float lowPassCutoff = 22000)
        {
            Sound sound = _soundsDictionary[soundName];
            _play3DSoundFunctions[sound.GetAudioMixerGroup()](sound, minDistance, maxDistance, position, lowPassCutoff);
        }

        public int Play3DLoopSound(string soundName, float minDistance, float maxDistance, 
            Vector2 position, float lowPassCutoff = 22000)
        {
            Sound sound = _soundsDictionary[soundName];
            return _play3DLoopSoundFunctions[sound.GetAudioMixerGroup()](sound, minDistance, maxDistance, position, lowPassCutoff);
        }

        public void Play3DRandomSound(string[] soundsNames, float minDistance, float maxDistance, 
            float minVolume, float maxVolume, float minPitch, float maxPitch,
            Vector2 position, float lowPassCutoff = 22000)
        {
            Sound sound = _soundsDictionary[soundsNames[Random.Range(0, soundsNames.Length)]];
            _play3DRandomSoundFunctions[sound.GetAudioMixerGroup()](sound, minDistance, maxDistance, 
                minVolume, maxVolume, minPitch, maxPitch, position, lowPassCutoff);
        }

        public void StopLoopingAudio(int id)
        {
            AudioSource audioSource = _audiosLooping[id];
            audioSource.loop = false;
            audioSource.Stop();
            _audiosLooping.Remove(id);
        }

        private AudioSource Play2DAudioOnAudioSource(AudioSource[] audioSources, Sound sound)
        {
            int audioSourceIndex = GetOldestOrPausedAudioSourceIndex(audioSources);
            
            AudioSource audioSource = audioSources[audioSourceIndex];

            audioSource.clip = sound.GetClip();
            audioSource.volume = sound.GetVolume();
            audioSource.pitch = sound.GetPitch();
            _audioSourcesPlayTimestamps[audioSource] = DateTime.Now;
            audioSource.Play();

            return audioSource;
        }

        private int Play2DLoopAudioOnAudioSource(AudioSource[] audioSources, Sound sound)
        {
            AudioSource audioSource = Play2DAudioOnAudioSource(audioSources, sound);

            audioSource.loop = true;

            int randomId = Random.Range(0, 100000);
            
            _audiosLooping.Add(randomId, audioSource);

            return randomId;
        }

        private void Play2DRandomAudioOnAudioSource(AudioSource[] audioSources, Sound sound, 
            float minVolume, float maxVolume, float minPitch, float maxPitch)
        {
            AudioSource audioSource = Play2DAudioOnAudioSource(audioSources, sound);

            audioSource.volume = Random.Range(minVolume, maxVolume);
            audioSource.pitch = Random.Range(minPitch, maxPitch);
        }

        private AudioSource Play3DAudioOnAudioSource(AudioSource[] audioSources, AudioLowPassFilter[] audioLowPassFilters, 
            Sound sound, float minDistance, float maxDistance, Vector2 position, float lowPassCutoff = 22000)
        {
            int audioSourceIndex = GetOldestOrPausedAudioSourceIndex(audioSources);
            
            AudioSource audioSource = audioSources[audioSourceIndex];

            audioSource.gameObject.transform.position = new Vector3(position.x, position.y, -10);

            audioSource.clip = sound.GetClip();
            audioSource.volume = sound.GetVolume();
            audioSource.pitch = sound.GetPitch();
            audioSource.minDistance = minDistance;
            audioSource.maxDistance = maxDistance; 
            audioLowPassFilters[audioSourceIndex].cutoffFrequency = lowPassCutoff;
            _audioSourcesPlayTimestamps[audioSource] = DateTime.Now;
            audioSource.Play();

            return audioSource;
        }

        private int Play3DLoopAudioOnAudioSource(AudioSource[] audioSources, AudioLowPassFilter[] audioLowPassFilters, 
            Sound sound, float minDistance, float maxDistance, Vector2 position, float lowPassCutoff = 22000)
        {
            AudioSource audioSource = Play3DAudioOnAudioSource(audioSources, audioLowPassFilters, sound, 
                minDistance, maxDistance, position, lowPassCutoff);

            audioSource.loop = true;

            int randomId = Random.Range(0, 100000);
            
            _audiosLooping.Add(randomId, audioSource);

            return randomId;
        }

        private void Play3DRandomAudioOnAudioSource(AudioSource[] audioSources, AudioLowPassFilter[] audioLowPassFilters, 
            Sound sound, float minDistance, float maxDistance, float minVolume, float maxVolume, 
            float minPitch, float maxPitch, Vector2 position, float lowPassCutoff = 22000)
        {
            AudioSource audioSource = Play3DAudioOnAudioSource(audioSources, audioLowPassFilters, sound, 
                minDistance, maxDistance, position, lowPassCutoff);

            audioSource.minDistance = minDistance;
            audioSource.maxDistance = maxDistance;
            audioSource.volume = Random.Range(minVolume, maxVolume);
            audioSource.pitch = Random.Range(minPitch, maxPitch);
        }

        private int GetOldestOrPausedAudioSourceIndex(AudioSource[] audioSources)
        {
            int audioSouceIndex = 0;
            
            AudioSource currentAudioSource;
                
            DateTime oldestAudioSourcePlayTime = DateTime.Now;
            DateTime currentAudioSourcePlayTime;

            for (int i = 0; i < audioSources.Length; i++)
            {
                currentAudioSource = audioSources[i];
                if (!currentAudioSource.isPlaying)
                {
                    audioSouceIndex = i;
                    break;
                }

                if (currentAudioSource.loop)
                {
                    continue;
                }
                    
                currentAudioSourcePlayTime = _audioSourcesPlayTimestamps[currentAudioSource];
                if (currentAudioSourcePlayTime >= oldestAudioSourcePlayTime)
                {
                    continue;
                }

                audioSouceIndex = i;
                oldestAudioSourcePlayTime = currentAudioSourcePlayTime;
            }

            return audioSouceIndex;
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