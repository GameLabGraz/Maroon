using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PWSourceEvent : UnityEvent<PointWaveSource>
{
}

public class PointWavePoolHandler : MonoBehaviour, IResetObject
{
    public enum Axis
    {
        XAxis,
        YAxis,
        ZAxis
    }

    [Header("General Settings")]
    public int maxSourceCount = 10;
    public Axis restrict2dModeMovement = Axis.YAxis;

    public PWSourceEvent onSourceAdded;
    public PWSourceEvent onSourceRemoved;
    public UnityEvent onParameterChange;

    [Header("Calculation Settings")]
    public Transform xOrigin2d;
    public Transform xAt1m2d;

    [Header("Axis Settings")]
    public Axis calcSpaceXAxis = Axis.XAxis;
    public Axis calcSpaceYAxis = Axis.YAxis;
    public Axis calcSpaceZAxis = Axis.ZAxis;

    [Header("Maximum Ranges")]
    public float xMax2d = 1f;
    public float yMax2d = 1.35f;

    public PointWaveWaterPlane waterPlane;

    public GameObject minBoundary2d;
    public GameObject maxBoundary2d;

    private ulong _sourceIdCount;
    private Dictionary<ulong, PointWaveSource> _sources = new Dictionary<ulong, PointWaveSource>();

    private float _worldToCalcSpaceFactor2d;
    private float _worldToCalcSpaceFactor2dLocal;

    private bool _initialized = false;

    private static PointWavePoolHandler _instance;

    public static PointWavePoolHandler Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<PointWavePoolHandler>();
            return _instance;
        }
    }

    private void Awake()
    {
        foreach (var waveSource in FindObjectsOfType<PointWaveSource>())
        {
            _sources.Add(_sourceIdCount++, waveSource);
            waterPlane.RegisterWaveSource(waveSource);
        }
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _worldToCalcSpaceFactor2d = Mathf.Abs(xAt1m2d.position.x - xOrigin2d.position.x);
        _worldToCalcSpaceFactor2dLocal = Mathf.Abs(xAt1m2d.localPosition.x - xOrigin2d.localPosition.x);
        _initialized = true;
    }

    private void FixedUpdate()
    {
        if (!_initialized)
            Initialize();

        if (SimulationController.Instance.SimulationRunning)
        {
            RunSimulation();
        }
    }

    public float WorldToCalcSpace(float distanceWorldSpace, bool local = false)
    {
        return distanceWorldSpace / (local ? _worldToCalcSpaceFactor2dLocal : _worldToCalcSpaceFactor2d);
    }

    public Vector3 CalcToWorldSpace(Vector3 distance, bool local = false)
    {
        return new Vector3(CalcToWorldSpace(distance.x, local), CalcToWorldSpace(distance.y, local),
            CalcToWorldSpace(distance.z, local));
    }

    public Vector3 CalcToWorldSpaceCoordinates(Vector3 coord, bool local = true)
    {
        if (local)
        {
            var origin = Vector3.zero;
            var at1m = Vector3.zero;

            var tmp = coord.y;
            coord.y = coord.z;
            coord.z = tmp;
            origin = xOrigin2d.transform.localPosition;
            at1m = xAt1m2d.transform.localPosition;

            return new Vector3(
                origin.x + (at1m.x - origin.x) * coord.x,
                origin.y + (at1m.y - origin.y) * coord.y,
                origin.z + (at1m.z - origin.z) * coord.z);
        }

        throw new NotImplementedException();
    }

    public float CalcToWorldSpace(float distanceCalcSpace, bool local = false)
    {
        return distanceCalcSpace * (local ? _worldToCalcSpaceFactor2dLocal : _worldToCalcSpaceFactor2d);
    }

    private void RunSimulation()
    {
    }

    public GameObject CreateSource(GameObject prefab, Vector3 position, float waveAmplitude, float waveLength, float waveFrequency, float wavePhase,  bool positionInWorldCoord = true)
    {
        var obj = Instantiate(prefab, waterPlane.transform, true);
        Debug.Assert(obj != null);

        var waveSource = obj.GetComponent<PointWaveSource>();  
        waveSource.SetPlane(waterPlane);
        Debug.Assert(waveSource != null);

        waveSource.WaveAmplitude = waveAmplitude;
        waveSource.WaveLength = waveLength;
        waveSource.WaveFrequency = waveFrequency;
        waveSource.WavePhase = wavePhase;

        if (positionInWorldCoord) waveSource.SetPosition(position);
        else
        {
            var pos = xOrigin2d.position + CalcToWorldSpace(new Vector3(position.x, 0,  position.z));
            obj.transform.position = pos;
        }
        waveSource.SetPosition(obj.transform.position);
   
        var movement = obj.GetComponent<PointWave_DragHandler>();
        if (!movement) movement = obj.GetComponentInChildren<PointWave_DragHandler>();
        if (movement)
        {
            movement.SetBoundaries(minBoundary2d, maxBoundary2d);
            movement.allowedXMovement = restrict2dModeMovement != Axis.XAxis;
            movement.allowedYMovement = restrict2dModeMovement != Axis.YAxis;
            movement.allowedZMovement = restrict2dModeMovement != Axis.ZAxis;
        }

        AddSource(waveSource);
        return obj;
    }


    public void AddSource(PointWaveSource pointWaveSource)
    {
        SimulationController.Instance.StopSimulation();

        pointWaveSource.SetSourceKey(_sourceIdCount);
        _sources.Add(_sourceIdCount++, pointWaveSource);
        waterPlane.RegisterWaveSource(pointWaveSource);

        onSourceAdded.Invoke(pointWaveSource);
    }

    public bool ContainsSource(PointWaveSource waveSource)
    {
        return _sources.ContainsKey(waveSource.GetSourceKey());
    }

    public void RemoveSource(PointWaveSource source)
    {
        waterPlane.UnregisterWaveSource(source);
        _sources.Remove(source.GetSourceKey());

        Destroy(source.gameObject);
        onSourceRemoved.Invoke(source);
    }

    public void ResetObject()
    {
        RemoveAllSources();
        waterPlane.UpdateParameterAndPosition();
    }

    public void RemoveAllSources()
    {
        foreach (KeyValuePair<ulong, PointWaveSource> source in _sources)
        {
            waterPlane.UnregisterWaveSource(source.Value);
            Destroy(source.Value.gameObject);
        }

        _sources.Clear();
    }

    public Vector3 GetMinimumPos()
    {
        return minBoundary2d.transform.position;
    }

    public Vector3 GetMaximumPos()
    {
        return maxBoundary2d.transform.position;
    }
}