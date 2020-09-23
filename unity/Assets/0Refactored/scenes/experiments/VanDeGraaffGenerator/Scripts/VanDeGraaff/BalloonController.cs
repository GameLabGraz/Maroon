using UnityEngine;

namespace Maroon.Physics.Electromagnetism.VanDeGraaff
{
    public class BalloonController : PausableObject, ICharge
    {

        [SerializeField] private VanDeGraaffController chargeInducer;
        [SerializeField] private Charge charge;
        [SerializeField] private float maxChargeStrength = 1.65e-6f; // in Coulomb [C]
        [SerializeField] private bool isAttachedToInducer;

        [SerializeField] private Light glow;
        [SerializeField] private Light leftGlow;
        [SerializeField] private Light rightGlow;

        private const float MinGlowRangeOffset = 0.65f;
        private const float MaxGlowRangeOffset = 0.75f;
        private const float MinGlowRangeDynamic = 0.25f;
        private const float MaxGlowRangeDynamic = 0.4f;

        private float _chargeStrength;
        private AudioSource _sound;

        public bool GlowEnabled { get; set; } = true;

        public float ChargeStrength
        {
            get => charge.Strength;
            set => charge.Strength = value;
        }

        public bool IsAttachedToInducer
        {
            get => isAttachedToInducer; 
            set => isAttachedToInducer = value;
        }

        private void Start()
        {
            // Randomly set initial charge of balloon - positive or negative
            if (Random.Range(0, 2) > 0)
            {
                charge.Strength *= -1f;
            }

            _sound = GetComponent<AudioSource>();
        }

        protected override void HandleUpdate()
        {
            var inducerChargeStrengthIsZero = chargeInducer.ChargeStrength <= 0;
            charge.enabled = !inducerChargeStrengthIsZero;

            leftGlow.enabled = !inducerChargeStrengthIsZero && GlowEnabled;
            rightGlow.enabled = !inducerChargeStrengthIsZero && GlowEnabled;
            glow.enabled = inducerChargeStrengthIsZero && GlowEnabled;

            var glowFactor = chargeInducer.Voltage / chargeInducer.MaxVoltage;

            if (charge.Strength < 0f)
            {
                leftGlow.range = MinGlowRangeOffset + MinGlowRangeDynamic * glowFactor;
                rightGlow.range = MaxGlowRangeOffset + MaxGlowRangeDynamic * glowFactor;
                glow.color = Color.blue;
            }
            else
            {
                leftGlow.range = MaxGlowRangeOffset + MaxGlowRangeDynamic * glowFactor;
                rightGlow.range = MinGlowRangeOffset + MinGlowRangeDynamic * glowFactor;
                glow.color = Color.red;
            }
        }

        protected override void HandleFixedUpdate()
        {
        }

        public float GetMaxCharge()
        {
            return maxChargeStrength;
        }

        public float GetCharge()
        {
            return ChargeStrength;
        }

        public void SetCharge(float chargeStrength)
        {
            if (Mathf.Abs(chargeStrength) <= maxChargeStrength)
            {
                ChargeStrength = chargeStrength;
            }
        }

        public void AddCharge(float chargeStrength)
        {
            if (Mathf.Abs(ChargeStrength + chargeStrength) <= maxChargeStrength)
            {
                ChargeStrength += chargeStrength;
            }
        }

        public void Discharge()
        {
            SetCharge(0f);
        }

        public void OnCollisionEnter(Collision col)
        {
            if(_sound) _sound.Play();
        }
    }
}
