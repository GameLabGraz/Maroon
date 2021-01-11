using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizScore : NetworkBehaviour
{
    private QuizManager _quizManager;

    [SerializeField] private Color localColor;

    private void Start()
    {
        if(_quizManager == null)
            _quizManager = FindObjectOfType<SortingController>().TheQuizManager;
        _quizManager.SetQuizScoreParent(gameObject);
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        
        CmdSetName(MaroonNetworkManager.Instance.PlayerName);
        if(_quizManager == null)
            _quizManager = FindObjectOfType<SortingController>().TheQuizManager;
        _quizManager.SetLocalQuizScore(gameObject);

        GetComponent<Image>().color = localColor;
    }

    public enum QuizChoice
    {
        Nothing,
        Left,
        Right
    }
    
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI choiceText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [SyncVar(hook = "OnNameChanged")] private string _name;
    public string Name  => _name;

    [Command]
    private void CmdSetName(string playerName)
    {
        _name = playerName;
    }

    private void OnNameChanged(string oldName, string newName)
    {
        nameText.text = newName;
    }

    [SyncVar(hook = "OnChoiceChanged")] private QuizChoice _choice;
    public QuizChoice Choice => _choice;

    public void ChooseAlgorithm(QuizChoice choice)
    {
        CmdChooseAlgorithm(choice);
    }

    [Command]
    private void CmdChooseAlgorithm(QuizChoice choice)
    {
        _choice = choice;
    }

    private void OnChoiceChanged(QuizChoice oldChoice, QuizChoice newChoice)
    {
        if(newChoice == QuizChoice.Nothing)
            choiceText.text = "";
        else
            choiceText.text = "?";
    }

    public void ShowChoice()
    {
        switch (_choice)
        {
            case QuizChoice.Nothing:
                choiceText.text = "";
                break;
            case QuizChoice.Left:
                choiceText.text = "L";
                break;
            case QuizChoice.Right:
                choiceText.text = "R";
                break;
        }
    }
    
    [Server]
    public void ResetChoice()
    {
        _choice = QuizChoice.Nothing;
    }

    [SyncVar(hook = "OnScoreChanged")] private int _score;
    public int Score => _score;

    [Server]
    public void ResetScore()
    {
        _score = 0;
    }
    
    [Server]
    public void IncreaseScore()
    {
        _score++;
    }

    private void OnScoreChanged(int oldScore, int newScore)
    {
        scoreText.text = newScore.ToString();
    }
}
