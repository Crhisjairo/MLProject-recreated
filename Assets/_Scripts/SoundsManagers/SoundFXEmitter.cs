using System;
using _Scripts.Utils;
using UnityEngine;
using UnityEngine.Audio;

namespace _Scripts.SoundsManagers
{
    public class SoundFXEmitter : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup _mixerGroup;
        [SerializeField] private Sound[] _sounds;

        private void Awake()
        {
            if (_sounds.Length == 0)
                return;
        
            foreach (var sound in _sounds)
            {
                sound.SetAudioSource(gameObject.AddComponent<AudioSource>(), _mixerGroup);
            }
        }

        public void Play(string name, bool oneShoot = false)
        {
            Sound sound = Array.Find(_sounds, sound => sound.name == name);

            if (sound is null)
            {
                Debug.LogWarning(ConsoleMessages.SoundNameNotFound);
                return;
            }

            if (oneShoot)
            {
                sound.Source.PlayOneShot(sound.clip);
            }
            else
            {
                sound.Source.Play();
            }
        
        }

        public void PlayFirst()
        {
            Sound sound = _sounds[0];
            
            sound.Source.PlayOneShot(sound.clip);
        }
    }
}
