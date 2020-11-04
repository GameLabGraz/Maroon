using UnityEngine;

public class PointWaveSource : MonoBehaviour, IResetObject, IDeleteObject
{
    [SerializeField]
    private PointWaveWaterPlane planeObject;

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

    public ulong pointWaveSourceKey = 0;
    private Vector3 _resetPosition;

    private Vector3 _updatePosition;
    private Rigidbody _rigidbody;

    private PointWavePoolHandler _poolHandler;

    public float WaveAmplitude{ get => waveAmplitude; set {waveAmplitude = value;}}

    public float WaveLength{get => waveLength; set{ waveLength = value;}}

    public float WaveFrequency{get => waveFrequency;set{waveFrequency = value;}}

    public float WavePhase{ get => wavePhase;set{wavePhase = value;}}


    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.velocity = Vector3.zero;

        var obj = GameObject.Find("PoolHandler");
        if (obj)
            _poolHandler = obj.GetComponent<PointWavePoolHandler>();

        Debug.Assert(_poolHandler != null);

        transform.localRotation = Quaternion.Euler(0, 0, 0);
        GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezePositionY;
    }

    public void SetPlaneObject(PointWaveWaterPlane plane)
    {
        this.planeObject = plane;
    }

    public PointWaveWaterPlane GetPlaneObject()
    {
        return this.planeObject;
    }


    public void SetSourceKey(ulong key)
    {
        this.pointWaveSourceKey = key;
    }

    public ulong GetSourceKey()
    {
        return this.pointWaveSourceKey;
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

    public void SetPlane(PointWaveWaterPlane pointWaveWaterPlane)
    {
        planeObject = pointWaveWaterPlane;
    }

    public void ResetObject()
    {
        if(this != null)
            transform.localPosition = _resetPosition;
    }

    public void CalculatedPosition(Vector3 calculatedPosition)
    {
        _updatePosition = calculatedPosition;
    }

    public void MovementStart()
    {
        if (pauseSimulationWhileMoving) SimulationController.Instance.StopSimulation();
    }

    public void MovementEndOutsideBoundaries()
    {
        if (!deleteIfOutsideBoundaries) return;
        _poolHandler.RemoveParticle(this);
        planeObject.UpdateParameterAndPosition();
    }

    public void MovementEndInsideBoundaries()
    {
        UpdateResetPosition();
        planeObject.UpdateParameterAndPosition();
    }
    public void OnDeleteObject()
    {
        _poolHandler.RemoveParticle(this);
    }
}
