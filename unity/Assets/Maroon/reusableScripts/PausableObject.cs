using UnityEngine;

public abstract class PausableObject : MonoBehaviour
{
    [SerializeField] private int fixedUpdateRate = 1;

    private int _fixedUpdateCount = 0;

    private bool save = false;

    protected Vector3 CurrentVelocity;

    protected bool IsPause = false;


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
                    save = false;
                }
            }

            HandleUpdate();
        }
        else if(!IsPause)
        {
            IsPause = true;
            if (_rigidbody != null && !save)
            {
                CurrentVelocity = _rigidbody.velocity;
                _rigidbody.isKinematic = true;
                save = true;
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
                if (_rigidbody != null)
                {
                    _rigidbody.isKinematic = false;
                    _rigidbody.velocity = CurrentVelocity;
                    _fixedUpdateCount = 0;
                    save = false;
                }

                HandleFixedUpdate();
            }
            else
            {
                if (_rigidbody == null) return;
                if (save) return;

                CurrentVelocity = _rigidbody.velocity;
                _rigidbody.isKinematic = true;
                save = true;
            }
        }
    }

    protected abstract void HandleUpdate();

    protected abstract void HandleFixedUpdate();

}
