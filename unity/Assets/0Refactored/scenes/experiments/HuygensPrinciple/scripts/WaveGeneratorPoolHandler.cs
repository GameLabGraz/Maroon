using System.Collections.Generic;
using Maroon.Physics.HuygensPrinciple;
using UnityEngine;

public class WaveGeneratorPoolHandler : MonoBehaviour
{
    [SerializeField] private float waveAmplitude;
    [SerializeField] private float waveLength;
    [SerializeField] private float waveFrequency;

    [SerializeField] private WaterPlane waterPlane;

    private ulong _generatorIdCount;
    private Dictionary<ulong, WaveGenerator> _generators = new Dictionary<ulong, WaveGenerator>();
    private SlitPlate slitPlate;

    private static WaveGeneratorPoolHandler _instance;
    public static WaveGeneratorPoolHandler Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<WaveGeneratorPoolHandler>();
            return _instance;
        }
    }

    public float WaveLength => waveLength;

    private void Awake()
    {
        slitPlate = GameObject.Find("Plate").GetComponent<SlitPlate>();

        foreach (var waveGenerator in FindObjectsOfType<WaveGenerator>())
        {
            _generators.Add(_generatorIdCount++, waveGenerator);
            waterPlane.RegisterWaveGenerator(waveGenerator);
        }
    }

    public void AddWaveGenerator(WaveGenerator generator)
    {
        generator.setGeneratorKey(_generatorIdCount);
        _generators.Add(_generatorIdCount++, generator);
        waterPlane.RegisterWaveGenerator(generator);
    }

    public void RemoveWaveGenerator(WaveGenerator generator)
    {
        waterPlane.UnregisterWaveGenerator(generator);
        _generators.Remove(generator.getGeneratorKey());
    }

    public WaveGenerator GetWaveGenerator(ulong generatorId)
    {
        if (_generators.ContainsKey(generatorId))
            return _generators[generatorId];
        return null;
    }

    public void SetWaveAmplitude(float waveAmplitude)
    {
        this.waveAmplitude = waveAmplitude;

        foreach (var generator in _generators.Values)
            generator.WaveAmplitude = waveAmplitude;


        var generators = slitPlate.GetComponentsInChildren<WaveGenerator>();
        foreach (var generator in generators)
        {
            generator.WaveAmplitude = waveAmplitude / generators.Length;
        }
    }

    public void SetWaveAmplitude(float waveAmplitude, ulong generatorId)
    {
        if (_generators.ContainsKey(generatorId))
            _generators[generatorId].WaveAmplitude = waveAmplitude;
    }

    public void SetWaveLength(float waveLength)
    {
        this.waveLength = waveLength;

        foreach (var generator in _generators.Values)
            generator.WaveLength = waveLength;
    }

    public void SetWaveLength(float waveLength, ulong generatorId)
    {
        if (_generators.ContainsKey(generatorId))
            _generators[generatorId].WaveLength = waveLength;
    }

    public void SetWaveFrequency(float waveFrequency)
    {
        this.waveFrequency = waveFrequency;

        foreach (var generator in _generators.Values)
            generator.WaveFrequency = waveFrequency;
    }

    public void SetWaveFrequency(float waveFrequency, ulong generatorId)
    {
        if (_generators.ContainsKey(generatorId))
            _generators[generatorId].WaveFrequency = waveFrequency;
    }

    public WaveGenerator CreateWaveGenerator(WaveGenerator.WavePropagation propagationMode)
    {
        var waveGenerator = new GameObject("WaveGenerator");
        var waveGeneratorScript = waveGenerator.AddComponent<WaveGenerator>();
        waveGeneratorScript.WaveAmplitude = waveAmplitude;
        waveGeneratorScript.WaveLength = waveLength;
        waveGeneratorScript.WaveFrequency = waveFrequency;
        waveGeneratorScript.SetPropagationMode(propagationMode);

        AddWaveGenerator(waveGeneratorScript);
        return waveGeneratorScript;
    }
}
