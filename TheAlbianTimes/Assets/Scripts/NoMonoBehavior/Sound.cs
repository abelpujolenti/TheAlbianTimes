using System;
using Managers;
using UnityEngine;

namespace NoMonoBehavior
{
    [Serializable]
    public class Sound
    {
        private AudioSource _audioSource;
        [SerializeField] private AudioClip _audioClip;

        [SerializeField] private AudioMixers _audioMixerGroup;
        
        [SerializeField] private string _name;
    
        [Range(0, 1)]
        [SerializeField] private float _volume;
        
        [SerializeField] private bool _loop;
        [SerializeField] private bool _play;
        [SerializeField] private bool _sound3D;


        public AudioSource GetAudioSource()
        {
            return _audioSource;
        }

        public void SetAudioSource(AudioSource source)
        {
            _audioSource = source;
        }

        public AudioClip GetClip()
        {
            return _audioClip;
        }

        public void SetAudioClip(AudioClip audioClip)
        {
            _audioClip = audioClip;
        }

        public void SetAudioMixerGroup(AudioMixers audioMixerGroup)
        {
            _audioMixerGroup = audioMixerGroup;
        }

        public AudioMixers GetAudioMixerGroup()
        {
            return _audioMixerGroup;
        }

        public float GetVolume()
        {
            return _volume;
        }

        public string GetName()
        {
            return _name;
        }

        public bool GetLoop()
        {
            return _loop;
        }

        public bool GetPlay()
        {
            return _play;
        }

        public bool GetSound3D()
        {
            return _sound3D;
        }
    }
}
