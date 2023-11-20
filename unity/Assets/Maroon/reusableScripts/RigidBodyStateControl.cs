using UnityEngine;

namespace Maroon.Physics
{
    public class RigidBodyState
    {
        public Vector3 Position { get; set; } = Vector3.zero;
        public Quaternion Rotation { get; set; } = Quaternion.identity;
        public Vector3 Velocity { get; set; } = Vector3.zero;
        public Vector3 AngularVelocity { get; set; } = Vector3.zero;
        public bool IsKinematic { get; set; } = false;
    }

    [RequireComponent(typeof(Rigidbody))]
    public class RigidBodyStateControl : MonoBehaviour, IResetObject
    {
        private Rigidbody _rigidBody;

        private readonly RigidBodyState _startState = new RigidBodyState();
        private readonly RigidBodyState _currentState = new RigidBodyState();

        public bool IsStateStored { get; private set; }

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();

            _startState.Position = _rigidBody.position;
            _startState.Rotation = _rigidBody.rotation;
            _startState.Velocity = _rigidBody.velocity;
            _startState.AngularVelocity = _rigidBody.angularVelocity;
            _startState.IsKinematic = _rigidBody.isKinematic;

            StoreRigidBodyState();
            
            if (SimulationController.Instance.SimulationRunning)
                RestoreRigidBodyState();
        }

        public void StoreRigidBodyState()
        {
            if(_currentState == null || _rigidBody == null)
                return;
        
            _currentState.Position = _rigidBody.position;
            _currentState.Rotation = _rigidBody.rotation;
            _currentState.Velocity = _rigidBody.velocity;
            _currentState.AngularVelocity = _rigidBody.angularVelocity;
            _currentState.IsKinematic = _rigidBody.isKinematic;

            _rigidBody.isKinematic = true;
            IsStateStored = true;
        }

        public void RestoreRigidBodyState()
        {
            _rigidBody.position = _currentState.Position;
            _rigidBody.rotation = _currentState.Rotation;
            _rigidBody.velocity = _currentState.Velocity;
            _rigidBody.angularVelocity = _currentState.AngularVelocity;
            _rigidBody.isKinematic = _currentState.IsKinematic;

            IsStateStored = false;
        }

        public void ResetObject()
        {
            _rigidBody.position = _startState.Position;
            _rigidBody.rotation = _startState.Rotation;
            _rigidBody.velocity = _startState.Velocity;
            _rigidBody.angularVelocity = _startState.AngularVelocity;
            _rigidBody.isKinematic = _startState.IsKinematic;

            IsStateStored = false;
        }
    }
}
