using UnityEngine;

public class RigidBodyState
{
    public Vector3 Position { get; set; } = Vector3.zero;
    public Quaternion Rotation { get; set; } = Quaternion.identity;
    public Vector3 Velocity { get; set; } = Vector3.zero;
    public Vector3 AngularVelocity { get; set; } = Vector3.zero;
    public bool IsKinematic { get; set; }
    public bool IsStored { get; set; }

    public RigidBodyState() {}

    public RigidBodyState(Rigidbody rigidBody)
    {
        if (rigidBody == null) return;

        Position = rigidBody.position;
        Rotation = rigidBody.rotation;
        Velocity = rigidBody.velocity;
        AngularVelocity = rigidBody.angularVelocity;
        IsKinematic = rigidBody.isKinematic;
        IsStored = true;
    }
}

public abstract class PausableObject : MonoBehaviour
{
    [SerializeField] private int updateRate = 1;
    [SerializeField] private int fixedUpdateRate = 1;

    private int _updateCount = 0;
    private int _fixedUpdateCount = 0;

    protected Rigidbody _rigidBody;
    protected RigidBodyState startRigidBodyState;
    protected RigidBodyState rigidBodyState = new RigidBodyState();

    protected virtual void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();

        startRigidBodyState = new RigidBodyState(_rigidBody);
        StoreRigidBodyState();

        SimulationController.Instance.OnReset.AddListener(() =>
        {
            _updateCount = 0;
            _fixedUpdateCount = 0;

            rigidBodyState.Position = startRigidBodyState.Position;
            rigidBodyState.Rotation = startRigidBodyState.Rotation;
            rigidBodyState.Velocity = startRigidBodyState.Velocity;
            rigidBodyState.AngularVelocity = startRigidBodyState.AngularVelocity;
            rigidBodyState.IsKinematic = startRigidBodyState.IsKinematic;
            rigidBodyState.IsStored = true;

        });
        SimulationController.Instance.OnStop.AddListener(() =>
        {
            if (_rigidBody == null || rigidBodyState.IsStored)
                return;

            StoreRigidBodyState();
            _rigidBody.isKinematic = true;
        });
        SimulationController.Instance.OnStart.AddListener(RestoreRigidBody);
    }

    protected virtual void Update()
    {
        if (SimulationController.Instance == null) return;

        if(SimulationController.Instance.SimulationRunning)
        {
            if (++_updateCount % updateRate == 0)
            {
                HandleUpdate();
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        if (SimulationController.Instance == null) return;

        if (SimulationController.Instance.SimulationRunning)
        {
            if (++_fixedUpdateCount % fixedUpdateRate == 0)
            {
                RestoreRigidBody();
                HandleFixedUpdate();
            }
            else
            {
                if (_rigidBody == null || rigidBodyState.IsStored)
                    return;

                StoreRigidBodyState();
                _rigidBody.isKinematic = true;
            }
        }
    }

    private void StoreRigidBodyState()
    {
        if (_rigidBody == null) return;

        rigidBodyState.Position = _rigidBody.position;
        rigidBodyState.Rotation = _rigidBody.rotation;
        rigidBodyState.Velocity = _rigidBody.velocity;
        rigidBodyState.AngularVelocity = _rigidBody.angularVelocity;
        rigidBodyState.IsKinematic = _rigidBody.isKinematic;
        rigidBodyState.IsStored = true;
        _rigidBody.isKinematic = true;
    }

    private void RestoreRigidBody()
    {
        if (_rigidBody == null) return;

        _rigidBody.position = rigidBodyState.Position;
        _rigidBody.rotation = rigidBodyState.Rotation;
        _rigidBody.velocity = rigidBodyState.Velocity;
        _rigidBody.angularVelocity = rigidBodyState.AngularVelocity;
        _rigidBody.isKinematic = rigidBodyState.IsKinematic;
        rigidBodyState.IsStored = false;
    }

    protected abstract void HandleUpdate();

    protected abstract void HandleFixedUpdate();

}
