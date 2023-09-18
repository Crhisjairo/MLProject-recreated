using UnityEngine;
using UnityEngine.Audio;

namespace _Scripts.SoundsManagers
{
    [System.Serializable]
    public class Sound
    {
        public AudioClip clip;
        public string name;

        [Range(0f, 1f)] public float volume = 1f;
        [Range(.1f, 3f)] public float pitch = 1f;
        public float spatialBlend = 0f;
        public bool loop = false;
        public bool playOnAwake = false;

        public AudioSource Source { get; private set; }

        public void SetAudioSource(AudioSource audioSource, AudioMixerGroup mixerGroup)
        {
            Source = audioSource;
            Source.outputAudioMixerGroup = mixerGroup;
        
            Source.clip = clip;
            Source.volume = volume;
            Source.pitch = pitch;
            Source.spatialBlend = spatialBlend;
            Source.loop = loop;
            Source.playOnAwake = playOnAwake;
        }
    }
}