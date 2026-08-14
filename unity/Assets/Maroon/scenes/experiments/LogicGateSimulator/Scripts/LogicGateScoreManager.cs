using Maroon.ScoreView;
using UnityEngine;

public class LogicGateScoreManager : MonoBehaviour
{
    public ScoreCollector scoreCollector;
    static ExperimentScore scoreKeptInMemory;

    bool[] solvedPuzzles = new bool[3];


    void Awake()
    {
        scoreKeptInMemory = Resources.Load<ExperimentScore>("LogicGateSimulatorExperimentScore");

        if (scoreKeptInMemory == null)
        {
            Debug.LogError("LogicGateSimulatorExperimentScore could not be loaded.");
        }
    }

    public void PuzzleSolved(int puzzleNumber)
    {
   
        int puzzleIndex = puzzleNumber - 1;

        if (solvedPuzzles[puzzleIndex])
        {
            return;
        }

        solvedPuzzles[puzzleIndex] = true;

        for (int i = 0; i < 3; i++)
        {
            scoreCollector.CollectScore();
        }
    }
}