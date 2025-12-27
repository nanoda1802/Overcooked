using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Sfx
{
    [Serializable]
    public struct ClipInfo
    {
        public AudioClip clip;
        public AudioMixerGroup mixerGroup;
        public bool isLoop;
        // public bool isFrequent;
        // public GameObject source; 소리가 발생하는 오브제 참조! 특정 오브젝트의 소리만 끄거나 할 때 활용 가능
    }
}