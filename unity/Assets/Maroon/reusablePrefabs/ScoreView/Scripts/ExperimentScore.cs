using UnityEngine;
using UnityEngine.Events;

namespace Maroon.ScoreView
{
    [CreateAssetMenu(menuName = ("Scriptable Objects/ExperimentScore"))]
    public class ExperimentScore : ScriptableObject
    {
        [SerializeField] private string experimentName;
        [SerializeField] private int maxScore;
        [SerializeField] private int score;

        public UnityEvent OnScoreChange = new UnityEvent();

        public string ExperimentName => experimentName;
        public int MaxScore => maxScore;

        public int Score
        {
            get => score;
            set
            {
                score = value;
                OnScoreChange.Invoke();
            }
        }
    }
}
