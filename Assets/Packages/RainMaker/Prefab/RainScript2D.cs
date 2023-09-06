//
// Rain Maker (c) 2015 Digital Ruby, LLC
// http://www.digitalruby.com
//

using UnityEngine;

namespace DigitalRuby.RainMaker
{
    public class RainScript2D : BaseRainScript
    {
        private float cameraMultiplier = 1.0f;
        private Bounds visibleBounds = new Bounds();
        private float yOffset;
        private float visibleWorldWidth;
        private float initialEmissionRain;
        private Vector2 initialStartSpeedRain;
        private Vector2 initialStartSizeRain;
        private readonly ParticleSystem.Particle[] particles = new ParticleSystem.Particle[2048];

        [Tooltip("The starting y offset for rain and mist. This will be offset as a percentage of visible height from the top of the visible world.")]
        public float RainHeightMultiplier = 0.15f;

        [Tooltip("The total width of the rain and mist as a percentage of visible width")]
        public float RainWidthMultiplier = 1.5f;

        private void TransformParticleSystem(ParticleSystem p, Vector2 initialStartSpeed, Vector2 initialStartSize)
        {
            p.transform.position = new Vector3(Camera.transform.position.x, visibleBounds.max.y + yOffset, p.transform.position.z);
            
            p.transform.localScale = new Vector3(visibleWorldWidth * RainWidthMultiplier, 1.0f, 1.0f);
            var m = p.main;
            var speed = m.startSpeed;
            var size = m.startSize;
            speed.constantMin = initialStartSpeed.x * cameraMultiplier;
            speed.constantMax = initialStartSpeed.y * cameraMultiplier;
            size.constantMin = initialStartSize.x * cameraMultiplier;
            size.constantMax = initialStartSize.y * cameraMultiplier;
            m.startSpeed = speed;
            m.startSize = size;
        }

        protected override void Start()
        {
            base.Start();

            initialEmissionRain = RainFallParticleSystem.emission.rateOverTime.constant;
            initialStartSpeedRain = new Vector2(RainFallParticleSystem.main.startSpeed.constantMin, RainFallParticleSystem.main.startSpeed.constantMax);
            initialStartSizeRain = new Vector2(RainFallParticleSystem.main.startSize.constantMin, RainFallParticleSystem.main.startSize.constantMax);

        }

        protected override void Update()
        {
            base.Update();

            cameraMultiplier = (Camera.orthographicSize * 0.25f);
            visibleBounds.min = Camera.main.ViewportToWorldPoint(Vector3.zero);
            visibleBounds.max = Camera.main.ViewportToWorldPoint(Vector3.one);
            visibleWorldWidth = visibleBounds.size.x;
            yOffset = (visibleBounds.max.y - visibleBounds.min.y) * RainHeightMultiplier;

            TransformParticleSystem(RainFallParticleSystem, initialStartSpeedRain, initialStartSizeRain);
        }


    }
}