using System;
using Managers;
using UnityEngine;

namespace NoMonoBehavior
{
    [Serializable]
    public class Sound
    {
        [SerializeField] private AudioClip _audioClip;

        [SerializeField] private AudioGroups audioGroup;
        
        [SerializeField] private string _name;
    
        [Range(0, 1)]
        [SerializeField] private float _volume;

        [Range(-3, 3)]
        [SerializeField] private float _pitch;

        public AudioClip GetClip()
        {
            return _audioClip;
        }

        public AudioGroups GetAudioGroup()
        {
            return audioGroup;
        }

        public string GetName()
        {
            return _name;
        }

        public float GetVolume()
        {
            return _volume;
        }

        public float GetPitch()
        {
            return _pitch;
        }
    }
}
