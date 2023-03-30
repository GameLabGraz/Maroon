using System;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon.ScoreView
{
    [Serializable] public class ScoreCollectEvent : UnityEvent<ExperimentScore>{}

    public class ScoreCollector : MonoBehaviour
    {
        [SerializeField] private ExperimentScore experimentScore;

        public ScoreCollectEvent OnScoreCollect;

        private void Start()
        {
            experimentScore.Score = 0;
        }

        public void CollectScore()
        {
            experimentScore.Score++;
            OnScoreCollect?.Invoke(experimentScore);
        }
    }
}
