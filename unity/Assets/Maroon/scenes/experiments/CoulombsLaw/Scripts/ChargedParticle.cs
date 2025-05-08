using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLaw
{
    // Note(MartinR):
    // In previous Versions the Particles/Spheres in CoulombsExperiment were handled by
    // different classes, namely: CoulombChargeBehavior, then Charge, and in the newest version this class is used
    public class ChargedParticle : MonoBehaviour, IGenerateE
    {
        public const float MAX_ABSOLUTE_CHARGE = 1e-6f; // Current max is 1 mikro coulomb
        public const float RADIUS = 0.05f; // In unity units
        public float electricCharge = 0.0f; // In Coulomb

        [SerializeField] private PC_DragHandler _dragHandler = null; 
        private Maroon.Physics.Electromagnetism.EField _eField = null;

        // IGenerateE
        public bool Enabled { get; set; } = true;

        private void Start()
        {
            // base.Start();

            _dragHandler.onEndMovingOutsideBoundaries.AddListener( () => { GameObject.Destroy(gameObject); } );

            // Set Drag Handler boundary
            var maxBoundary = GameObject.Find("MaxBoundary");
            var minBoundary = GameObject.Find("MinBoundary");
            if (maxBoundary != null && minBoundary != null)
            {
                _dragHandler.minBoundary = minBoundary.transform;
                _dragHandler.maxBoundary = maxBoundary.transform;
            }

            // Register this Particle in EField as Producer
            _eField = FindObjectOfType<Maroon.Physics.Electromagnetism.EField>();
            if (_eField == null)
            {
                Debug.LogError("No efield found after Particle Instanciation, so particle won't do anything!");
                return;
            }
            _eField.AddProducerToSet(gameObject);
        }

        private void OnDestroy()
        {
            if (_eField == null) return;
            _eField.RemoveProducerFromSet(gameObject);
        }

        public void UpdateParticleColor()
        {
            var particleBase = transform.Find("Base").GetComponent<MeshRenderer>();
            var mat = particleBase.materials;

            electricCharge = Mathf.Clamp(electricCharge, -MAX_ABSOLUTE_CHARGE, MAX_ABSOLUTE_CHARGE);
            if (electricCharge < 0)
            {
                mat[0].color = Color.blue;
                mat[1].color = Color.white;
                mat[2].color = mat[0].color;
            }
            else if (electricCharge > 0)
            {
                mat[0].color = Color.red;
                mat[1].color = mat[2].color = Color.white;
            }
            else
            {
                mat[0].color = mat[1].color = mat[2].color = Color.green;
            }

            particleBase.materials = mat;
        }



        // -------------------------
        // Vector Field Calculations
        // -------------------------
        private const float CoulombConstant = 1f / (4 * Mathf.PI * Maroon.Physics.PhysicalConstants.e0);

        public Vector3 getE(Vector3 position)
        {
            var coordSystem = Maroon.GlobalEntities.CoordSystemHandler.Instance;
            var direction = position - coordSystem.GetSystemPosition(transform.position);
            // Note(MartinR): To get the corrects units for the E-Field, we need to use meters in our calculation
            var unit = coordSystem.GetSubdivisionUnits()[0];
            var distanceInMeters = direction.magnitude * Mathf.Pow(10, -(int)unit); 
            direction = direction.normalized;

            // Find Sphere radius in coord system, TODO(MartinR): There has to be a better way to do this
            var diff = 
                coordSystem.GetSystemPosition(new Vector3(RADIUS, RADIUS, RADIUS), Physics.CoordinateSystem.Unit.m) -
                coordSystem.GetSystemPosition(Vector3.zero, Physics.CoordinateSystem.Unit.m);
            float radiusInCoordSystem = Mathf.Max(diff.z, Mathf.Max(diff.x, diff.y));

            // Clamp Strength to sphere-radius, which avoids division by 0 and makes force not explode on overlapping particles
            distanceInMeters = Mathf.Max(distanceInMeters, radiusInCoordSystem);

            return direction * electricCharge * CoulombConstant /  (distanceInMeters * distanceInMeters);
        }

        public float getEPotential(Vector3 position)
        {
            return 0.0f;
        }

        public float getEFlux(Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public float getFieldStrength()
        {
            // Note(MartinR): Not sure when this is used, and if this value is correct
            return electricCharge;
        }

        // ------------------------------
        // Pausable Object Implementation
        // ------------------------------
        private void Update() {}

        private void FixedUpdate()
        {
            if (_eField == null) return;

            var coordSystem = Maroon.GlobalEntities.CoordSystemHandler.Instance;
            var pos = coordSystem.GetSystemPosition(transform.position);
            var fieldValue = _eField.get(pos, gameObject); // In [Newton/Coulomb]
            var force = fieldValue * electricCharge;
            // TODO(MartinR): Force needs to be converted from simulation position to world position,
            // but we only need scale + rotation, but no translation, as this will add a constant offset to force...

            // For debugging, add a constant acceleration towards target
            force = fieldValue.normalized * 10;
            force = Vector3.left * 10;

            var rigidBody = GetComponent<Rigidbody>();
            if (Input.GetKey(KeyCode.V) && rigidBody != null)
            {
                rigidBody.AddForce(force, ForceMode.Acceleration); // Assuming mass = 1kg
            }
        }

    }
}
