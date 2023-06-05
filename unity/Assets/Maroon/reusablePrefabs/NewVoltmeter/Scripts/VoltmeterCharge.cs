using Maroon.GlobalEntities;
using Maroon.Physics;
using UnityEngine;

namespace Maroon.Tools.Voltmeter
{
    public class VoltmeterCharge : MonoBehaviour, IGenerateE
    {
        public QuantityFloat charge = 0.0f;
        public QuantityString chargeUnit = "nC";
        public QuantityBool isVisible = true;

        [SerializeField] private float maxChargeValue = 5f;
        [SerializeField] private float minChargeValue = -5f;
        [SerializeField] public QuantityFloat strength;

        public float radius = 0.7022421f;
        private const float CoulombConstant = 1f / (4 * Mathf.PI * 8.8542e-12f);

        public float Charge
        {
            get => charge;
            set
            {
                charge.Value = value;
                chargeUnit.Value = "nC";
            }
        }

        public float ChargeValue => Charge * ChargeUnit;

        public float ChargeUnit
        {
            get
            {
                switch (chargeUnit)
                {
                    case "mC": return 1e-3f;
                    case "µC": return 1e-6f;
                    case "nC": return 1e-9f;
                    default: return 1f;
                }
            }
        }

        public float Strength
        {
            get => strength.Value;
            set => strength.Value = value;
        }

        public bool Enabled { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public Vector3 getE(Vector3 position)
        {
            if (Mathf.Abs(Charge) < 0.001f) return Vector3.zero;
            var distance = CoordSystemHandler.Instance.CalculateDistanceBetween(transform.position, position);
            var dir = (position - transform.position).normalized;
            var potential = CoulombConstant * ChargeValue / Mathf.Pow(distance, 2f);
            return potential * dir;
        }

        public float getEFlux(Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public float getEPotential(Vector3 position)
        {
            if (Mathf.Abs(Charge) < 0.0001f) return 0f;

            var distance = CoordSystemHandler.Instance.CalculateDistanceBetween(transform.position, position);
            return CoulombConstant * ChargeValue / distance;
        }

        public float getFieldStrength()
        {
            return Strength;
        }
    }
}