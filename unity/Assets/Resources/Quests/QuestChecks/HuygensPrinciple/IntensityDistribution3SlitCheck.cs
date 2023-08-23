using UnityEngine;

namespace Quests
{
    public class IntensityDistribution3SlitCheck : IntensityDistributionCheck
    {
        protected override void InitCheck()
        {
            base.InitCheck();
            
            taregetNumberOfSlits = 3;
            targetSlitWidth = 0.1f;
            targetWaveLength = 0.3f;
            targetSlitPlatePosition = new Vector3(0.77f, 0.63f, 1.61f);
        }
    }
}
