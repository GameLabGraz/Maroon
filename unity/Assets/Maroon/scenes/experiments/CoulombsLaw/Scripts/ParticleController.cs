using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLaw
{
    // Note(MartinR):
    // The main use for the particle-controller is to keep a list of references to all created particles
    public class ParticleController : MonoBehaviour
    {
        private List<ChargedParticle> _particles = new List<ChargedParticle>();

        [SerializeField] private ChargedParticle _particlePrefab = null;
        [SerializeField] private Maroon.Physics.Electromagnetism.EField _eField = null;
        [SerializeField] private Transform _minBoundary = null;
        [SerializeField] private Transform _maxBoundary = null;

        [SerializeField] private UIFloatInput _dragInput = null;
        [SerializeField] private UIFloatInput _bouncinessInput = null;
        [SerializeField] private UnityEngine.PhysicMaterial _chargePhysicsMaterial = null;

        private void Awake()
        {
            _dragInput.OnValueChanged.AddListener((float newDrag) =>
            {
                foreach (var particle in _particles)
                {
                    var rigidbody = particle.GetComponent<Rigidbody>();
                    if (rigidbody == null) continue;
                    rigidbody.drag = newDrag;
                }
            });

            _bouncinessInput.OnValueChanged.AddListener((float bounciness) =>
            {
                _chargePhysicsMaterial.bounciness = bounciness;
            });

            _chargePhysicsMaterial.bounciness = _bouncinessInput.GetValue();
        }

        public ChargedParticle CreateChargedParticle(Vector3 position, float charge, bool positionLocked)
        {
            var particle = Instantiate(_particlePrefab, position, Quaternion.identity, transform);
            particle.Initialize(this, _eField, _maxBoundary, _minBoundary, charge, positionLocked);
            particle.GetComponent<Rigidbody>().drag = _dragInput.GetValue();

            if (_eField != null)
            {
                _eField.AddProducerToSet(particle.gameObject);
            }
            _particles.Add(particle);

            return particle;
        }

        public void RemoveChargedParticle(ChargedParticle particle)
        {
            _particles.Remove(particle);
            if (_eField != null)
            {
                _eField.RemoveProducerFromSet(particle.gameObject);
            }
            if (particle != null)
            {
                GameObject.Destroy(particle.gameObject);
            }
        }

        // FUNCTIONS FOR VISUALIZATION PLANE
        public int GetMaxChargeCount() { return 10; }
        public bool GetVisualizationEnabled() { return true; }

        public List<Vector4> GetPackedParticleInfo()
        {
            var result = new List<Vector4>();
            foreach (var particle in _particles)
            {
                var pos = particle.transform.position;
                result.Add(new Vector4(pos.x, pos.y, pos.z, particle.GetCharge()));
            }
            return result;
        }
    }
}
