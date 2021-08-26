using UnityEngine;

public abstract class PausableObject : MonoBehaviour
{
    private Vector3 CurrentVelocity;

    private bool IsPause = false;

    protected Rigidbody _rigidbody;

    protected virtual void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void Update()
    {
        if (SimulationController.Instance == null) return;

        if(SimulationController.Instance.SimulationRunning)
        {
            if(IsPause)
            {
                IsPause = false;
                if(_rigidbody != null)
                {
                    _rigidbody.isKinematic = false;
                    _rigidbody.velocity = CurrentVelocity;
                }
            }

            HandleUpdate();
        }
        else if(!IsPause)
        {
            IsPause = true;
            if (_rigidbody != null)
            {
                CurrentVelocity = _rigidbody.velocity;
                _rigidbody.isKinematic = true;
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        if (SimulationController.Instance == null) return;

        if (SimulationController.Instance.SimulationRunning)
            HandleFixedUpdate();
    }

    protected abstract void HandleUpdate();

    protected abstract void HandleFixedUpdate();

}
