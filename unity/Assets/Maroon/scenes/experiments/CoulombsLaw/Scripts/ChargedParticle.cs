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
        // CONSTANTS
        public const float MAX_ABSOLUTE_CHARGE = 5e-6f; // In Coulomb, current max is 1 mikro coulomb
        public const float RADIUS = 0.065f; // In unity units (World units)

        // DATA MEMBERS
        public float electricCharge = 0.0f; // In Coulomb
        private bool _fixPositionDuringSimulation = false;
        private Vector3 _positionAtSimulationStart;

        // REFERENCES TO OTHER OBJECTS
        [SerializeField] private GameObject _fixingRing = null;
        [SerializeField] private PC_DragHandler _dragHandler = null; 
        [SerializeField] private PC_ArrowMovement _arrowMovement = null; 
        private Maroon.Physics.Electromagnetism.EField _eField = null;
        private SimulationController _simulationController = null;
        private Rigidbody _rigidBody = null;

        // @TODO(MartinR):
        // Using the CoordSystem causes some problems for calculation, as GetSystemPosition
        // returns values in Axis Units, which may not be in meters, but rather cm in the Coulombs Law experiment.
        // This causes problems in the calculation, as the strength is affected by distance _squared_, so the unit prefix matters.
        // And it's also not possible to just get the coordinates in meters, because the position parameter in getE is also in local values.
        // I generally don't get why CoordAxis returns Values in Subdivision Units, and not in LocalLength units (See properties of CoordAxis)
        // As a quick and dirty fix, I just assume that the CoordinateSystem is uniformly scaled and not rotated, and do the correct conversion manually.
        private float _localToMeterScaleFactor = 1.0f;
        private float _meterToWorldScaleFactor = 1.0f;


        // IGenerateE
        public bool Enabled { get; set; } = true;

        private void Awake()
        {
            _positionAtSimulationStart = transform.position;
            _simulationController = SimulationController.Instance;
            _rigidBody = GetComponent<Rigidbody>();

            // Note(MartinR): The Prefab Mesh has a radius of 1, e.g. bounds in the range [-1, 1]
            transform.localScale = new Vector3(RADIUS, RADIUS, RADIUS);

            // Register Simulation Start/Stop/Reset Handlers
            // Note(MartinR): I'm specifically not using the PausableObject class, because Particles in the CoulombsLaw Experiment
            //      can be edited/moved while the simulation is paused, which leads to inconsistent Behaviour with PausableObject
            //      (e.g. particles resetting to different positions depending if they were edited...)
            // Also currently velocity is not stored between start/stop presses, not sure if this is wanted without a way to edit/view velocity
            if (_simulationController != null && _rigidBody != null)
            {
                _rigidBody.isKinematic = _fixPositionDuringSimulation || !_simulationController.SimulationRunning;

                _simulationController.onStartRunning.AddListener(() =>
                {
                    // Note(MartinR): Particles are always set kinematic, except while the simulation is running
                    _rigidBody.isKinematic = _fixPositionDuringSimulation;
                    _positionAtSimulationStart = transform.position;
                });
                _simulationController.onStopRunning.AddListener(() =>
                {
                    _rigidBody.isKinematic = true;
                });
                _simulationController.OnReset.AddListener(() =>
                {
                    _rigidBody.isKinematic = true;
                    transform.position = _positionAtSimulationStart;
                });
            }

            // Calculate Conversion-Factors between Unity-Coordinates and CoordSystem
            var coordSystem = Maroon.GlobalEntities.CoordSystemHandler.Instance;
            _localToMeterScaleFactor = Mathf.Pow(10, (int) coordSystem.GetSubdivisionUnits()[0]);
            _meterToWorldScaleFactor = 2.25f / 2.0f; // Note(MartinR): Hardcoded values for CoulombsLaw experiment

            // Update Drag Handler and MovementArrows (Set boundary)
            _dragHandler.onEndMovingOutsideBoundaries.AddListener( () => { GameObject.Destroy(gameObject); } );
            var maxBoundary = GameObject.Find("MaxBoundary");
            var minBoundary = GameObject.Find("MinBoundary");
            if (maxBoundary != null && minBoundary != null)
            {
                _dragHandler.minBoundary = minBoundary.transform;
                _dragHandler.maxBoundary = maxBoundary.transform;
                _arrowMovement.SetBoundaries(minBoundary.transform, maxBoundary.transform);
            }

            // Register Particle in EField (OnDestroy removs it from EField, so we can just use DestroyObject to remove Particles)
            _eField = FindObjectOfType<Maroon.Physics.Electromagnetism.EField>();
            if (_eField != null)
            {
                _eField.AddProducerToSet(gameObject);
            }
            else
            {
                Debug.LogError("No efield found after Particle Instanciation, so particle won't do anything!");
            }
        }

        public static ChargedParticle InstanciateParticle(
            ChargedParticle particlePrefab, Vector3 position, float charge, bool fixPosition, Transform parentTransform)
        {
            var particle = Instantiate(particlePrefab, position, Quaternion.identity, parentTransform);
            particle.electricCharge = charge;
            particle.UpdateParticleColor();
            particle.SetFixPosition(fixPosition);

            return particle;
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

        public void SetFixPosition(bool fixPosition)
        {
            _fixPositionDuringSimulation = fixPosition;
            _fixingRing.SetActive(_fixPositionDuringSimulation);
            // Note(MartinR): Particle is always kinematic while simulation is not running
            _rigidBody.isKinematic = _fixPositionDuringSimulation || !_simulationController.SimulationRunning;
        }
        public bool GetFixPosition()
        {
            return _fixPositionDuringSimulation;
        }


        // ---------------------------------
        // Vector Field/Physics Calculations
        // ---------------------------------
        private const float CoulombConstant = 1f / (4 * Mathf.PI * Maroon.Physics.PhysicalConstants.e0);

        public Vector3 getE(Vector3 position)
        {
            var chargePos = transform.position;
            chargePos.z = 0.0f; // 2D mode

            var coordSystem = Maroon.GlobalEntities.CoordSystemHandler.Instance;
            var direction = position - coordSystem.GetSystemPosition(chargePos);
            float distanceInMeter = direction.magnitude * _localToMeterScaleFactor;
            direction = direction.normalized;

            // Clamp Strength to sphere-radius, which avoids division by 0 and makes force not explode on overlapping particles
            distanceInMeter = Mathf.Max(distanceInMeter, RADIUS / _meterToWorldScaleFactor);

            // Coulombs Law calculation (Unit Newton/Coulomb, [N/C])
            return direction * electricCharge * CoulombConstant /  (distanceInMeter * distanceInMeter);
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

        private void FixedUpdate()
        {
            if (_simulationController == null || _eField == null || _rigidBody == null) return;
            if (!_simulationController.SimulationRunning) return;

            var coordSystem = Maroon.GlobalEntities.CoordSystemHandler.Instance;
            var pos = transform.position;
            pos.z = 0.0f;
            var fieldValue = _eField.get(coordSystem.GetSystemPosition(pos), gameObject); // In [Newton/Coulomb]

            // TODO(MartinR): Force needs to be converted from simulation position to world position,
            // but we cannot use coordSystem.GetWorldPosition, because only need scaling + rotation,
            // without translation, as this will add a constant offset to force...
            // See comment at the start of this file 
            var forceInSystemScale = fieldValue * electricCharge;
            var force = forceInSystemScale * _meterToWorldScaleFactor;
            _rigidBody.AddForce(force, ForceMode.Force);
        }
    }
}
