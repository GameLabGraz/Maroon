using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveGeneratorPoolHandler : MonoBehaviour
{

    public enum WavePropagation { Rectilinear, Circular }

    [SerializeField] private float waveAmplitude;
    [SerializeField] private float waveLength;
    [SerializeField] private float waveFrequency;
    [SerializeField] private int numberOfBasinGenerators = 1;
    [SerializeField] private WaterPlane waterPlane;
    [SerializeField] private WavePropagation wavePropagationMode;
    [SerializeField] private GameObject waterBasinGeneratorPosition;
   
    private ulong _generatorIdCount;
    private Dictionary<ulong, WaveGenerator> _generators = new Dictionary<ulong, WaveGenerator>();
    private float _waterPlaneWidth = 1.2f;

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
    public float WaveFrequency => waveFrequency;
    public float WaveAmplitude
    {
        get => waveAmplitude;
        set
        {
            waveAmplitude = value;
            UpdateAmplitudes(WaveGenerator.GeneratorMembership.WaterBasin);
            UpdateAmplitudes(WaveGenerator.GeneratorMembership.SlitPlate1);
        }
    }

    public WavePropagation WavePropagationMode => wavePropagationMode;

    private void Awake()
    {
        CreateWaterBasinGenerators();
    }

    public void CreateWaterBasinGenerators()
    {      
        for (var count = 0; count < numberOfBasinGenerators; count++)
        {
            var waterBasinGenerator = CreateWaveGenerator(WaveGenerator.GeneratorMembership.WaterBasin);
            waterBasinGenerator.transform.parent = waterBasinGeneratorPosition.transform;

            var coordinates = new Vector3(waterBasinGeneratorPosition.transform.position.x, waterBasinGeneratorPosition.transform.position.y, waterBasinGeneratorPosition.transform.position.z + _waterPlaneWidth / 2 - count * _waterPlaneWidth / numberOfBasinGenerators);
            waterBasinGenerator.transform.position = coordinates;
        }
    }

    public void SetPropagationMode(bool isCircular)
    {
        wavePropagationMode = isCircular ? WavePropagation.Circular : WavePropagation.Rectilinear;
        ChangePropagationMode();
    }
    
    public void SetPropagationMode(int propagationMode)
    {
        wavePropagationMode = (WavePropagation)propagationMode;
        ChangePropagationMode();
    }

    public void SetPropagationMode(object o)
    {
        wavePropagationMode = o.ToString() == "1" ? WavePropagation.Circular : WavePropagation.Rectilinear;
        ChangePropagationMode();
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
        return _generators.ContainsKey(generatorId) ? _generators[generatorId] : null;
    }

    public void SetWaveLength(float waveLength)
    {
        this.waveLength = waveLength;

        foreach (var generator in _generators.Values)
            generator.WaveLength = waveLength;
    }


    public void SetWaveFrequency(float waveFrequency)
    {
        this.waveFrequency = waveFrequency;

        foreach (var generator in _generators.Values)
            generator.WaveFrequency = waveFrequency;
    }

    public WaveGenerator CreateWaveGenerator(WaveGenerator.GeneratorMembership membership)
    {
        var waveGenerator = new GameObject("WaveGenerator");
        var waveGeneratorScript = waveGenerator.AddComponent<WaveGenerator>();
        waveGeneratorScript.WaveLength = waveLength;
        waveGeneratorScript.WaveFrequency = waveFrequency;
        waveGeneratorScript.setGeneratorMembership(membership);
       
        AddWaveGenerator(waveGeneratorScript);
        UpdateAmplitudes(membership);

        return waveGeneratorScript;
    }

    public List<WaveGenerator> GetGeneratorListOfType(WaveGenerator.GeneratorMembership membership)
    {
        var generatorList = new List<WaveGenerator>();
        foreach (var generator in _generators.Values)
        {
            if (generator.getGeneratorMembership() == membership)
            {
                generatorList.Add(generator);
            }
        }
        return generatorList;
    }

    private void UpdateAmplitudes(WaveGenerator.GeneratorMembership membership)
    {
        var generatorList = GetGeneratorListOfType(membership);
        var amplitude = waveAmplitude / generatorList.Count;

        //Debug.Log(amplitude + " " + generatorList.Count);

        foreach (var generator in generatorList)
        {
            generator.WaveAmplitude = amplitude;
        }
    }

    private void ChangePropagationMode()
    {
        RemoveBasinGenerators();

        if (wavePropagationMode == WavePropagation.Circular)
        {
            var waterBasinGenerator = CreateWaveGenerator(WaveGenerator.GeneratorMembership.WaterBasin);
            waterBasinGenerator.transform.parent = waterBasinGeneratorPosition.transform;

            var coordinates = new Vector3(waterBasinGeneratorPosition.transform.position.x, waterBasinGeneratorPosition.transform.position.y, waterBasinGeneratorPosition.transform.position.z); //+ 0.1f - count * 0.1f/(numberOfBasinGenerators/2));
            waterBasinGenerator.transform.position = coordinates;

        }
        else
        {
            for (var count = 0; count < numberOfBasinGenerators; count++)
            {
                var waterBasinGenerator = CreateWaveGenerator(WaveGenerator.GeneratorMembership.WaterBasin);
                waterBasinGenerator.transform.parent = waterBasinGeneratorPosition.transform;

                var coordinates = new Vector3(waterBasinGeneratorPosition.transform.position.x, waterBasinGeneratorPosition.transform.position.y, waterBasinGeneratorPosition.transform.position.z + _waterPlaneWidth / 2 - count * _waterPlaneWidth / numberOfBasinGenerators);
                waterBasinGenerator.transform.position = coordinates;
            }
        }

        waterPlane.UpdatePlane();
    }

    private void RemoveBasinGenerators()
    {
        var keys = _generators.Keys.ToArray();

        foreach(var key in keys)
        {
           if((_generators[key]).getGeneratorMembership() == WaveGenerator.GeneratorMembership.WaterBasin)
           {            
                Destroy(_generators[key].gameObject);
                RemoveWaveGenerator(_generators[key]);            
           }
        }       
    }
}