using Maroon.GlobalEntities;
using UnityEngine;

namespace Maroon.ScoreView
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private GameObject experimentScoreViewPrefab;
        
        private void Start()
        {
            var scenes = SceneManager.Instance.ActiveSceneCategory.Scenes;
            foreach (var scene in scenes)
            {
                var experimentScore = Resources.Load<ExperimentScore>($"{scene.SceneNameWithoutPlatformExtension}ExperimentScore");
                if(experimentScore == null) continue;

                var experimentScoreViews = Instantiate(experimentScoreViewPrefab, this.transform).GetComponent<ExperimentScoreView>();
                experimentScoreViews.ExperimentScore = experimentScore;
            }
        }
    }
}