using System;
using _Scripts.Utils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
namespace _Scripts.SoundsManagers
{
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup mixerGroup;
        [SerializeField] private Sound[] musics;

        private bool _musicPlayerOnAwake;
        
        private AudioSource _currentAudioSource;

        private void Start()
        {
            SetMusicsAudioSources();
            
            foreach (var music in musics)
            {
                if (music.playOnAwake)
                {
                    StartMusic(music.name);
                }
            }
            
        }
        
        private void SetMusicsAudioSources()
        {
            foreach (var sound in musics)
            {
                sound.SetAudioSource(gameObject.AddComponent<AudioSource>(), mixerGroup);
            }
        }

        public void StartMusic(string name)
        {
            Sound music = Array.Find(musics, music => music.name == name);
            
            if (music is null)
            {
                Debug.LogWarning(ConsoleMessages.MusicNameNotFound);
                return;
            }
            
            if(_currentAudioSource is not null)
                _currentAudioSource.Stop();
            
            _currentAudioSource = music.Source;
            
            _currentAudioSource.Play();
        }

        public void ResumeMusic()
        {
            if (_currentAudioSource is null)
                return;
            
            _currentAudioSource.Play();
        }
        
        public void PauseMusic()
        {
            _currentAudioSource.Pause();
        }
        
        public void StopMusic()
        {
            _currentAudioSource.Stop();
        }
    }
}
