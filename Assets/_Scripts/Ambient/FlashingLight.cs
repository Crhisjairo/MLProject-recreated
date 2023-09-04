using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace _Scripts.Ambient
{
    [RequireComponent(typeof(Light2D))]
    public class FlashingLight : MonoBehaviour
    {
        [SerializeField] private float flashSpeed = 0.1f;
        [SerializeField] private float minIntensity = 0.6f, maxIntensity = 0.8f;

        public bool smoothIntensity = false;
        
        private Light2D _light2D;

        private void Awake()
        {
            SetComponents();
        }

        private void Start()
        {
            StartCoroutine(StartFlashing());
        }

        private void SetComponents()
        {
            _light2D = GetComponent<Light2D>();
        }

        private IEnumerator StartFlashing()
        {
            while (true)
            {
                var startInten = _light2D.intensity;
                var endInten = Random.Range(minIntensity, maxIntensity);

                if (smoothIntensity)
                {
                    LeanTween.value(gameObject, ChangeLigthIntensity, startInten, endInten, flashSpeed / 2);
                }
                else
                {
                    ChangeLigthIntensity(endInten);
                }
                
                yield return new WaitForSeconds(flashSpeed);
            }
        }

        private void ChangeLigthIntensity(float intensity)
        {
            _light2D.intensity = intensity;
        }
    }
}