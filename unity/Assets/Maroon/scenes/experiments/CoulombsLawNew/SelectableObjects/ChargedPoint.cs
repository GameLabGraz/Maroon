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
        private bool generateTrail = false;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private Material trailMaterial;

        private Vector3 storedVelocity  = Vector3.zero; // While the simulation is not running, we store the velocity in this member
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
            GetComponent<SelectableObject>().OnMoved.AddListener((SelectableObject _unused) =>
            {
                trailRenderer.Clear();
            });

            // Note(MartinR): The Prefab Mesh has a radius of 1, e.g. bounds in the range [-1, 1]
            transform.localScale = new Vector3(RADIUS, RADIUS, RADIUS);

            ElectricField.Instance.chargedPoints.Add(this);
            SetCharge(_charge);
            rigidBody.isKinematic = !SimulationController.Instance.SimulationRunning;
            SetHasCollision(hasCollision);

            // Init trailrenderer
            trailRenderer.emitting = false;
            trailRenderer.startWidth = 0.02f;
            trailRenderer.endWidth = 0.02f;
            trailRenderer.time = 3.0f;
            trailRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            trailRenderer.minVertexDistance = 0.05f;
            trailRenderer.material = trailMaterial;

            SimulationController.Instance.OnStart.AddListener(() =>
            {
                if (generateTrail)
                {
                    trailRenderer.emitting = true;
                }
                if (lockPosition) return;
                rigidBody.isKinematic = false;
                rigidBody.velocity = storedVelocity;
            });
            SimulationController.Instance.OnStop.AddListener(() =>
            {
                trailRenderer.emitting = false;
                storedVelocity = rigidBody.velocity;
                rigidBody.isKinematic = true;
            });
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

        public bool GetGenerateTrail()
        {
            return generateTrail;
        }

        public void SetGenerateTrail(bool generateTrail)
        {
            this.generateTrail = generateTrail;
            if (generateTrail)
            {
                if (SimulationController.Instance.SimulationRunning)
                {
                    trailRenderer.emitting = true;
                }
            }
            else
            {
                trailRenderer.Clear();
                trailRenderer.emitting = false;
            }
        }

        public void SetHasCollision(bool hasCollision)
        {
            this.hasCollision = hasCollision;
            gameObject.layer = hasCollision ? 0 : 16; // 16 is BoundaryCollisionOnly layer
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

        public Vector3 GetVelocity()
        {
            if (SimulationController.Instance.SimulationRunning && !rigidBody.isKinematic)
            {
                return rigidBody.velocity;
            }
            return storedVelocity;
        }

        public void SetVelocity(Vector3 newValue)
        {
            storedVelocity = newValue;
            if (SimulationController.Instance.SimulationRunning && !rigidBody.isKinematic)
            {
                rigidBody.velocity = newValue;
            }
        }

        public void FixedUpdate()
        {
            if (!SimulationController.Instance.SimulationRunning) return;
            rigidBody.isKinematic = lockPosition;
            if (lockPosition) return;

            // Apply forces from electric field on the particle
            var fieldVector = ElectricField.Instance.GetFieldValue(transform.position, true, gameObject); // In [Newton/Coulomb]
            rigidBody.AddForce(fieldVector * _charge, ForceMode.Force);
        }
    }
}
