using System;
using _Scripts.Utils;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace _Scripts.SoundsManagers
{
    public class SoundEmitter : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup _mixerGroup;
        [SerializeField] private Sound[] _sounds;
        
        private AudioSource _currentAudioSource;
        
        private void Awake()
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

        public void SetFirstSound(Sound sound)
        {
            _sounds[0] = sound;
            
        }

        public void RandomizePitchFirstAudio()
        {
            _sounds[0].pitch = Random.Range(_sounds[0].pitch - 0.2f, _sounds[0].pitch + 0.2f);
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
            
            sound.Source.PlayOneShot(sound.clip);
        }

        public void PlayFirst()
        {
            Sound sound = _sounds[0];

            _currentAudioSource = sound.Source;
            sound.Source.Play();
        }
    }
}
