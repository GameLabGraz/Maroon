using UnityEngine;

[ExecuteInEditMode]
public class WaveGenerator : MonoBehaviour
{
    public enum WavePropagation { Rectilinear, Circular }

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
    private Vector3 startingPoint = Vector3.zero;

    [SerializeField]
    private WavePropagation propagationMode;

    [SerializeField]
    private Vector3 propagationAxis = Vector3.right;

    [SerializeField]
    private bool isActive = true;

    public ulong waveGeneratorKey = 0; 

    public float WaveAmplitude { get { return waveAmplitude; } set { waveAmplitude = value; } }

    public float WaveLength { get { return waveLength; } set { waveLength = value; } }

    public float WaveFrequency { get { return waveFrequency; } set { waveFrequency = value; } }


    public float GetWaveValue(Vector3 position, float time)
    {
        if (!isActive)
            return 0;

        startingPoint = transform.position;

        if (propagationMode == WavePropagation.Rectilinear)
        {
            position.z = 0;
            startingPoint.z = 0;
        }
           
        float distanceToSource = Vector3.Distance(startingPoint, position);
        return waveAmplitude * Mathf.Sin(2 * Mathf.PI * waveFrequency * (time - distanceToSource / (waveLength * waveFrequency)));
    }

    public void SetPropagationMode(int propagationMode)
    {
        this.propagationMode = (WavePropagation)propagationMode;
    }

    public void SetPropagationMode(WavePropagation propagationMode)
    {
        this.propagationMode = propagationMode;
    }

    public void SetPlaneObject(GameObject plane)
    {
        this.planeObject = plane;
    }

    public GameObject GetPlaneObject()
    {
        return this.planeObject;
    }

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
}
