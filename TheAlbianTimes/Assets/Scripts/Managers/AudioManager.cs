using System;
using System.Collections;
using System.Collections.Generic;
using NoMonoBehavior;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace Managers
{
    public enum AudioSnapshots
    {   
        MENU,
        WORKSPACE,
        TRANSITION
    }
    
    public enum AudioGroups
    {
        MASTER,
        SFX,
        MUSIC,
        SIZE
    }

    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;
        public static AudioManager Instance => _instance;
        
        private const String PLAYER_PREFS_MASTER_VOLUME_VALUE = "Player Prefs Master Volume Value";
        private const String PLAYER_PREFS_SFX_VOLUME_VALUE = "Player Prefs SFX Volume Value";
        private const String PLAYER_PREFS_MUSIC_VOLUME_VALUE = "Player Prefs Music Volume Value";
        
        private const String PLAYER_PREFS_MASTER_MUTE = "Player Prefs Master Mute";
        private const String PLAYER_PREFS_SFX_MUTE = "Player Prefs SFX Mute";
        private const String PLAYER_PREFS_MUSIC_MUTE = "Player Prefs Music Mute";

        private const int NUM_2D_SFX_AUDIO_SOURCES = 6;
        private const int NUM_2D_MUSIC_AUDIO_SOURCES = 3;
        private const int NUM_3D_SFX_AUDIO_SOURCES = 8;
        private const int NUM_3D_MUSIC_AUDIO_SOURCES = 3;
        private const int MAX_AUDIO_VOLUME_VALUE = 1;
        private const int AUDIO_MUTE_VALUE = 0;

        private Dictionary<AudioSnapshots, AudioSnapshot> _audioSnapshots;
        
        [SerializeField] private AudioSnapshot _menuSnapshot;
        [SerializeField] private AudioSnapshot _workspaceSnapshot;
        [SerializeField] private AudioSnapshot _transitionSnapshot;
        
        [SerializeField] private Sound[] _sounds;

        private Dictionary<AudioGroups, float> _currentAudioSnapshotValues;
        private Dictionary<AudioGroups, float> _groupAudioVolumeValues;
        private Dictionary<AudioGroups, bool> _groupAudioMutes;
        private Dictionary<AudioGroups, AudioSource[]> _audioSourcesInGroups;
        private Dictionary<AudioGroups, Func<float>> _groupHierarchyAudioVolumeValues;
        private Dictionary<string, Sound> _soundsDictionary = new Dictionary<string, Sound>();
        private Dictionary<AudioGroups, Action<Sound>> _play2DSoundFunctions;
        private Dictionary<AudioGroups, Func<Sound, int>> _play2DLoopSoundFunctions;
        private Dictionary<AudioGroups, Action<Sound, float, float, float, float>> _play2DRandomSoundFunctions;
        private Dictionary<AudioGroups, Action<Sound, float, float, Vector2, float>> _play3DSoundFunctions;
        private Dictionary<AudioGroups, Func<Sound, float, float, Vector2, float, int>> _play3DLoopSoundFunctions;
        private Dictionary<AudioGroups, Action<Sound, float, float, float, float, float, float, Vector2, float>> _play3DRandomSoundFunctions;
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

        private Coroutine _transitionAudioSnapshot;

        private float _masterAudioVolumeValue;
        private float _SFXAudioVolumeValue;
        private float _musicAudioVolumeValue;

        private bool _masterAudioMute;
        private bool _SFXAudioMute;
        private bool _musicAudioMute;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                
                InitializeAudioSourcesArrays();
                
                FillAudioDictionaries();
                
                InitializeAudioValues();

                DontDestroyOnLoad(gameObject);
                
                return;
            }
            Destroy(gameObject);
        }

        private void InitializeAudioValues()
        {
            SetMasterVolumeValue(PlayerPrefs.GetFloat(PLAYER_PREFS_MASTER_VOLUME_VALUE));
            SetMasterMute(PlayerPrefs.GetInt(PLAYER_PREFS_MASTER_MUTE) != 0);
            
            SetInitialVolumeValue(AudioGroups.SFX, 
                PlayerPrefs.GetFloat(PLAYER_PREFS_SFX_VOLUME_VALUE), PlayerPrefs.GetInt(PLAYER_PREFS_SFX_MUTE) != 0);
            SetInitialVolumeValue(AudioGroups.MUSIC, 
                PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME_VALUE), PlayerPrefs.GetInt(PLAYER_PREFS_MUSIC_MUTE) != 0);
        }

        private void InitializeAudioSourcesArrays()
        {
            _2DSFXAudioSources = new AudioSource[NUM_2D_SFX_AUDIO_SOURCES];
            
            _3DSFXAudioSources = new AudioSource[NUM_3D_SFX_AUDIO_SOURCES];
            _3DSFXAudioLowPassFilters = new AudioLowPassFilter[NUM_3D_SFX_AUDIO_SOURCES];
            
            _3DSFXGameObjects = new GameObject[NUM_3D_SFX_AUDIO_SOURCES];
            
            Initialize2DAudioSourcesArrays(NUM_2D_SFX_AUDIO_SOURCES, _2DSFXAudioSources);
            Initialize3DAudioSourcesArrays(NUM_3D_SFX_AUDIO_SOURCES, _3DSFXAudioSources, _3DSFXGameObjects, 
                _3DSFXAudioLowPassFilters);
            
            _2DMusicAudioSources = new AudioSource[NUM_2D_MUSIC_AUDIO_SOURCES];
            
            _3DMusicAudioSources = new AudioSource[NUM_3D_MUSIC_AUDIO_SOURCES];
            _3DMusicAudioLowPassFilters = new AudioLowPassFilter[NUM_3D_MUSIC_AUDIO_SOURCES];
            
            _3DMusicGameObjects = new GameObject[NUM_3D_MUSIC_AUDIO_SOURCES];
            
            Initialize2DAudioSourcesArrays(NUM_2D_MUSIC_AUDIO_SOURCES, _2DMusicAudioSources);
            Initialize3DAudioSourcesArrays(NUM_3D_MUSIC_AUDIO_SOURCES, _3DMusicAudioSources, _3DMusicGameObjects, 
                _3DMusicAudioLowPassFilters);

            List<AudioSource> sfxAudioSources = new List<AudioSource>();
            sfxAudioSources.AddRange(_2DSFXAudioSources);
            sfxAudioSources.AddRange(_3DSFXAudioSources);

            List<AudioSource> musicAudioSources = new List<AudioSource>();
            musicAudioSources.AddRange(_2DMusicAudioSources);
            musicAudioSources.AddRange(_3DMusicAudioSources);

            _audioSourcesInGroups = new Dictionary<AudioGroups, AudioSource[]>
            {
                { AudioGroups.SFX , sfxAudioSources.ToArray()},
                { AudioGroups.MUSIC , musicAudioSources.ToArray()},
            };
        }

        private void Initialize2DAudioSourcesArrays(int numAudioSources, AudioSource[] audioSources)
        {
            
            for (int i = 0; i < numAudioSources; i++)
            {
                audioSources[i] = gameObject.AddComponent<AudioSource>();
            }
        }
        
        private void Initialize3DAudioSourcesArrays(int numAudioSources, AudioSource[] audioSources,
            GameObject[] gameObjects, AudioLowPassFilter[] audioLowPassFilters)
        {
            for (int i = 0; i < numAudioSources; i++)
            {
                GameObject obj = new GameObject();
                obj.transform.SetParent(transform);
                gameObjects[i] = obj;
                audioSources[i] = obj.AddComponent<AudioSource>();
                audioSources[i].spatialBlend = 1;
                audioLowPassFilters[i] = obj.AddComponent<AudioLowPassFilter>();
                audioLowPassFilters[i].cutoffFrequency = 22000;
            }
        }

        private void FillAudioDictionaries()
        {
            _currentAudioSnapshotValues = new Dictionary<AudioGroups, float>
            {
                { AudioGroups.MASTER, MapAudioSnapshotToAudioGroup(_menuSnapshot.masterAudioVolumeValue)},
                { AudioGroups.SFX, MapAudioSnapshotToAudioGroup(_menuSnapshot.SFXAudioVolumeValue)},
                { AudioGroups.MUSIC, MapAudioSnapshotToAudioGroup(_menuSnapshot.musicAudioVolumeValue)},
            };
            
            _groupAudioVolumeValues = new Dictionary<AudioGroups, float>
            {
                {AudioGroups.MASTER, _masterAudioVolumeValue},  
                {AudioGroups.SFX, _SFXAudioVolumeValue},  
                {AudioGroups.MUSIC, _musicAudioVolumeValue}  
            };
            
            _groupAudioMutes = new Dictionary<AudioGroups, bool>
            {
                {AudioGroups.MASTER, _masterAudioMute},  
                {AudioGroups.SFX, _SFXAudioMute},  
                {AudioGroups.MUSIC, _musicAudioMute}  
            };

            _groupHierarchyAudioVolumeValues = new Dictionary<AudioGroups, Func<float>>
            {
                { AudioGroups.SFX , GetSFXHierarchyAudioVolumeValue},
                { AudioGroups.MUSIC , GetMusicHierarchyAudioVolumeValue}
            };
            
            _audioSnapshots = new Dictionary<AudioSnapshots, AudioSnapshot>
            {
                { AudioSnapshots.MENU, _menuSnapshot },
                { AudioSnapshots.WORKSPACE, _workspaceSnapshot },
                { AudioSnapshots.TRANSITION, _transitionSnapshot }
            };
            
            foreach (Sound sound in _sounds)
            {
                _soundsDictionary.Add(sound.GetName(), sound);
            }

            _play2DSoundFunctions = new Dictionary<AudioGroups, Action<Sound>>
            {
                {AudioGroups.SFX, (sound) => Play2DAudioOnAudioSource(_2DSFXAudioSources, sound)},
                {AudioGroups.MUSIC, (sound) => Play2DAudioOnAudioSource(_2DMusicAudioSources, sound)}
            };

            _play2DLoopSoundFunctions = new Dictionary<AudioGroups, Func<Sound, int>>
            {
                {AudioGroups.SFX, (sound) => Play2DLoopAudioOnAudioSource(_2DSFXAudioSources, sound)},
                {AudioGroups.MUSIC, (sound) => Play2DLoopAudioOnAudioSource(_2DMusicAudioSources, sound)}
            };
            
            _play2DRandomSoundFunctions = new Dictionary<AudioGroups, Action<Sound, float, float, float, float>>
            {
                {AudioGroups.SFX, (sound, minVolume, maxVolume, minPitch, maxPitch) => 
                    Play2DRandomAudioOnAudioSource(_2DSFXAudioSources, sound, minVolume, maxVolume, minPitch, maxPitch)},
                
                {AudioGroups.MUSIC, (sound, minVolume, maxVolume, minPitch, maxPitch) => 
                    Play2DRandomAudioOnAudioSource(_2DMusicAudioSources, sound, minVolume, maxVolume, minPitch, maxPitch)}
            };

            _play3DSoundFunctions = new Dictionary<AudioGroups, Action<Sound, float, float, Vector2, float>>
            {
                {AudioGroups.SFX, (sound, minDistance, maxDistance, position, lowPassCutoff) => 
                    Play3DAudioOnAudioSource(_3DSFXAudioSources, _3DSFXAudioLowPassFilters, sound, minDistance, maxDistance, position, lowPassCutoff)},
                
                {AudioGroups.MUSIC, (sound, minDistance, maxDistance, position, lowPassCutoff) => 
                    Play3DAudioOnAudioSource(_3DMusicAudioSources, _3DMusicAudioLowPassFilters, sound, minDistance, maxDistance, position, lowPassCutoff)}
            };

            _play3DLoopSoundFunctions = new Dictionary<AudioGroups, Func<Sound, float, float, Vector2, float, int>>
            {
                {AudioGroups.SFX, (sound, minDistance, maxDistance, position, lowPassCutoff) => 
                    Play3DLoopAudioOnAudioSource(_3DSFXAudioSources, _3DSFXAudioLowPassFilters, sound, minDistance, maxDistance, position, lowPassCutoff)},
                
                {AudioGroups.MUSIC, (sound, minDistance, maxDistance, position, lowPassCutoff) => 
                    Play3DLoopAudioOnAudioSource(_3DMusicAudioSources, _3DMusicAudioLowPassFilters, sound, minDistance, maxDistance, position, lowPassCutoff)}
            };

            _play3DRandomSoundFunctions = new Dictionary<AudioGroups, Action<Sound, float, float, float, float, float, float, Vector2, float>>
            {
                {AudioGroups.SFX, (sound, minDistance, maxDistance, minVolume, maxVolume, minPitch, maxPitch, position, lowPassCutoff) => 
                    Play3DRandomAudioOnAudioSource(_3DSFXAudioSources, _3DSFXAudioLowPassFilters, sound, 
                        minDistance, maxDistance, minVolume, maxVolume, minPitch, maxPitch, position, lowPassCutoff)},
                
                {AudioGroups.MUSIC, (sound, minDistance, maxDistance, minVolume, maxVolume, minPitch, maxPitch, position, lowPassCutoff) => 
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

        private float GetSFXHierarchyAudioVolumeValue()
        {
            return _groupAudioVolumeValues[AudioGroups.MASTER] * _groupAudioVolumeValues[AudioGroups.SFX];
        }

        private float GetMusicHierarchyAudioVolumeValue()
        {
            return _groupAudioVolumeValues[AudioGroups.MASTER] * _groupAudioVolumeValues[AudioGroups.MUSIC];
        }

        private void SetInitialVolumeValue(AudioGroups audioGroup, float volumeValue, bool volumeMute)
        {
            SetVolumeValue(audioGroup, volumeValue);
            SetMute(audioGroup, volumeMute);
        }

        public void ChangeAudioSnapshot(AudioSnapshots audioSnapshot, float timeToTransition)
        {
            if (_transitionAudioSnapshot != null)
            {
                StopCoroutine(_transitionAudioSnapshot);
            }
            _transitionAudioSnapshot = StartCoroutine(TransitionBetweenAudioSnapshots(audioSnapshot, timeToTransition));
        }

        private IEnumerator TransitionBetweenAudioSnapshots(AudioSnapshots audioSnapshot, float timeToTransition)
        {
            int audioGroupsCount = (int)AudioGroups.SIZE;
            
            float[] currentAudioSnapshotAudioVolumesValue = new float[audioGroupsCount];

            for (int i = 0; i < currentAudioSnapshotAudioVolumesValue.Length; i++)
            {
                currentAudioSnapshotAudioVolumesValue[i] = _currentAudioSnapshotValues[(AudioGroups)i];
            }
            
            float[] newAudioSnapshotAudioVolumesValue = new float[audioGroupsCount];

            newAudioSnapshotAudioVolumesValue[0] = MapAudioSnapshotToAudioGroup(_audioSnapshots[audioSnapshot].masterAudioVolumeValue);
            newAudioSnapshotAudioVolumesValue[1] = MapAudioSnapshotToAudioGroup(_audioSnapshots[audioSnapshot].SFXAudioVolumeValue);
            newAudioSnapshotAudioVolumesValue[2] = MapAudioSnapshotToAudioGroup(_audioSnapshots[audioSnapshot].musicAudioVolumeValue);

            float time = 0;

            while (time <= timeToTransition)
            {
                time += Time.deltaTime;

                for (int i = 0; i < audioGroupsCount; i++)
                {
                    AudioGroups audioGroup = (AudioGroups)i;
                    _currentAudioSnapshotValues[audioGroup] = Mathf.Lerp(
                        currentAudioSnapshotAudioVolumesValue[i], newAudioSnapshotAudioVolumesValue[i], time / timeToTransition);

                    if (audioGroup == AudioGroups.MASTER)
                    {
                        SetMasterVolumeValue(_groupAudioVolumeValues[audioGroup]);
                        continue;
                    }
                    
                    SetVolumeValue(audioGroup, _groupAudioVolumeValues[audioGroup]);
                }

                yield return null;
            }
        }

        public void Play2DSound(string soundName)
        {
            Sound sound = _soundsDictionary[soundName];
            _play2DSoundFunctions[sound.GetAudioGroup()](sound);
        }

        public int Play2DLoopSound(string soundName)
        {
            Sound sound = _soundsDictionary[soundName];
            return _play2DLoopSoundFunctions[sound.GetAudioGroup()](sound);
        }

        public void Play2DRandomSound(string[] soundsNames, float minVolume, float maxVolume, 
            float minPitch, float maxPitch)
        {
            Sound sound = _soundsDictionary[soundsNames[Random.Range(0, soundsNames.Length)]];
            _play2DRandomSoundFunctions[sound.GetAudioGroup()](sound, minVolume, maxVolume, 
                minPitch, maxPitch);
        }

        public void Play3DSound(string soundName, float minDistance, float maxDistance, 
            Vector2 position, float lowPassCutoff = 22000)
        {
            Sound sound = _soundsDictionary[soundName];
            _play3DSoundFunctions[sound.GetAudioGroup()](sound, minDistance, maxDistance, position, lowPassCutoff);
        }

        public int Play3DLoopSound(string soundName, float minDistance, float maxDistance, 
            Vector2 position, float lowPassCutoff = 22000)
        {
            Sound sound = _soundsDictionary[soundName];
            return _play3DLoopSoundFunctions[sound.GetAudioGroup()](sound, minDistance, maxDistance, position, lowPassCutoff);
        }

        public void Play3DRandomSound(string[] soundsNames, float minDistance, float maxDistance, 
            float minVolume, float maxVolume, float minPitch, float maxPitch,
            Vector2 position, float lowPassCutoff = 22000)
        {
            Sound sound = _soundsDictionary[soundsNames[Random.Range(0, soundsNames.Length)]];
            _play3DRandomSoundFunctions[sound.GetAudioGroup()](sound, minDistance, maxDistance, 
                minVolume, maxVolume, minPitch, maxPitch, position, lowPassCutoff);
        }

        public void StopAllAudios()
        {
            for (AudioGroups i = (AudioGroups)1; i < AudioGroups.SIZE; i++)
            {
                StopAudioGroupAudios(i);
            }
        }

        public void StopAudioGroupAudios(AudioGroups audioGroup)
        {
            AudioSource[] audioSources = _audioSourcesInGroups[audioGroup];

            foreach (AudioSource audioSource in audioSources)
            {
                if (audioSource.loop)
                {
                    audioSource.loop = false;
                    foreach (KeyValuePair<int, AudioSource> audioLooping in _audiosLooping)
                    {
                        if (audioLooping.Value != audioSource)
                        {
                            continue;
                        }

                        _audiosLooping.Remove(audioLooping.Key);
                        break;
                    }
                }
                
                audioSource.Stop();
            }
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

            AudioGroups audioGroup = sound.GetAudioGroup();

            audioSource.clip = sound.GetClip();
            audioSource.volume = 
                MapAudioGroupToAudioSource(_currentAudioSnapshotValues[audioGroup] * _groupHierarchyAudioVolumeValues[audioGroup](), sound.GetVolume());
            
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

            AudioGroups audioGroup = sound.GetAudioGroup();

            audioSource.gameObject.transform.position = new Vector3(position.x, position.y, -10);

            audioSource.clip = sound.GetClip();
            audioSource.volume =
                MapAudioGroupToAudioSource(_currentAudioSnapshotValues[audioGroup] * _groupHierarchyAudioVolumeValues[audioGroup](), sound.GetVolume());
                
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

        public void SetMasterVolumeValue(float volumeValue)
        {
            _groupAudioVolumeValues[AudioGroups.MASTER] = volumeValue;

            for (AudioGroups i = (AudioGroups)1; i < AudioGroups.SIZE; i++)
            {
                SetVolumeValue(i, _groupAudioVolumeValues[i]);
            }
        }

        public void SetMasterMute(bool mute)
        {
            _groupAudioMutes[AudioGroups.MASTER] = mute;
            
            for (AudioGroups i = (AudioGroups)1; i < AudioGroups.SIZE; i++)
            {
                if (_groupAudioMutes[i])
                {
                    continue;
                }
                Mute(i, mute);
            }
        }

        public void SetVolumeValue(AudioGroups audioGroup, float volumeValue)
        {
            _groupAudioVolumeValues[audioGroup] = volumeValue;

            AudioSource[] audioSources = _audioSourcesInGroups[audioGroup];

            float resultAudioVolumeValue;
            
            foreach (AudioSource audioSource in audioSources)
            {
                if (!audioSource.isPlaying)
                {
                    continue;
                }
                resultAudioVolumeValue =
                    MapAudioGroupToAudioSource(_currentAudioSnapshotValues[audioGroup] * _groupHierarchyAudioVolumeValues[audioGroup](), 
                        _soundsDictionary[audioSource.clip.name].GetVolume());
                
                audioSource.volume = resultAudioVolumeValue;
            }
        }

        public void SetMute(AudioGroups audioGroup, bool mute)
        {
            _groupAudioMutes[audioGroup] = mute;
            
            if (_groupAudioMutes[AudioGroups.MASTER])
            {
                return;
            }
            
            Mute(audioGroup, mute);
        }

        private void Mute(AudioGroups audioGroup, bool mute)
        {
            AudioSource[] audioSources = _audioSourcesInGroups[audioGroup];

            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.mute = mute;
            }
        }

        public float GetGroupVolumeValue(AudioGroups audioGroup)
        {
            return _groupAudioVolumeValues[audioGroup];
        }

        public bool GetGroupMute(AudioGroups audioGroup)
        {
            return _groupAudioMutes[audioGroup];
        }

        private float MapAudioSnapshotToAudioGroup(float audioSnapshotVolumeValue)
        {
            return MathUtil.Map(audioSnapshotVolumeValue, -80, 0, AUDIO_MUTE_VALUE, MAX_AUDIO_VOLUME_VALUE);
        }

        private float MapAudioGroupToAudioSource(float audioGroupVolumeValue, float audioVolume)
        {
            return MathUtil.Map(audioGroupVolumeValue, AUDIO_MUTE_VALUE, MAX_AUDIO_VOLUME_VALUE, 0, audioVolume);
        }

        private void OnDestroy()
        {
            if (_instance != this)
            {
                return;
            }
            PlayerPrefs.SetFloat(PLAYER_PREFS_MASTER_VOLUME_VALUE, _groupAudioVolumeValues[AudioGroups.MASTER]);
            PlayerPrefs.SetFloat(PLAYER_PREFS_SFX_VOLUME_VALUE, _groupAudioVolumeValues[AudioGroups.SFX]);
            PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME_VALUE, _groupAudioVolumeValues[AudioGroups.MUSIC]);
            
            PlayerPrefs.SetInt(PLAYER_PREFS_MASTER_MUTE, _groupAudioMutes[AudioGroups.MASTER] ? 1 : 0);
            PlayerPrefs.SetInt(PLAYER_PREFS_SFX_MUTE, _groupAudioMutes[AudioGroups.SFX] ? 1 : 0);
            PlayerPrefs.SetInt(PLAYER_PREFS_MUSIC_MUTE, _groupAudioMutes[AudioGroups.MUSIC] ? 1 : 0);
        }
    }
}