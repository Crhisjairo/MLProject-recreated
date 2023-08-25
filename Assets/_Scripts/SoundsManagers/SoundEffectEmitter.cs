using System;
using UnityEngine;
using UnityEngine.Audio;

namespace _Scripts.SoundsManagers
{
    public class SoundEffectEmitter : MonoBehaviour
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
            /**
            if (_sounds.Length == 1)
            {
                if (oneShoot)
                {
                    _sounds[0].Source.PlayOneShot(_sounds[0].clip);
                }
                else
                {
                    _sounds[0].Source.Play();
                }
            
                return;
            }**/
            
        
            Sound sound = Array.Find(_sounds, sound => sound.name == name);

            if (sound is null)
            {
                Debug.LogWarning("Sound " + name + " no fue encontrado.");
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
    }
}
