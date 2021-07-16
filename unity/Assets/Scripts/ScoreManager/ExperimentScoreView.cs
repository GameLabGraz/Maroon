using Maroon;
using TMPro;
using UnityEngine;

namespace MyNamespace
{
    public class ExperimentScoreView : MonoBehaviour
    {
        [SerializeField] private ExperimentScores experimentScores;
        [SerializeField] private string experiment;

        [SerializeField] private GameObject[] stars = new GameObject[3];
        [SerializeField] private TMP_Text scoreText;

        private ExperimentScore _experimentScore;

        private void Start()
        {
            _experimentScore = experimentScores.GetExperimentScore(experiment);
            _experimentScore.OnScoreChange.AddListener(UpdateScoreView);
            UpdateScoreView();
        }

        public void UpdateScoreView()
        {
            for (var starIndex = 0; starIndex < 3; starIndex++)
            {
                stars[starIndex].GetComponent<Renderer>().material.
                    SetColor("_Color",
                        _experimentScore.Score >= (_experimentScore.MaxScore / 3) * (starIndex + 1)
                            ? Color.yellow
                            : Color.gray);
            }

            if (scoreText != null) scoreText.text = $"{_experimentScore.Score}/{_experimentScore.MaxScore}";
        }
    }
}