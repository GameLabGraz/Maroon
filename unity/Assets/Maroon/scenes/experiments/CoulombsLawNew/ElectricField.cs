using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class ElectricField : MonoBehaviour
    {
        private const float COULOMB_CONSTANT = 1f / (4 * Mathf.PI * Maroon.Physics.PhysicalConstants.e0);

        // Note(MartinR): Electrically charged objects (points, rods, planes) register themselves in these lists
        public List<ChargedPoint> chargedPoints = new List<ChargedPoint>();
        public List<ChargedRod> chargedRods = new List<ChargedRod>();
        public List<ChargedPlane> chargedPlanes = new List<ChargedPlane>();

        // Returns the vector-value of the electric field at a given position, Unit: [Newton/Coulomb]
        //      If limitChargeInfluenceDistance is set, then charged objects use a distance threshhold so that
        //      no division by zero/infinitely high values can be produces. This behavior is usually desired
        //      for visualizations and physics calculations (To prevent simulations from exploding due to e.g. overlapping objects)
        public Vector3 GetFieldValue(Vector3 position, bool limitChargeInfluenceDistance, GameObject excludeObject = null)
        {
            Vector3 fieldValue = Vector3.zero;

            // Add point charge influence
            foreach (var chargedPoint in chargedPoints)
            {
                if (chargedPoint.gameObject == excludeObject) continue;

                var toChargeDirection = position - chargedPoint.transform.position;
                float distanceInMeter = toChargeDirection.magnitude;
                toChargeDirection = toChargeDirection.normalized; // Note(MartinR): This creates a zero-vector if the position is exactly the charge pos
                if (limitChargeInfluenceDistance) 
                {
                    distanceInMeter = Mathf.Max(distanceInMeter, ChargedPoint.RADIUS); 
                }

                fieldValue += toChargeDirection * chargedPoint.GetCharge() * COULOMB_CONSTANT / (distanceInMeter * distanceInMeter);
            }

            // Add rod influences
            foreach (var chargedRod in chargedRods)
            {
                if (chargedRod.gameObject == excludeObject) continue;

                Vector3 direction = chargedRod.GetDirection(); // Should be normalized
                Vector3 rodPos = chargedRod.transform.position;

                Vector3 positionProjectedOnRod = rodPos + direction * Vector3.Dot(position - rodPos, direction);
                Vector3 rodToPositionDir = position - positionProjectedOnRod;
                float distanceToRod = rodToPositionDir.magnitude;
                rodToPositionDir = rodToPositionDir.normalized;
                if (limitChargeInfluenceDistance)
                {
                    distanceToRod = Mathf.Max(distanceToRod, ChargedRod.RADIUS);
                }

                fieldValue += chargedRod.GetChargeDensity() * 2 * COULOMB_CONSTANT * rodToPositionDir / distanceToRod;
            }

            // Add plane influences
            foreach (var chargedPlane in chargedPlanes)
            {
                if (chargedPlane.gameObject == excludeObject) continue;

                var normal = chargedPlane.GetNormal();
                var pointOnPlane = chargedPlane.transform.position;
                var signedDistance = Vector3.Dot(normal, position - pointOnPlane);

                fieldValue += chargedPlane.GetChargeDensity() * (2 * Mathf.PI * COULOMB_CONSTANT) * Mathf.Sign(signedDistance) * normal;
            }

            return fieldValue;
        }

        // Returns the electric potential (In Volt) at a given position, parameters are similar to GetFieldValue
        public float GetPotential(Vector3 position, bool limitChargeInfluenceDistance, GameObject excludeObject = null)
        {
            float potential = 0.0f;

            // Add point charge influence
            foreach (var chargedPoint in chargedPoints)
            {
                if (chargedPoint.gameObject == excludeObject) continue;

                float distanceInMeter = (position - chargedPoint.transform.position).magnitude;
                if (limitChargeInfluenceDistance) { 
                    distanceInMeter = Mathf.Max(distanceInMeter, ChargedPoint.RADIUS); 
                }

                potential += chargedPoint.GetCharge() * COULOMB_CONSTANT / distanceInMeter;
            }

            // Add rod influences
            foreach (var chargedRod in chargedRods)
            {
                if (chargedRod.gameObject == excludeObject) continue;

                Vector3 direction = chargedRod.GetDirection(); // Should be normalized
                Vector3 rodPos = chargedRod.transform.position;

                Vector3 positionProjectedOnRod = rodPos + direction * Vector3.Dot(position - rodPos, direction);
                Vector3 rodToPositionDir = position - positionProjectedOnRod;
                float distanceToRod = rodToPositionDir.magnitude;
                rodToPositionDir = rodToPositionDir.normalized;
                if (limitChargeInfluenceDistance)
                {
                    distanceToRod = Mathf.Max(distanceToRod, ChargedRod.RADIUS);
                }

                potential += -chargedRod.GetChargeDensity() * 2 * COULOMB_CONSTANT * Mathf.Log(distanceToRod);
            }

            // Add plane influences
            foreach (var chargedPlane in chargedPlanes)
            {
                if (chargedPlane.gameObject == excludeObject) continue;

                var normal = chargedPlane.GetNormal();
                var pointOnPlane = chargedPlane.transform.position;
                var signedDistance = Vector3.Dot(normal, position - pointOnPlane);

                potential += -chargedPlane.GetChargeDensity() * Mathf.Abs(signedDistance) * (2 * Mathf.PI * COULOMB_CONSTANT);
            }

            return potential;
        }



        // Singleton pattern, see SimulationBox.cs for more comments about the implementation
        private static ElectricField _instance;

        public static ElectricField Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<ElectricField>();
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;
        }

        private void OnDestroy()
        {
            if (this == _instance) { _instance = null; }
        }
    }
}
