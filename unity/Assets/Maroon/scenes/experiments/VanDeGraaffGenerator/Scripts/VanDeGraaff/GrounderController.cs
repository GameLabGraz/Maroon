using UnityEngine;

namespace Maroon.Physics.Electromagnetism.VanDeGraaff
{
    public class GrounderController : MonoBehaviour, ICharge
    {
        [SerializeField] private Charge charge;

        [SerializeField] private Light glow;

        [SerializeField] private float maxChargeStrength;

        [SerializeField] private bool isAttachedToInducer;

        public float ChargeStrength
        {
            get => charge.Strength;
            set => charge.Strength = value;
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

        public bool IsAttachedToInducer
        {
            get => isAttachedToInducer;
            set => isAttachedToInducer = value;
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
            var sign = chargeStrength > 0 ? 1 : -1;

            ChargeStrength = Mathf.Abs(chargeStrength) <= maxChargeStrength
                ? chargeStrength
                : sign * maxChargeStrength;

            UpdateGlow();
        }

        public void AddCharge(float chargeStrength)
        {
            if (!(Mathf.Abs(ChargeStrength + chargeStrength) <= maxChargeStrength)) return;
            ChargeStrength += chargeStrength;
            UpdateGlow();
        }

        public void Discharge()
        {
            SetCharge(0f);
        }

        private void UpdateGlow()
        {
            if (glow == null) return;

            glow.color = ChargeStrength < 0 ? Color.blue : Color.red;
            glow.range = 0.8f + 0.4f * Mathf.Abs(ChargeStrength / maxChargeStrength);
        }

        public void OnCollisionEnter(Collision col)
        {
            if (!col.gameObject.CompareTag("Balloon")) return;

            var balloonController = col.gameObject.GetComponent<BalloonController>();
            if (null == balloonController) return;

            var balloonCharge = balloonController.GetCharge();

            // Move charge from Grounder to balloon. The moving charge is the amount 
            // to make the balloon charge negative.
            // For simplicity the balloon charge will always be of the same magnitude and only
            // changes its sign (+/-).

            ChargeStrength += (2f * Mathf.Abs(balloonCharge));
            UpdateGlow();

            // invert charge of balloon (set it to negative)
            balloonController.SetCharge(-1f * Mathf.Abs(balloonCharge));
        }
    }
}