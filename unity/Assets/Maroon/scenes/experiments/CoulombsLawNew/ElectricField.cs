using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class ElectricField : MonoBehaviour
    {
        private const float COULOMB_CONSTANT = 1f / (4 * Mathf.PI * Maroon.Physics.PhysicalConstants.e0);

        // Note(MartinR): Electrically charged objects (points, rods, planes) register themselves in these lists
        public List<PointCharge> pointCharges = new List<PointCharge>();

        // Returns the vector-value of the electric field at a given position, Unit: [Newton/Coulomb]
        //      If limitPointChargeInfluence is set, then point charges use a distance threshhold so that
        //      no division by zero/infinitely high values can be produces. This behavior is usually desired
        //      for visualizations and physics calculations (To prevent simulations from exploding due to e.g. overlapping particles)
        public Vector3 GetFieldValue(Vector3 position, bool limitPointChargeInfluence, GameObject excludeObject = null)
        {
            Vector3 fieldValue = Vector3.zero;
            foreach (var pointCharge in pointCharges)
            {
                if (pointCharge.gameObject == excludeObject) continue;

                var toChargeDirection = position - pointCharge.transform.position;
                float distanceInMeter = toChargeDirection.magnitude;
                toChargeDirection = toChargeDirection.normalized; // Note(MartinR): This creates a zero-vector if the position is exactly the charge pos
                if (limitPointChargeInfluence) { 
                    distanceInMeter = Mathf.Min(distanceInMeter, PointCharge.RADIUS); 
                }

                fieldValue += toChargeDirection * pointCharge.GetCharge() * COULOMB_CONSTANT / (distanceInMeter * distanceInMeter);
            }

            return fieldValue;
        }

        // Returns the electric potential (In Volt) at a given position, parameters are similar to GetFieldValue
        public float GetPotential(Vector3 position, bool limitPointChargeInfluence, GameObject excludeObject = null)
        {
            float potential = 0.0f;
            foreach (var pointCharge in pointCharges)
            {
                if (pointCharge.gameObject == excludeObject) continue;

                float distanceInMeter = (position - pointCharge.transform.position).magnitude;
                if (limitPointChargeInfluence) { 
                    distanceInMeter = Mathf.Min(distanceInMeter, PointCharge.RADIUS); 
                }

                potential += pointCharge.GetCharge() * COULOMB_CONSTANT / (distanceInMeter * distanceInMeter);
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
                    // Check if scene already contains a singleton Instance
                    _instance = GameObject.FindObjectOfType<ElectricField>();
                    // Create instance if none exists in scene
                    if (_instance == null) _instance = new GameObject("ElectricField").AddComponent<ElectricField>();
                }

                return _instance;
            }
        }

        private void Awake()
        {
            // To avoid duplication, this script deletes the attached gameObject if an instance of the Singleton already exists
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                Debug.LogWarning("ElectricField instance was destroyed due to duplication");
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
