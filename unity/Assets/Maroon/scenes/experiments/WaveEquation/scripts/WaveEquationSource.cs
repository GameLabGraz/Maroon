using UnityEngine;

public class WaveEquationSource : MonoBehaviour, IResetObject, IDeleteObject
{
    [SerializeField]
    private WaveEquationWaterPlane planeObject;

    [SerializeField]
    private float waveAmplitude;

    [SerializeField]
    private float waveLength;

    [SerializeField]
    private float waveFrequency;

    [SerializeField]
    private float wavePhase;

    [Header("Movement Settings")]
    public bool pauseSimulationWhileMoving = true;
    public bool deleteIfOutsideBoundaries = true;

    public ulong WaveEquationSourceKey = 0;
    private Vector3 _resetPosition;

    private Rigidbody _rigidbody;

    private WaveEquationPoolHandler _poolHandler;

    public float WaveAmplitude{ get => waveAmplitude; set => waveAmplitude = value; }

    public float WaveLength{get => waveLength; set => waveLength = value;}

    public float WaveFrequency{get => waveFrequency; set => waveFrequency = value;}

    public float WavePhase{ get => wavePhase; set => wavePhase = value;}

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.velocity = Vector3.zero;

        var obj = FindObjectOfType<WaveEquationPoolHandler>();
        if (obj)
            _poolHandler = obj.GetComponent<WaveEquationPoolHandler>();

        transform.localRotation = Quaternion.identity;
        GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezePositionY;
    }

    public void SetPlaneObject(WaveEquationWaterPlane plane)
    {
        this.planeObject = plane;
    }

    public WaveEquationWaterPlane GetPlaneObject()
    {
        return this.planeObject;
    }


    public void SetSourceKey(ulong key)
    {
        this.WaveEquationSourceKey = key;
    }

    public ulong GetSourceKey()
    {
        return this.WaveEquationSourceKey;
    }

    public void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
        UpdateResetPosition();

    }

    public void UpdateResetPosition() 
    {
        _resetPosition = transform.localPosition; 
    }

    public void SetPlane(WaveEquationWaterPlane WaveEquationWaterPlane)
    {
        planeObject = WaveEquationWaterPlane;
    }

    public void ResetObject()
    {
        if(this != null)
            transform.localPosition = _resetPosition;
    }

    public void MovementStart()
    {
        if (pauseSimulationWhileMoving) SimulationController.Instance.StopSimulation();
    }

    public void MovementEndOutsideBoundaries()
    {
        if (!deleteIfOutsideBoundaries) return;
        _poolHandler.RemoveSource(this);
        planeObject.UpdateParameterAndPosition();
    }

    public void MovementEndInsideBoundaries()
    {
        UpdateResetPosition();
        planeObject.UpdateParameterAndPosition();
    }
    public void OnDeleteObject()
    {
        _poolHandler.RemoveSource(this);
    }
}
