using System;
using System.Collections;
using _Scripts.Shared.Utils;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundsManagers._Scripts.SoundsManagers
{
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup mixerGroup;
        [SerializeField] private Sound[] musics;

        [SerializeField] private bool useFadeOnStop;
        public float fadeDuration;
        
        private bool _musicPlayerOnAwake;
        
        private AudioSource _currentAudioSource;

        private const float MinVolume = 0f;
        
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
            if(useFadeOnStop)
                StartCoroutine(StartFade(fadeDuration, MinVolume));
            else
                _currentAudioSource.Stop();
        }
        
        public IEnumerator StartFade(float duration, float targetVolume)
        {
            if(!_currentAudioSource)
                yield break;
            
            float currentTime = 0;
            float start = _currentAudioSource.volume;
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                _currentAudioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
                yield return null;
            }
            _currentAudioSource.Stop();
            
            yield break;
        }
    }
}
