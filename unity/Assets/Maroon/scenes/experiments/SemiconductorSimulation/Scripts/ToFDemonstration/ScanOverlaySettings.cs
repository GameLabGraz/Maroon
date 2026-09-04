using UnityEngine;
using Maroon.Physics;

public class ScanOverlaySettings : MonoBehaviour
{

    [Header("Shader Settings")]
    [SerializeField] private QuantityInt sensorXResolutionQuantity = 512;
    [SerializeField] private QuantityInt sensorYResolutionQuantity = 512;
    [SerializeField] private float depthTolerance = 0.0008f;
    [SerializeField] private float colourIntensity = 0.85f;
    [SerializeField] private float skipFarDistance = 0.999f;
    [SerializeField] private float maxHeatmapDistance;
    [SerializeField] private float maxIRLightingRange;
    [SerializeField] private QuantityInt debugMode = 0;

    public int SensorXResolution { get => sensorXResolutionQuantity.Value; set => sensorXResolutionQuantity.Value = value; }
    public int SensorYResolution { get => sensorYResolutionQuantity.Value; set => sensorYResolutionQuantity.Value = value; }
    public QuantityInt SensorResolutionQuantity
    {
        get => sensorXResolutionQuantity;
    }
    public QuantityInt SensorYResolutionQuantity
    {
        get => sensorYResolutionQuantity;
    }
    public float DepthTolerance { get => depthTolerance; set => depthTolerance = value; }
    public float ColourIntensity { get => colourIntensity; set => colourIntensity = value; }
    public float SkipFarDistance { get => skipFarDistance; set => skipFarDistance = value; }
    public float MaxHeatmapDistance { get => maxHeatmapDistance; set => maxHeatmapDistance = value; }
    public float MaxIRLightingRange { get => maxIRLightingRange; set => maxIRLightingRange = value; }
    public int DebugMode { get => debugMode.Value; set => debugMode.Value = value; } 
    public QuantityInt DebugModeQuantity
    {
        get => debugMode;
    }
}
