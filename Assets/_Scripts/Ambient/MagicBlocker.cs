using System;
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
            _particleSystem.Stop();
        }
    }
}