using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public struct SoundDataasdf
{
   public AudioClip clip;
   public AudioMixerGroup mixerGroup;
   public bool loop;
   public bool playOnAwake;

   public bool isFrequent;
}