using UnityEngine;

namespace QuestManager
{
    public class IntensityDistribution3SlitCheck : IntensityDistributionCheck
    {
        protected override void InitCheck()
        {
            base.InitCheck();
            
            tolerance = 0.05f;
            taregetNumberOfSlits = 3;
            targetSlitWidth = 0.1f;
            targetWaveLength = 0.3f;
            targetSlitPlatePosition = new Vector3(0.77f, 0.63f, 1.61f);
        }
    }
}
