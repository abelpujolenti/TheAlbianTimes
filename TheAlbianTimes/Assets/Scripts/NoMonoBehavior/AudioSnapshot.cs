using System;
using UnityEngine;

namespace NoMonoBehavior
{
    [Serializable]
    public class AudioSnapshot
    {
        [Range(-80, 0)] 
        [SerializeField] public float masterAudioVolumeValue;
        
        [Range(-80, 0)] 
        [SerializeField] public float SFXAudioVolumeValue;
        
        [Range(-80, 0)] 
        [SerializeField] public float musicAudioVolumeValue;
    }
}