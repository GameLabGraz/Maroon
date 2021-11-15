using UnityEngine;

namespace Maroon.Physics
{
    public abstract class PausableObject : MonoBehaviour
    {
        private int _updateRate = 1;
        private int _fixedUpdateRate = 1;

        private int _updateCount = 0;
        private int _fixedUpdateCount = 0;

        protected Rigidbody _rigidBody;
        protected RigidBodyStateControl _rigidBodyStateControl;

        protected virtual void Start()
        {
            if (SimulationController.Instance == null) return;

            _updateRate = SimulationController.Instance.UpdateRate;
            _fixedUpdateRate = SimulationController.Instance.FixedUpdateRate;

            _rigidBody = GetComponent<Rigidbody>();
            _rigidBodyStateControl = GetComponent<RigidBodyStateControl>();

            if (_rigidBody != null && _rigidBodyStateControl == null)
                _rigidBodyStateControl = gameObject.AddComponent<RigidBodyStateControl>();

            SimulationController.Instance.OnReset.AddListener(() =>
            {
                _updateCount = 0;
                _fixedUpdateCount = 0;
            });
            SimulationController.Instance.OnStop.AddListener(() =>
            {
                if (_rigidBody == null || _rigidBodyStateControl.IsStateStored)
                    return;

                _rigidBodyStateControl.StoreRigidBodyState();
            });
            SimulationController.Instance.OnStart.AddListener(() =>
            {
                if (_rigidBody == null)
                    return;

                _rigidBodyStateControl.RestoreRigidBodyState();
            });
        }

        protected virtual void Update()
        {
            if (SimulationController.Instance == null || !SimulationController.Instance.SimulationRunning) 
                return;

            if (++_updateCount % _updateRate == 0)
            {
                HandleUpdate();
            }
        }

        protected virtual void FixedUpdate()
        {
            if (SimulationController.Instance == null || !SimulationController.Instance.SimulationRunning)
                return;

            if (++_fixedUpdateCount % _fixedUpdateRate == 0)
            {
                if(_rigidBody != null & _fixedUpdateRate > 1)
                    _rigidBodyStateControl.RestoreRigidBodyState();

                HandleFixedUpdate();
            }
            else
            {
                if (_rigidBody == null || _rigidBodyStateControl.IsStateStored)
                    return;

                _rigidBodyStateControl.StoreRigidBodyState();
            }
        }

        protected abstract void HandleUpdate();

        protected abstract void HandleFixedUpdate();
    }
}
