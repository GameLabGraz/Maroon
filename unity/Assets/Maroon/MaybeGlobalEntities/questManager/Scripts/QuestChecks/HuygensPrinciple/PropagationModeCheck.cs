using System;
using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class WaveParameterCheck : IQuestCheck
    {
        private WaveGeneratorPoolHandler _wavePoolHandler;
        private float _initWaveAmplitude;
        private float _initWaveLength;
        private float _initWaveFrequency;


        protected override void InitCheck()
        {
            _wavePoolHandler = FindObjectOfType<WaveGeneratorPoolHandler>();

            if (_wavePoolHandler == null) throw new NullReferenceException("There is no wave generator pool handler in the scene.");

            _initWaveAmplitude = _wavePoolHandler.WaveAmplitude;
            _initWaveLength = _wavePoolHandler.WaveLength;
            _initWaveFrequency = _wavePoolHandler.WaveFrequency;
        }

        protected override bool CheckCompliance()
        {
            return Math.Abs(_initWaveAmplitude - _wavePoolHandler.WaveAmplitude) > Mathf.Epsilon ||
                   Math.Abs(_initWaveLength - _wavePoolHandler.WaveLength) > Mathf.Epsilon ||
                   Math.Abs(_initWaveFrequency - _wavePoolHandler.WaveFrequency) > Mathf.Epsilon;
        }
    }
}