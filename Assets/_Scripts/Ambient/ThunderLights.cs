using System;
using System.Collections;
using _Scripts.SoundsManagers;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace _Scripts.Ambient
{
    [RequireComponent(typeof(SoundFXEmitter))]
    public class ThunderLights : MonoBehaviour
    {
        private const int _minProbability = 0, _maxProbability = 100;
        [Range(_minProbability, _maxProbability)] public int ThunderProbability = _maxProbability;

        private float randomNumberGeneratorInterval = 1f;
        private float normalIntensity;
        [SerializeField] private float maxIntensity = 1f;
        [SerializeField] private float fadeDuration = 0.2f;
        private SoundFXEmitter _soundFXEmitter;
        
        private Light2D _light2D;
        
        private void Awake()
        {
            SetComponents();
        }
        
        private void SetComponents()
        {
            _light2D = GetComponent<Light2D>();
            _soundFXEmitter = GetComponent<SoundFXEmitter>();
        }
        
        private void Start()
        {
            normalIntensity = _light2D.intensity;
            
            StartCoroutine(StartAnimation());
        }


        private IEnumerator StartAnimation()
        {
            while (true)
            {
                int prob = Random.Range(_minProbability, _maxProbability);
                Debug.Log(prob);
                
                yield return new WaitForSeconds(randomNumberGeneratorInterval);

                if (prob <= ThunderProbability)
                {
                    _soundFXEmitter.PlayFirst();
                    
                    LeanTween.value(_light2D.gameObject, _light2D.intensity, maxIntensity, fadeDuration)
                        .setEaseLinear()
                        .setOnUpdate(intensity =>
                        {
                            _light2D.intensity = intensity;
                        });

                    yield return new WaitForSeconds(fadeDuration);
            
                    LeanTween.value(_light2D.gameObject, _light2D.intensity, normalIntensity, fadeDuration)
                        .setEaseLinear()
                        .setOnUpdate(intensity =>
                        {
                            _light2D.intensity = intensity;
                        });

            
                    yield return new WaitForSeconds(fadeDuration);
                }
            }
        }
    }
}