using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.ScoreView
{
    public class ExperimentScoreView : MonoBehaviour
    {
        [SerializeField] private ExperimentScore _experimentScore;

        [SerializeField] private TMP_Text experimenNameText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private GameObject[] stars = new GameObject[3];

        public ExperimentScore ExperimentScore
        {
            get => _experimentScore;
            set
            {
                _experimentScore = value;
                _experimentScore?.OnScoreChange.AddListener(UpdateScoreView);
                UpdateScoreView();
            }
        }

        private void Start()
        {
            ExperimentScore?.OnScoreChange.AddListener(UpdateScoreView);
            UpdateScoreView();
        }

        public void UpdateScoreView()
        {
            if(ExperimentScore == null) return;
            for (var starIndex = 0; starIndex < 3; starIndex++)
            {
                var starColor = ExperimentScore.Score >= (ExperimentScore.MaxScore / 3f) * (starIndex + 1)
                    ? Color.yellow
                    : Color.gray;

                var starImage = stars[starIndex].GetComponent<Image>();
                if (starImage) starImage.color = starColor;

                var starRenderer = stars[starIndex].GetComponent<Renderer>();
                if (starRenderer) starRenderer.material.SetColor("_Color", starColor);
            }

            if (experimenNameText) experimenNameText.text = ExperimentScore.ExperimentName;
            if (scoreText) scoreText.text = $"{_experimentScore.Score}/{_experimentScore.MaxScore}";
        }
    }
}
