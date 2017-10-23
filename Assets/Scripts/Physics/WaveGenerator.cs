using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaveGenerator : MonoBehaviour
{
    enum WavePropagation { Rectilinear, Circular }

    [SerializeField]
    private Mesh planeMesh;

    [SerializeField]
    private GameObject planeObject;

    [SerializeField]
    private int VerticesPerLength = 40;

    [SerializeField]
    private int VerticesPerWidth = 20;

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


    public float GetWaveValue(Vector3 position, float time)
    {
        startingPoint = transform.position;

        if (propagationMode == WavePropagation.Rectilinear)
        {
            position.x = 0;
            startingPoint.x = 0;
        }
           

        float distanceToSource = Vector3.Distance(startingPoint, position);

        return waveAmplitude * Mathf.Sin(2 * Mathf.PI * waveFrequency * (time - distanceToSource / (waveLength * waveFrequency)));
    }
}
