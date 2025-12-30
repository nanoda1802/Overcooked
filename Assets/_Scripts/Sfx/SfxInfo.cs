using System;
using UnityEngine;
using UnityEngine.Audio;
using SF = UnityEngine.SerializeField;

namespace Sfx
{
    [Serializable]
    public class SfxInfo
    {
        [SF] private AudioClip clip;
        [SF] private AudioMixerGroup mixerGroup;
        [SF] private bool isLoop;
        
        public AudioClip Clip => clip;
        public AudioMixerGroup MixerGroup => mixerGroup;
        public bool IsLoop => isLoop;
        // public bool isFrequent;
        // public GameObject source; 소리가 발생하는 오브제 참조! 특정 오브젝트의 소리만 끄거나 할 때 활용 가능
    }
}