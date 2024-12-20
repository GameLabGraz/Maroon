using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizScore : MonoBehaviour
{
    private QuizManager _quizManager;

    [SerializeField] private Color localColor;

    private void Start()
    {
        if(_quizManager == null)
            _quizManager = FindObjectOfType<SortingController>().TheQuizManager;
        _quizManager.SetQuizScoreParent(gameObject);
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
    
    private string _name;

    public string Name
    {
        get { return _name; }
        set { 
            _name = value;
            nameText.text = _name;
        }
    }

    private void SetName(string playerName)
    {
        Name = playerName;
    }

    private QuizChoice _choice;

    public QuizChoice Choice
    {
        get { return _choice; }
        set { 
            _choice = value;
            if (_choice == QuizChoice.Nothing)
                choiceText.text = "";
            else
                choiceText.text = "?";
        }
    }


    public void ChooseAlgorithm(QuizChoice choice)
    {
        Choice = choice;
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
    
    public void ResetChoice()
    {
        Choice = QuizChoice.Nothing;
    }

    private int _score;

    public int Score
    {
        get { return _score; }
        set { 
            _score = value;
            scoreText.text = _score.ToString();
            transform.SetSiblingIndex(_quizManager.GetRanking(this));
        }
    }

    public void ResetScore()
    {
        Score = 0;
    }
    
    public void IncreaseScore()
    {
        Score++;
    }
}
