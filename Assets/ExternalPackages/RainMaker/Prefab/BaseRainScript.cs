//
// Rain Maker (c) 2015 Digital Ruby, LLC
// http://www.digitalruby.com
//

using UnityEngine;
using UnityEngine.Audio;

namespace ExternalPackages.RainMaker.Prefab
{
    public class BaseRainScript : MonoBehaviour
    {
        [Tooltip("Camera the rain should hover over, defaults to main camera")]
        public Camera Camera;

        [Tooltip("Whether rain should follow the camera. If false, rain must be moved manually and will not follow the camera.")]
        public bool FollowCamera = true;

        [Tooltip("Light rain looping clip")]
        public AudioClip RainSoundLight;

        [Tooltip("AudoMixer used for the rain sound")]
        public AudioMixerGroup RainSoundAudioMixer;

        [Range(0.0f, 1.0f)]
        public float SpatialBlend;
        
        [Tooltip("Intensity of rain (0-1)")]
        [Range(0.0f, 1.0f)]
        public float RainIntensity;

        [Tooltip("Rain particle system")]
        public ParticleSystem RainFallParticleSystem;
        private Renderer RainFallParticleSystemRenderer;

        protected LoopingAudioSource audioSourceRainLight;
        protected Material rainMaterial;

        private float lastRainIntensityValue = -1.0f;
        private float nextWindTime;

        private void Awake()
        {
            RainFallParticleSystemRenderer = RainFallParticleSystem.GetComponent<Renderer>();
        }

        protected virtual void Start()
        {
            audioSourceRainLight = new LoopingAudioSource(this, RainSoundLight, RainSoundAudioMixer, SpatialBlend);
            audioSourceRainLight.Play(1.0f);

            
            ParticleSystem.EmissionModule e = RainFallParticleSystem.emission;
            e.enabled = false;
            Renderer rainRenderer = RainFallParticleSystem.GetComponent<Renderer>();
            rainRenderer.enabled = false;
            rainMaterial = new Material(rainRenderer.material);
            rainMaterial.EnableKeyword("SOFTPARTICLES_OFF");
            rainRenderer.material = rainMaterial;

        }
        
        private void CheckForRainChange()
        {
            if (lastRainIntensityValue != RainIntensity)
            {
                ParticleSystem.EmissionModule e = RainFallParticleSystem.emission;
                e.enabled = RainFallParticleSystemRenderer.enabled = true;
                if (!RainFallParticleSystem.isPlaying)
                {
                    RainFallParticleSystem.Play();
                }
                ParticleSystem.MinMaxCurve rate = e.rateOverTime;
                rate.mode = ParticleSystemCurveMode.Constant;
                rate.constantMin = rate.constantMax = RainFallEmissionRate();
                e.rateOverTime = rate;
                
            }
        }


        protected virtual void Update()
        {

            CheckForRainChange();
            audioSourceRainLight.Update();
        }

        protected virtual float RainFallEmissionRate()
        {
            return (RainFallParticleSystem.main.maxParticles / RainFallParticleSystem.main.startLifetime.constant) * RainIntensity;
        }
    }

    /// <summary>
    /// Provides an easy wrapper to looping audio sources with nice transitions for volume when starting and stopping
    /// </summary>
    public class LoopingAudioSource
    {
        public AudioSource AudioSource { get; private set; }
        public float TargetVolume { get; private set; }

        public LoopingAudioSource(MonoBehaviour script, AudioClip clip, AudioMixerGroup mixer, float spatialBlend)
        {
            AudioSource = script.gameObject.AddComponent<AudioSource>();

            if (mixer != null)
            {
                AudioSource.outputAudioMixerGroup = mixer;
            }

            AudioSource.spatialBlend = spatialBlend;
            AudioSource.loop = true;
            AudioSource.clip = clip;
            AudioSource.playOnAwake = false;
            AudioSource.volume = 0.0f;
            AudioSource.Stop();
            TargetVolume = 1.0f;
        }

        public void Play(float targetVolume)
        {
            if (!AudioSource.isPlaying)
            {
                AudioSource.volume = 0.0f;
                AudioSource.Play();
            }
            TargetVolume = targetVolume;
        }

        public void Stop()
        {
            TargetVolume = 0.0f;
        }

        public void Update()
        {
            if (AudioSource.isPlaying && (AudioSource.volume = Mathf.Lerp(AudioSource.volume, TargetVolume, Time.deltaTime)) == 0.0f)
            {
                AudioSource.Stop();
            }
        }
    }
}