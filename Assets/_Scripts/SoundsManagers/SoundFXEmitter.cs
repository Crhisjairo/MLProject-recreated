using System;
using _Scripts.Utils;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace _Scripts.SoundsManagers
{
    public class SoundFXEmitter : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup _mixerGroup;
        [SerializeField] private Sound[] _sounds;
        
        private AudioSource _currentAudioSource;
        
        private void Awake()
        {
            SetAudios();
        }

        private void SetAudios()
        {
            if (_sounds.Length == 0)
                return;

            foreach (var sound in _sounds)
            {
                sound.SetAudioSource(gameObject.AddComponent<AudioSource>(), _mixerGroup);
            }

            if (_sounds[0].playOnAwake)
            {
                _currentAudioSource = _sounds[0].Source;
                _currentAudioSource.Play();
            }
        }

        public void PlayOneShot(string name)
        {
            Sound sound = Array.Find(_sounds, sound => sound.name == name);
            
            if (sound is null)
            {
                Debug.LogWarning(ConsoleMessages.SoundNameNotFound);
                return;
            }

            sound.Source.PlayOneShot(sound.clip);
        }

        public void PlayFirstOneShot()
        {
            Sound sound = _sounds[0];

            if (sound.Source is null)
                return;
            
            
            sound.Source.PlayOneShot(sound.clip);
        }
    }
}
