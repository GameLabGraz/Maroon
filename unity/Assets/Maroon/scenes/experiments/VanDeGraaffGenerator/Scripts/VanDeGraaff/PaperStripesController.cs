using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics.Electromagnetism.VanDeGraaff
{
    public class PaperStripesController : MonoBehaviour, ICharge
    {
        [SerializeField] private Light glow;

        [SerializeField] private float maxChargeStrength;

        [SerializeField] private bool isAttachedToInducer;

        [SerializeField] private List<Charge> charges = new List<Charge>();

        private float _chargeStrength;

        public bool IsAttachedToInducer
        {
            get => isAttachedToInducer;
            set => isAttachedToInducer = value;
        }

        public bool GlowEnabled
        {
            get => glow != null && glow.enabled;
            set
            {
                if (glow != null)
                    glow.enabled = value;
            }
        }

        public float GetMaxCharge()
        {
            return maxChargeStrength;
        }

        public float GetCharge()
        {
            return _chargeStrength;
        }

        public void SetCharge(float chargeStrength)
        {
            var sign = chargeStrength > 0 ? 1 : -1;
            _chargeStrength = Mathf.Abs(chargeStrength) <= maxChargeStrength
                ? chargeStrength
                : sign * maxChargeStrength;

            SetCharges(_chargeStrength);
            UpdateGlow();
        }

        public void AddCharge(float chargeStrength)
        {
            if (!(Mathf.Abs(_chargeStrength + chargeStrength) <= maxChargeStrength)) return;

            _chargeStrength += chargeStrength;
            SetCharges(_chargeStrength);
            UpdateGlow();
        }

        public void Discharge()
        {
            SetCharges(0f);
        }

        private void SetCharges(float chargeStrength)
        {
            foreach (var charge in charges)
            {
                charge.Strength = chargeStrength;
            }
        }

        private void UpdateGlow()
        {
            if (glow == null) return;

            glow.color = _chargeStrength < 0 ? Color.blue : Color.red;
            glow.intensity = 0.5f * Mathf.Abs(_chargeStrength / maxChargeStrength);
        }
    }
}