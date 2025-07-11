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
        [SerializeField] private GameObject _fixingRing;
        public Rigidbody rigidBody;

        public bool generateFieldLines = false;

        public Vector3 initialVelocity = Vector3.zero;
        public Vector3 storedVelocity  = Vector3.zero; // See CoulombsLawUILogic start/stop/continue buttons for this
        [SerializeField] private bool hasCollision = true;
        public bool contributeToEField = true;
        public bool isConductive = false;

        private bool _lockPosition = false;
        public bool lockPosition { get { return _lockPosition; } set { _lockPosition = value; _fixingRing.SetActive(_lockPosition); } }

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
            return Color.Lerp(Color.gray, charge < 0 ? Color.blue : Color.red, t);
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

        public void SetHasCollision(bool hasCollision)
        {
            this.hasCollision = hasCollision;
            gameObject.layer = hasCollision ? 0 : 11; // 11 is justMouseCollisions layer
        }

        public bool GetHasCollision()
        {
            return hasCollision;
        }

        public void SetMass(float mass)
        {
            rigidBody.mass = mass;
        }

        public float GetMass()
        {
            return rigidBody.mass;
        }

        private void DistributeChargeWithOther(GameObject otherGameObject)
        {
            if (!isConductive) return;

            var otherPoint = otherGameObject.GetComponent<ChargedPoint>();
            if (otherPoint != null && otherPoint.isConductive)
            {
                float newCharge = (GetCharge() + otherPoint.GetCharge()) / 2.0f;
                SetCharge(newCharge);
                otherPoint.SetCharge(newCharge);
                return;
            }

            var otherPlane = otherGameObject.GetComponent<ChargedPlane>();
            if (otherPlane != null && otherPlane.isConductive)
            {
                SetCharge(otherPlane.GetChargeDensity());
                return;
            }

            var otherRod = otherGameObject.GetComponent<ChargedRod>();
            if (otherRod != null && otherRod.isConductive)
            {
                SetCharge(otherRod.GetChargeDensity());
                return;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            DistributeChargeWithOther(collision.gameObject);
        }

        private void OnCollisionStay(Collision collision)
        {
            DistributeChargeWithOther(collision.gameObject);
        }

        public void FixedUpdate()
        {
            if (!SimulationController.Instance.SimulationRunning) return;
            rigidBody.isKinematic = lockPosition;
            if (lockPosition)
            {
                rigidBody.velocity = Vector3.zero;
                return;
            }

            // Apply forces from electric field on the particle
            var fieldVector = ElectricField.Instance.GetFieldValue(transform.position, true, gameObject); // In [Newton/Coulomb]
            rigidBody.AddForce(fieldVector * _charge, ForceMode.Force);
        }
    }
}
