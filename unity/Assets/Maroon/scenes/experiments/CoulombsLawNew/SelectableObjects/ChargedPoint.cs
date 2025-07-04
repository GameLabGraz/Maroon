using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class ChargedPoint : MonoBehaviour
    {
        // Constants
        public const float RADIUS = 0.065f; // In unity units
        public const float MAX_ABSOLUTE_CHARGE = 5e-6f; // In Coulomb, current max is 1 mikro coulomb

        // Members
        private float _charge = 0.0f; // In Coulomb
        private MeshRenderer _baseMeshRenderer;
        private GameObject _fixingRing;
        public Rigidbody rigidBody;

        private void Awake()
        {
            _baseMeshRenderer = transform.Find("Base").GetComponent<MeshRenderer>();
            _fixingRing       = transform.Find("FixingRing").gameObject;
            rigidBody = GetComponent<Rigidbody>();
            Debug.Assert(rigidBody != null, "PointCharge should have rigidbody");

            GetComponent<SelectableObject>().boundingRadius = RADIUS;
            GetComponent<SelectableObject>().OnDraggedOutOfBounds.AddListener((SelectableObject _unused) =>
            {
                GameObject.Destroy(this.gameObject);
            });

            // Note(MartinR): The Prefab Mesh has a radius of 1, e.g. bounds in the range [-1, 1]
            transform.localScale = new Vector3(RADIUS, RADIUS, RADIUS);

            ElectricField.Instance.chargedPoints.Add(this);
            SetCharge(_charge);
            rigidBody.isKinematic = !SimulationController.Instance.SimulationRunning;
        }

        private void OnDestroy() 
        { 
            ElectricField.Instance?.chargedPoints.Remove(this); 
        }

        public static Color ChargeValueToColor(float charge, float maxValue)
        {
            float t = Mathf.Clamp(Mathf.Abs(charge) / maxValue, 0.0f, 1.0f);
            return Color.Lerp(Color.gray, charge < 0 ? Color.blue : Color.red, Mathf.Pow(t / maxValue, 1/2.2f));
        }

        public float GetCharge() { return _charge; }

        public void SetCharge(float newCharge)
        {
            _charge = newCharge; // UI should handle clamping
            Color color = ChargeValueToColor(_charge, MAX_ABSOLUTE_CHARGE);

            // Change second material (Upper and lower part of the + symbol) to show + or - depending on charge
            List<Material> materials = new List<Material>();
            _baseMeshRenderer.GetMaterials(materials);
            materials[0].color = color;
            materials[2] = _baseMeshRenderer.materials[_charge < 0 ? 0 : 1];
            _baseMeshRenderer.SetMaterials(materials);
        }

        public void FixedUpdate()
        {
            if (!SimulationController.Instance.SimulationRunning) return;

            // Apply forces from electric field on the particle
            var fieldVector = ElectricField.Instance.GetFieldValue(transform.position, true, gameObject); // In [Newton/Coulomb]
            rigidBody.AddForce(fieldVector * _charge, ForceMode.Force);
        }
    }
}
