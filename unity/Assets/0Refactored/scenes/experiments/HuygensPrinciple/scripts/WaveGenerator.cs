using UnityEngine;

[ExecuteInEditMode]
public class WaveGenerator : MonoBehaviour
{
    public enum GeneratorMembership
    {
        WaterBasin,
        SlitPlate1,
        SlitPlateReflector1
    }

    [SerializeField]
    private Mesh planeMesh;

    [SerializeField]
    private GameObject planeObject;

    [SerializeField]
    private float waveAmplitude;

    [SerializeField]
    private float waveLength;

    [SerializeField]
    private float waveFrequency;

    [SerializeField]
    private bool isActive = true;

    public ulong waveGeneratorKey = 0;

    private GeneratorMembership _membership = GeneratorMembership.WaterBasin;

    public float WaveAmplitude { get { return waveAmplitude; } set { waveAmplitude = value; } }

    public float WaveLength { get { return waveLength; } set { waveLength = value; } }

    public float WaveFrequency { get { return waveFrequency; } set { waveFrequency = value; } }

    public void SetGeneratorActive(bool status)
    {
        this.isActive = status;
    }

    public void setGeneratorKey(ulong key)
    {
        this.waveGeneratorKey = key;
    }

    public ulong getGeneratorKey()
    {
        return this.waveGeneratorKey;
    }

    public void setGeneratorMembership(GeneratorMembership membership)
    {
        _membership = membership;
    }

    public GeneratorMembership getGeneratorMembership()
    {
        return _membership;
    }
}
