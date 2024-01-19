using System.Collections;
using SoundsManagers._Scripts.SoundsManagers;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace _Scripts.Ambient
{
    [RequireComponent(typeof(SoundFXEmitter))]
    public class ThunderLights : MonoBehaviour
    {
        private const int MinProbability = 0, MaxProbability = 100;
        [Range(MinProbability, MaxProbability)] public int ThunderProbability = MaxProbability;

        private readonly float _randomNumberGeneratorInterval = 1f;
        private float _normalIntensity;
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
            _normalIntensity = _light2D.intensity;
            
            StartCoroutine(StartAnimation());
        }


        private IEnumerator StartAnimation()
        {
            while (true)
            {
                int prob = Random.Range(MinProbability, MaxProbability);

                yield return new WaitForSeconds(_randomNumberGeneratorInterval);

                if (prob <= ThunderProbability)
                {
                    _soundFXEmitter.PlayFirstOneShot();
                    
                    LeanTween.value(_light2D.gameObject, _light2D.intensity, maxIntensity, fadeDuration)
                        .setEaseLinear()
                        .setOnUpdate(intensity =>
                        {
                            _light2D.intensity = intensity;
                        });

                    yield return new WaitForSeconds(fadeDuration);
            
                    LeanTween.value(_light2D.gameObject, _light2D.intensity, _normalIntensity, fadeDuration)
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