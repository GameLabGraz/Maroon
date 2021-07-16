using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon
{
    [Serializable]
    public class ExperimentScore
    {
        [SerializeField] private string experiment;
        [SerializeField] private int maxScore;
        [SerializeField] private int score;

        public UnityEvent OnScoreChange = new UnityEvent();

        public string Experiment => experiment;
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

    [CreateAssetMenu(menuName = ("Scriptable Objects/ExperimentScores"))]
    public class ExperimentScores : ScriptableObject, IEnumerable
    {
        [SerializeField] private ExperimentScore[] experimentScores;

        public ExperimentScore GetExperimentScore(string experiment)
        {
            return experimentScores.FirstOrDefault(experimentScore => 
                experimentScore.Experiment == experiment);
        }

        public IEnumerator GetEnumerator()
        {
            return experimentScores.GetEnumerator();
        }
    }
}
