using UnityEngine;

namespace Maroon.ScoreView
{
    public class ScoreCollector : MonoBehaviour
    {
        [SerializeField] private ExperimentScore experimentScore;
        [SerializeField] private UnlockableDrawer unlockableDrawer;

        private void Start()
        {
            experimentScore.Score = 0;
        }

        public void CollectScore()
        {
            experimentScore.Score++;
            if(experimentScore.Score >= experimentScore.MaxScore / 3)
                unlockableDrawer.LockLevel = 1;
            else if (experimentScore.Score >= experimentScore.MaxScore * 2 / 3)
                unlockableDrawer.LockLevel = 2;
            else if (experimentScore.Score >= experimentScore.MaxScore)
                unlockableDrawer.LockLevel = 3;
            else
                unlockableDrawer.LockLevel = 0;
        }
    }
}
