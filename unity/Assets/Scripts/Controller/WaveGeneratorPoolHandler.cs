using System.Collections.Generic;
using UnityEngine;

public class WaveGeneratorPoolHandler : MonoBehaviour
{

    private ulong generatorIdCount = 0;
    private Dictionary<ulong, WaveGenerator> mGenerators = new Dictionary<ulong, WaveGenerator>();
    private GameObject[] mSlitPlates;

    private static WaterPlane _waterPlane_instance;
  
    public static WaterPlane Instance
    {
        get
        {
            if (_waterPlane_instance == null)
                _waterPlane_instance = FindObjectOfType<WaterPlane>();
            return _waterPlane_instance;
        }
    }

    private void Start()
    {
        WaveGenerator basinWaveGenerator = GameObject.Find("WaveGeneratorBasin").GetComponent<WaveGenerator>();
        mGenerators.Add(generatorIdCount++, basinWaveGenerator);
        Instance.RegisterWaveGenerator(basinWaveGenerator);
    }

    public void AddWaveGenerator(WaveGenerator generator)
    {
        generator.setGeneratorKey(generatorIdCount);
        mGenerators.Add(generatorIdCount++, generator);
        Instance.RegisterWaveGenerator(generator);
    }

    public void RemoveWaveGenerator(WaveGenerator generator)
    {
        Instance.UnregisterWaveGenerator(generator);
        mGenerators.Remove(generator.getGeneratorKey());
    }



    public WaveGenerator GetWaveGenerator(ulong generatorId)
    {
        if (mGenerators.ContainsKey(generatorId))
            return mGenerators[generatorId];
        return null;
    }

    public void SetWaveAmplitude(float waveAmplitude)
    {
        foreach (WaveGenerator generator in mGenerators.Values)
            generator.WaveAmplitude = waveAmplitude;

        foreach (GameObject slitPlate in mSlitPlates)
        {
            WaveGenerator[] generators = slitPlate.GetComponentsInChildren<WaveGenerator>();
            foreach (WaveGenerator generator in generators)
                generator.WaveAmplitude = waveAmplitude / generators.Length;
        }
    }

    public void SetWaveAmplitude(float waveAmplitude, ulong generatorId)
    {
        if (mGenerators.ContainsKey(generatorId))
            mGenerators[generatorId].WaveAmplitude = waveAmplitude;
    }

    public void SetWaveLength(float waveLength)
    {
        foreach (WaveGenerator generator in mGenerators.Values)
            generator.WaveLength = waveLength;
    }

    public void SetWaveLength(float waveLength, ulong generatorId)
    {
        if (mGenerators.ContainsKey(generatorId))
            mGenerators[generatorId].WaveLength = waveLength;
    }

    public void SetWaveFrequency(float waveFrequency)
    {
        foreach (WaveGenerator generator in mGenerators.Values)
            generator.WaveFrequency = waveFrequency;
    }

    public void SetWaveFrequency(float waveFrequency, ulong generatorId)
    {
        if (mGenerators.ContainsKey(generatorId))
            mGenerators[generatorId].WaveFrequency = waveFrequency;
    }

    public WaveGenerator CreateWaveGenerator()
    {
        var waveGenerator = new GameObject("WaveGenerator");
        WaveGenerator waveGeneratorScript = waveGenerator.AddComponent<WaveGenerator>();
        waveGeneratorScript.WaveAmplitude = 0.5f;
        waveGeneratorScript.WaveLength = 0.25f;
        waveGeneratorScript.WaveFrequency = 0.5f;
        waveGeneratorScript.SetPropagationMode(WaveGenerator.WavePropagation.Circular);

        AddWaveGenerator(waveGeneratorScript);
        return waveGeneratorScript;
    }
}
