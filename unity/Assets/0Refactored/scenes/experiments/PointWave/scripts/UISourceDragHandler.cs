using UnityEngine;
using UnityEngine.UI;

public class UISourceDragHandler : UISourceDragHandlerSimple
{
    private PointWavePoolHandler _pointWaveLogic;

    private float _waveAmplitude = 0.1f;
    private float _waveLength = 0.1f;
    private float _waveFrequency = 0.1f;
    private float _wavePhase = 0.1f;

    public bool FixedPosition { get; set; } = false;

    public float WaveAmplitude { get => _waveAmplitude; set { _waveAmplitude = value; } }
    public float WaveLength { get => _waveLength; set { _waveLength = value; } }
    public float WaveFrequency { get => _waveFrequency; set { _waveFrequency = value; } }
    public float WavePhase { get => _wavePhase; set { _wavePhase = value; } }

    private void Start()
    {
        var simControllerObject = GameObject.Find("PoolHandler");
        if (simControllerObject)
            _pointWaveLogic = simControllerObject.GetComponent<PointWavePoolHandler>();
    }

    protected override void ShowObject(Vector3 position, Transform parent)
    {
        _pointWaveLogic.CreateCharge(generatedObject, position, _waveAmplitude, _waveLength, _waveFrequency, _wavePhase, true);
    }

    public void AddWithGivenSettings()
    {
        _pointWaveLogic.CreateCharge(generatedObject, _pointWaveLogic.GetComponent<PointWave_SelectionHandler>().GetPositionInWorldSpace(PointWaveSelectScript.SelectObjectType.SourceSelect),
            _waveAmplitude, _waveLength, _waveFrequency, _wavePhase, FixedPosition);
    }
}