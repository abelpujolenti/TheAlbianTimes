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
    
        private AudioSource oneShotAudioSource;

        [SerializeField] private AudioClip changeBiasSound;
        [SerializeField] private AudioClip dropPaperSound;
        [SerializeField] private AudioClip grabPaperSound;
        [SerializeField] private AudioClip submitPaperSound;
        [SerializeField] private AudioClip dropPieceSound;
        [SerializeField] private AudioClip frictionSound;
        [SerializeField] private AudioClip thudSound;
        [SerializeField] private AudioClip snapPieceSound;
        [SerializeField] private AudioClip grabPieceSound;
        [SerializeField] private AudioClip openDrawerSound;
        [SerializeField] private AudioClip closeDrawerSound;
        [SerializeField] private AudioClip phoneSound;
        [SerializeField] private AudioClip typeSound;
        [SerializeField] private AudioClip introSound;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                
                FillSoundsDictionary();
                
                if (!PlayerPrefs.HasKey(PLAYERS_PREFS_MASTER_VOLUME_VALUE))
                {
                    PlayerPrefs.SetFloat(PLAYERS_PREFS_MASTER_VOLUME_VALUE, -15);
                    PlayerPrefs.SetFloat(PLAYERS_PREFS_SFX_VOLUME_VALUE, -15);
                    PlayerPrefs.SetFloat(PLAYERS_PREFS_MUSIC_VOLUME_VALUE, -15);
                }
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(_instance);
            }
            oneShotAudioSource = gameObject.AddComponent<AudioSource>();
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

        public void SetAudioSourceComponent(AudioSource audioSource, string name)
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

        public void IntroSound()
        {
            oneShotAudioSource.PlayOneShot(introSound);

        }
        public void PhoneSound()
        {
            oneShotAudioSource.PlayOneShot(phoneSound);
        }
        public void TypeSound()
        {
            oneShotAudioSource.PlayOneShot(typeSound, .4f);
        }
        public void ChangeBiasSound()
        {
            oneShotAudioSource.PlayOneShot(changeBiasSound, .6f);
        }
        public void SubmitPaperSound()
        {
            oneShotAudioSource.PlayOneShot(grabPaperSound, .3f);
        }
        public void ReturnPaperSound()
        {
            oneShotAudioSource.PlayOneShot(dropPaperSound, .4f);
            oneShotAudioSource.PlayOneShot(thudSound, .3f);
        }
        public void DropPaperSound()
        {
            oneShotAudioSource.PlayOneShot(dropPaperSound, .2f);
        }
        public void GrabPaperSound()
        {
            oneShotAudioSource.PlayOneShot(dropPaperSound, .1f);
            oneShotAudioSource.PlayOneShot(grabPaperSound, .6f);
        }
        public void DropPieceSound()
        {
            oneShotAudioSource.PlayOneShot(thudSound, .6f);
        }
        public void GrabPieceSound()
        {
            oneShotAudioSource.PlayOneShot(grabPieceSound, .4f);
            oneShotAudioSource.PlayOneShot(thudSound, .3f);
        }
        public void SnapPieceSound()
        {
            oneShotAudioSource.PlayOneShot(snapPieceSound, .7f);
            oneShotAudioSource.PlayOneShot(thudSound, .6f);
        }
        public void OpenDrawerSound()
        {
            oneShotAudioSource.PlayOneShot(openDrawerSound);
        }
        public void CloseDrawerSound()
        {
            oneShotAudioSource.PlayOneShot(closeDrawerSound);
        }
    }
}