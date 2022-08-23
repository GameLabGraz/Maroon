using System;
using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class PropagationModeCheck : IQuestCheck
    {
        private WaveGeneratorPoolHandler _wavePoolHandler;
        private WaveGeneratorPoolHandler.WavePropagation _initWaveAmplitude;


        protected override void InitCheck()
        {
            _wavePoolHandler = FindObjectOfType<WaveGeneratorPoolHandler>();

            if (_wavePoolHandler == null) throw new NullReferenceException("There is no wave generator pool handler in the scene.");

            _initWaveAmplitude = _wavePoolHandler.WavePropagationMode;
        }

        protected override bool CheckCompliance()
        {
            return _initWaveAmplitude != _wavePoolHandler.WavePropagationMode;
        }
    }
}