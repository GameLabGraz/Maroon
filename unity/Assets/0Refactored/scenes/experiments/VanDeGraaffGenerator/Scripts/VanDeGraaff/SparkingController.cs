using DigitalRuby.ThunderAndLightning;
using UnityEngine;

namespace Maroon.Physics.Electromagnetism.VanDeGraaff
{
    [RequireComponent(typeof(LightningBoltScript))]
    public class SparkingController : PausableObject
    {
        [SerializeField] private Transform sparkingStartPoint;
        [SerializeField] private Transform sparkingEndPoint;
        [SerializeField] private float eMax = 3e06f;

        /// <summary>
        /// The normalization factor to get the distance in meters.
        /// The VdG sphere should have a diameter of 1m. The in-game diameter
        /// is 2.1. So we multiply distances with 0.476 (1/2.1) to get a normalized result.
        /// </summary>
        [SerializeField] private float normalizationFactor = 0.476f;

        [SerializeField] private VanDeGraaffController vandeGraaffController;

        private LightningBoltScript _lightningBolt;
        private AudioSource _sound;

        private float SparkingPointsDistance =>
            Vector3.Distance(sparkingStartPoint.position, sparkingEndPoint.position) * normalizationFactor;

        protected override void Start()
        {
            base.Start();

            if(sparkingStartPoint == null)
                throw new System.Exception("No sparking start point found");

            if (sparkingEndPoint == null)
                throw new System.Exception("No sparking end point found");

            _lightningBolt = GetComponent<LightningBoltScript>();
            _sound = GetComponent<AudioSource>();
        }

        private void GenerateSpark()
        {
            var count = 1;

            var delay = 0.0f;
            var random = new System.Random();
            const float duration = 0.25f;
            var singleDuration = Mathf.Max(1.0f / 30.0f, (duration / (float) count));

            while (count-- > 0)
            {
                _lightningBolt.CreateLightningBolt(new LightningBoltParameters()
                {
                    Start = sparkingStartPoint.position,
                    End = sparkingEndPoint.position,
                    Generations = 6,
                    LifeTime = count == 1 ? singleDuration : singleDuration * ((float)random.NextDouble() * 0.4f) + 0.8f,
                    Delay = delay,
                    ChaosFactor = 0.2f,
                    TrunkWidth = 0.05f,
                    GlowIntensity = 0.1793653f,
                    GlowWidthMultiplier = 4f,
                    Forkedness = 0.5f,
                    Random = random,
                    FadePercent = 0.15f, // set to 0 to disable fade in / out
                    GrowthMultiplier = 0f
                });
                delay += (singleDuration * (((float)random.NextDouble() * 0.8f) + 0.4f));
            }
        }

        protected override void HandleUpdate()
        {
            if (vandeGraaffController.Voltage >= eMax * SparkingPointsDistance)
            {
                GenerateSpark();
                if(_sound) _sound.Play();
                vandeGraaffController.Discharge();
            }
        }

        protected override void HandleFixedUpdate()
        {

        }
    }
}