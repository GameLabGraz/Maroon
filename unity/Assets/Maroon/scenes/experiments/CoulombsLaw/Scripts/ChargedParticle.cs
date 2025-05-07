using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLaw
{
    // Note(MartinR):
    // In previous Versions the Particles/Spheres in CoulombsExperiment were handled by
    // different classes, namely: CoulombChargeBehavior, then Charge
    public class ChargedParticle : MonoBehaviour, IGenerateE
    {
        public const float MAX_ABSOLUTE_CHARGE = 5.0f;
        public float electricCharge = 0.0f; // In Coulomb

        [SerializeField] private PC_DragHandler _dragHandler = null; 
        private Maroon.Physics.Electromagnetism.EField efield = null;

        // IGenerateE
        public bool Enabled { get; set; } = true;

        void Start()
        {
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
            efield = FindObjectOfType<Maroon.Physics.Electromagnetism.EField>();
            if (efield == null)
            {
                Debug.LogError("No efield found after Particle Instanciation, so particle won't do anything!");
                return;
            }
            efield.AddProducerToSet(gameObject);
        }

        private void OnDestroy()
        {
            if (efield == null) return;
            efield.RemoveProducerFromSet(gameObject);
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
            var distance = direction.magnitude;
            direction = direction.normalized;

            // Clamp Strength to sphere-scale, which avoids division by 0 and handles overlapping particles gracefully
            float maxScale = Mathf.Max(transform.lossyScale.x, Mathf.Max(transform.lossyScale.y, transform.lossyScale.z));
            distance = Mathf.Max(distance, maxScale);

            return electricCharge * direction * CoulombConstant /  Mathf.Pow(distance, 2);
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
    }
}
