using UnityEngine;

namespace QuestManager
{
    public class IntensityDistribution5SlitCheck : IntensityDistributionCheck
    {
        protected override void InitCheck()
        {
            base.InitCheck();
            
            taregetNumberOfSlits = 5;
            targetSlitWidth = 0.1f;
            targetWaveLength = 0.2f;
            targetSlitPlatePosition = new Vector3(-0.23f, 0.63f, 1.61f);
        }
    }
}
