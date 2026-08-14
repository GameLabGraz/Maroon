using UnityEngine;

public class PuzzleGoal : MonoBehaviour
{
    public int puzzleNumber = 1;
    public InputS[] ledInputs;

    LogicGateScoreManager scoreManager;
    bool puzzleReported = false;

    void Start()
    {
        scoreManager = FindObjectOfType<LogicGateScoreManager>();
    }

    void Update()
    {
        if (puzzleReported)
        {
            return;
        }

        if (scoreManager == null)
        {
            return;
        }

        if (ledInputs == null || ledInputs.Length == 0)
        {
            return;
        }

        for (int i = 0; i < ledInputs.Length; i++)
        {
            if (ledInputs[i] == null)
            {
                return;
            }

	    if (ledInputs[i].InputValue != 1)
	    {
    		return;
	    }
        }

        puzzleReported = true;
        scoreManager.PuzzleSolved(puzzleNumber);
    }
}