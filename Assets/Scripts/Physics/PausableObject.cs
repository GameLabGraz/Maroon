using UnityEngine;

public abstract class PausableObject : MonoBehaviour
{
    private Vector3 CurrentVelocity;

    private bool IsPause = false;

    protected SimulationController simController;

    protected Rigidbody _rigidbody;

    protected virtual void Start()
    {
        simController = FindObjectOfType<SimulationController>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void Update()
    {
        if(simController.SimulationRunning)
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
        if (simController.SimulationRunning)
            HandleFixedUpdate();
    }

    protected abstract void HandleUpdate();

    protected abstract void HandleFixedUpdate();

}
