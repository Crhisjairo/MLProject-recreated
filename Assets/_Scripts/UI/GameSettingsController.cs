using System.Collections.Generic;
using _Scripts.Shared.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace _Scripts.UI
{
    public class GameSettingsController : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown resolutionsDropdown;
        [SerializeField] private Toggle fullScreenToggle, touchControlsToggle, showFpsToggle, verticalSyncToggle;
        [FormerlySerializedAs("generalVolumeSlider")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider, effectsVolumeSlider;

        [SerializeField] private AudioMixer audioMixer;

        [SerializeField] private GameObject fpsGo;
        
        private Resolution[] _resolutionsAvailable;
        
        private void Awake()
        {
            SetPlayformOptions();
            SetDropdownResolutions();
            SetSlidersVolume();
        }

        private void SetPlayformOptions()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                resolutionsDropdown.interactable = false;
                fullScreenToggle.interactable = false;
            }
        }
        
        private void SetDropdownResolutions()
        {
            _resolutionsAvailable = Screen.resolutions;
            List<string> resolutionsOptions = new List<string>();
            int currentResolutionSelectedIndex = 0;
            int resolutionIndex = 0;
            
            resolutionsDropdown.ClearOptions();
            
            foreach (var resolution in _resolutionsAvailable)
            {
                var dropdownOption = 
                    resolution.width + " X " + resolution.height + " | " + resolution.refreshRateRatio + " Hz";
                
                resolutionsOptions.Add(dropdownOption);

                if (resolution.Equals(Screen.currentResolution))
                {
                    currentResolutionSelectedIndex = resolutionIndex;
                }

                resolutionIndex++;
            }
            
            resolutionsDropdown.AddOptions(resolutionsOptions);
            resolutionsDropdown.value = currentResolutionSelectedIndex;
            resolutionsDropdown.RefreshShownValue();
        }

        private void SetSlidersVolume()
        {
            // TODO: maybe change this method when loading logic implemented.
            masterVolumeSlider.value = 1;
            musicVolumeSlider.value = 1;
            effectsVolumeSlider.value = 1;
        }

        public void SetMasterVolume(float volume)
        {
            SetAudioMixerVolume(AudioMixerGroupNames.Master.ToString(), volume);
        }
        
        
        public void SetMusicVolume(float volume)
        {
            SetAudioMixerVolume(AudioMixerGroupNames.Music.ToString(), volume);
        }
        
        public void SetSfxVolume(float volume)
        {
            SetAudioMixerVolume(AudioMixerGroupNames.Sfx.ToString(), volume);
        }
        
        private void SetAudioMixerVolume(string audioMixerGroup, float volume)
        {
            float processedVolume = Mathf.Log10(volume) * 20;

            audioMixer.SetFloat(audioMixerGroup, processedVolume);
        }

        public void SetFullScreen(bool fullScreen)
        {
            Screen.fullScreen = fullScreen;
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = _resolutionsAvailable[resolutionIndex];
            
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRateRatio);
        }

        public void ShowFPS(bool showFps)
        {
            fpsGo.SetActive(showFps);
        }

        public void ShowTouchControls(bool showTouchControls)
        {
            
        }

        public void SetVerticalSync(bool verticalSync)
        {
            QualitySettings.vSyncCount = verticalSync ? 1 : 0;
        }
    }
}
