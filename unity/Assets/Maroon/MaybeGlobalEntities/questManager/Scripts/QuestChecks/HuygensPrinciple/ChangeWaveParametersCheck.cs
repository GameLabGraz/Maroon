using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class ChangeWaveParametersCheck : IQuestCheck
    {
        private float _initWaveLength;
        private float _initWaveFrequency;
        private float _initWaveAmplitude;

        private readonly float _targetDifference = 0.1f;

        protected override void InitCheck()
        {
            _initWaveLength = WaveGeneratorPoolHandler.Instance.WaveLength;
            _initWaveFrequency = WaveGeneratorPoolHandler.Instance.WaveFrequency;
            _initWaveAmplitude = WaveGeneratorPoolHandler.Instance.WaveAmplitude;
        }

        protected override bool CheckCompliance()
        {
            return Mathf.Abs(_initWaveAmplitude - WaveGeneratorPoolHandler.Instance.WaveAmplitude) > _targetDifference &&
                   Mathf.Abs(_initWaveFrequency - WaveGeneratorPoolHandler.Instance.WaveFrequency) > _targetDifference &&
                   Mathf.Abs(_initWaveLength - WaveGeneratorPoolHandler.Instance.WaveLength) > _targetDifference;
        }
    }
}