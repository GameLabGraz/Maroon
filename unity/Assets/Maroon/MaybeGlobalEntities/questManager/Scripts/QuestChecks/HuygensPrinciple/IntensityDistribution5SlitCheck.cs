using UnityEngine;

namespace QuestManager
{
    public class IntensityDistribution5SlitCheck : IntensityDistributionCheck
    {
        protected override void InitCheck()
        {
            base.InitCheck();
            
            tolerance = 0.05f;
            taregetNumberOfSlits = 5;
            targetSlitWidth = 0.1f;
            targetWaveLength = 0.2f;
            targetSlitPlatePosition = new Vector3(-0.23f, 0.63f, 1.61f);
        }
    }
}
