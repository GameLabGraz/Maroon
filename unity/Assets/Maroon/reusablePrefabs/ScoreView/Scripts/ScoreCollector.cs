using UnityEngine;

namespace Maroon.ScoreView
{
    public class ScoreCollector : MonoBehaviour
    {
        [SerializeField] private ExperimentScore experimentScore;

        private void Start()
        {
            experimentScore.Score = 0;
        }

        public void CollectScore()
        {
            experimentScore.Score++;
        }
    }
}
