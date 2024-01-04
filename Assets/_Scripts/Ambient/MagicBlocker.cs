using System;
using System.Collections;
using UnityEngine;

namespace _Scripts.Ambient
{
    [RequireComponent(typeof(ParticleSystem))]
    public class MagicBlocker : Door
    {
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        protected override void PlayOpenAnimation()
        {
            StartCoroutine(ActiveAnimation());
        }

        private IEnumerator ActiveAnimation()
        {
            yield return new WaitForSecondsRealtime(waitTimeBeforeStartAnimation);
            _particleSystem.Stop();
            yield return new WaitForSecondsRealtime(waitTimeBeforeDisableAnimator);
        }
    }
}