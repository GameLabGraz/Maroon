using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maroon.Physics.Electromagnetism.VanDeGraaff
{
    public class VanDeGraaffController : PausableObject
    {
        /// <summary>
        /// The charge object of the Van de Graaff Generator
        /// </summary>
        [SerializeField] private Charge charge;

        /// <summary>
        /// The objects influenced through the Van de Graaff 
        /// Generator by Electrostatic Induction
        /// </summary>
        [SerializeField] private List<GameObject> inducedObjects;

        /// <summary>
        /// The glow corresponding to the charge
        /// </summary>
        [SerializeField] private Light glow;

        /// <summary>
        /// Noise of the mechanical parts (motor + belt)
        /// </summary>
        [SerializeField] private AudioSource sound;

        /// <summary>
        /// The sphere radius in [m]
        /// </summary>
        [SerializeField] private float radius = 0.5f;

        /// <summary>
        /// Electric breakdown field strength of the gas surrounding the sphere in [V/m]
        /// 30kV/cm typically for dry air at STP (Standard conditions for temperature and pressure)
        /// </summary>
        [SerializeField] private float emax = 3e06f;

        /// <summary>
        /// The charging current in [A], respectively [C/s]
        /// Charge transported by the belt to the sphere
        /// </summary>
        [SerializeField] private float chargingCurrent = 10e-6f;

        private List<ICharge> _inducedCharges = new List<ICharge>();
        private bool _on;

        /// <summary>
        /// The potential difference between the sphere and earth in [V]
        /// Derived from the charge in the sphere by Gauss' Law
        /// </summary>

        [SerializeField] private QuantityFloat voltage;

        public float Voltage => voltage.Value;

        /// <summary>
        /// The maximum voltage produced by the Van de Graaff Generator in [V]
        /// Depending on Emax and radius of sphere.
        /// </summary>
        public float MaxVoltage => emax * radius;

        /// <summary>
        /// The current charge strength in the sphere in [C]
        /// </summary>
        public float ChargeStrength
        {
            get => charge.Strength;
            set => charge.Strength = value;
        }

        public bool On
        {
            get => _on;
            set
            {
                _on = value;
                if (sound != null) sound.enabled = value;
            }
        }

        public bool GlowEnabled
        {
            get => glow != null && glow.enabled;
            set
            {
                if (glow != null) glow.enabled = value;
            }
        }

        protected override void Start()
        {
            base.Start();

            charge.strength.onValueChanged.AddListener(chargeStrength =>
            {
                voltage.Value = chargeStrength / (4.0f * Mathf.PI * PhysicalConstants.e0 * radius);
            });

            foreach (var inducedCharge in inducedObjects
                .Select(inducedObject => inducedObject.GetComponent<ICharge>())
                .Where(inducedCharge => inducedCharge != null))
            {
                _inducedCharges.Add(inducedCharge);
            }
        }

        protected override void HandleUpdate()
        {
            UpdateCharge();
            UpdateGlow();
            UpdateInducedCharges();
        }

        protected override void HandleFixedUpdate()
        {

        }

        private void UpdateCharge()
        {
            if (!On)
            {
                // TODO: apply formula for corona discharge and leakage
                // For now just leak a third of the charging current when VdG is switched off

                var leakedChargeSinceLastFrame = Time.deltaTime * (chargingCurrent / 3f);
                ChargeStrength = ChargeStrength - leakedChargeSinceLastFrame > 0f
                    ? ChargeStrength - leakedChargeSinceLastFrame
                    : 0f;
            }
            else if (Voltage < MaxVoltage)
            {
                // add charge transported to the sphere during the last frame
                var transportedChargeSinceLastFrame = Time.deltaTime * chargingCurrent;
                ChargeStrength += transportedChargeSinceLastFrame;
            }
        }

        private void UpdateInducedCharges()
        {
            // For now just fake electrostatic induction by applying
            // a charge of opposite value corresponding to the charge strength of the VdG
            // to all induced objects which are not touching the VdG.
            // If an induced object is attached to the VdG it will get a charge of the
            // same sign as the VdG.

            var vanDeGraaffChargeSign = (ChargeStrength >= 0f) ? 1f : -1f;
            foreach (var inducedCharge in _inducedCharges)
            {
                var inducedChargeSign = inducedCharge.IsAttachedToInducer ? vanDeGraaffChargeSign : vanDeGraaffChargeSign * -1f;
                inducedCharge.SetCharge(inducedChargeSign * Voltage / MaxVoltage * inducedCharge.GetMaxCharge());
            }
        }

        private void UpdateGlow()
        {
            if (glow == null) return;

            glow.color = ChargeStrength < 0 ? Color.blue : Color.red;
            glow.range = 1.35f + 1.4f * Mathf.Abs(Voltage / MaxVoltage);
        }

        public void Discharge()
        {
            ChargeStrength = 0f;
            foreach (var inducedCharge in _inducedCharges)
            {
                inducedCharge.Discharge();
            }
        }

        public void Switch()
        {
            On = !On;
        }

        public void GetVoltageByReference(MessageArgs args)
        {
            args.value = Voltage;
        }

        public void GetChargeStrengthByReference(MessageArgs args)
        {
            args.value = ChargeStrength;
        }

        public void OnCollisionEnter(Collision col)
        {
            if (!col.gameObject.CompareTag("Balloon")) return;

            var balloonController = col.gameObject.GetComponent<BalloonController>();
            if (balloonController == null) return;

            var balloonCharge = balloonController.GetCharge();

            // Move charge from balloon to VdG. The moving charge is the amount 
            // to make the balloon charge positive.
            // For simplicity the balloon charge will always be of the same magnitude and only
            // changes its sign (+/-).
            ChargeStrength -= (2f * Mathf.Abs(balloonCharge));

            // invert charge of balloon (set it to positive)
            balloonController.SetCharge(Mathf.Abs(balloonCharge));
        }
    }
}