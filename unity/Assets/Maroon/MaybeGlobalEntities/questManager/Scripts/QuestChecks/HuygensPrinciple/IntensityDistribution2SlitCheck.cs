using UnityEngine;

namespace QuestManager
{
    public class IntensityDistribution2SlitCheck : IntensityDistributionCheck
    {
        protected override void InitCheck()
        {
            base.InitCheck();
            
            tolerance = 0.05f;
            targetSlitWidth = 0.3f;
            targetWaveLength = 0.2f;
            targetSlitPlatePosition = new Vector3(-0.51f, 0.63f, 1.61f);
        }
    }
}
