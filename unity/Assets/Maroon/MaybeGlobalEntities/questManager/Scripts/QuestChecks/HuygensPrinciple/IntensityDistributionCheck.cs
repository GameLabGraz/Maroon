using System;
using Maroon.Physics.HuygensPrinciple;
using UnityEngine;

namespace QuestManager
{
    public class IntensityDistributionCheck : IQuestCheck
    {
        private SlitPlate _slitPlate;
        private WaveGeneratorPoolHandler _wavePoolHandler;

        protected float tolerance = 0.10f;

        protected int taregetNumberOfSlits = 2;
        protected float targetSlitWidth = 0.3f;
        protected float targetWaveLength = 0.2f;
        protected Vector3 targetSlitPlatePosition = new Vector3(-0.51f, 0.63f, 1.61f);

        protected override void InitCheck()
        {
            _slitPlate = FindObjectOfType<SlitPlate>();
            if (_slitPlate == null) throw new NullReferenceException("There is no slit plate in the scene.");

            _wavePoolHandler = FindObjectOfType<WaveGeneratorPoolHandler>();
            if (_wavePoolHandler == null) throw new NullReferenceException("There is no wave generator pool handler in the scene.");
        }

        protected override bool CheckCompliance()
        {
            return taregetNumberOfSlits == _slitPlate.NumberOfSlits &&
                   targetSlitWidth * (1 - tolerance) <= _slitPlate.SlitWidth &&
                   _slitPlate.SlitWidth <= targetSlitWidth * (1 + tolerance) &&
                   targetWaveLength * (1 - tolerance) <= _wavePoolHandler.WaveLength &&
                   _wavePoolHandler.WaveLength <= targetWaveLength * (1 + tolerance) &&
                   Vector3.Distance(_slitPlate.transform.position, targetSlitPlatePosition) < tolerance;
        }
    }
}
