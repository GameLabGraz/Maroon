using Maroon;
using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    [SerializeField] private string experiment;
    [SerializeField] private ExperimentScores experimentScores;

    private ExperimentScore _experimentScore;

    private void Start()
    {
        _experimentScore = experimentScores.GetExperimentScore(experiment);
        _experimentScore.Score = 0;
    }

    public void CollectCoin(GameObject coin)
    {
        _experimentScore.Score++;
        GameObject.Destroy(coin);
    }
}
