using System.Collections.Generic;
using UnityEngine;

public class WaveGeneratorPoolHandler : MonoBehaviour
{

    private ulong generatorIdCount = 0;
    private Dictionary<ulong, WaveGenerator> mGenerators = new Dictionary<ulong, WaveGenerator>();

    private void Start()
    {
        // Find wave generators in scene
        WaveGenerator[] generators = GameObject.FindObjectsOfType<WaveGenerator>();
        foreach (WaveGenerator generator in generators)
            mGenerators.Add(generatorIdCount++, generator);
    }

    public void AddWaveGenerator(WaveGenerator generator)
    {
        mGenerators.Add(generatorIdCount++, generator);
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
}
