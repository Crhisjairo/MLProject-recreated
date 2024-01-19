using System;
using _Scripts.Shared.Utils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace SoundsManagers._Scripts.SoundsManagers
{
    public class SoundFXEmitter : MonoBehaviour
    {
        [FormerlySerializedAs("_mixerGroup")] [SerializeField] private AudioMixerGroup mixerGroup;
        [FormerlySerializedAs("_sounds")] [SerializeField] private Sound[] sounds;
        
        private AudioSource _currentAudioSource;
        
        private void Awake()
        {
            SetAudios();
        }

        private void SetAudios()
        {
            if (sounds.Length == 0)
                return;

            foreach (var sound in sounds)
            {
                sound.SetAudioSource(gameObject.AddComponent<AudioSource>(), mixerGroup);
            }

            if (sounds[0].playOnAwake)
            {
                _currentAudioSource = sounds[0].Source;
                _currentAudioSource.Play();
            }
        }

        public void PlayOneShot(string name)
        {
            Sound sound = Array.Find(sounds, sound => sound.name == name);
            
            if (sound is null)
            {
                Debug.LogWarning(ConsoleMessages.SoundNameNotFound);
                return;
            }

            sound.Source.PlayOneShot(sound.clip);
        }

        public void PlayFirstOneShot()
        {
            Sound sound = sounds[0];

            if (sound.Source is null)
                return;
            
            
            sound.Source.PlayOneShot(sound.clip);
        }
    }
}
