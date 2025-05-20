using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLaw
{
    public class ParticleController : MonoBehaviour
    {
        private List<ChargedParticle> _particles = new List<ChargedParticle>();

        [SerializeField] private ChargedParticle _particlePrefab = null;
        [SerializeField] private Maroon.Physics.Electromagnetism.EField _eField = null;
        [SerializeField] private Transform _minBoundary = null;
        [SerializeField] private Transform _maxBoundary = null;

        public ChargedParticle CreateChargedParticle(Vector3 position, float charge, bool positionLocked)
        {
            var particle = Instantiate(_particlePrefab, position, Quaternion.identity, transform);
            particle.Initialize(this, _eField, _maxBoundary, _minBoundary, charge, positionLocked);

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
                result.Add(new Vector4(pos.x, pos.y, pos.z, particle.electricCharge));
            }
            return result;
        }
    }
}
