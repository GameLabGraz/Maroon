using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [SerializeField] private GameObject scrollViewContent;

    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button resetButton;

    [SerializeField] private Color defaultButtonColor;
    [SerializeField] private Color selectedButtonColor;

    private bool _isServer;

    private QuizScore _localQuizScore;

    private List<QuizScore> _allScores = new List<QuizScore>();

    private void Start()
    {
        leftButton.onClick.AddListener(ChooseLeftAlgorithm);
        rightButton.onClick.AddListener(ChooseRightAlgorithm);
        resetButton.onClick.AddListener(FullReset);

        _isServer = NetworkServer.active;

        leftButton.image.color = defaultButtonColor;
        rightButton.image.color = defaultButtonColor;
    }

    public void SetLeftButtonText(string text)
    {
        leftButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }
    
    public void SetRightButtonText(string text)
    {
        rightButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    public void SetQuizScoreParent(GameObject quizScore)
    {
        quizScore.transform.SetParent(scrollViewContent.transform, false);
        _allScores.Add(quizScore.GetComponent<QuizScore>());
    }

    public void SetLocalQuizScore(GameObject local)
    {
        _localQuizScore = local.GetComponent<QuizScore>();
    }

    public void ChooseLeftAlgorithm()
    {
        _localQuizScore.ChooseAlgorithm(QuizScore.QuizChoice.Left);
        leftButton.image.color = selectedButtonColor;
        rightButton.image.color = defaultButtonColor;
    }
    
    public void ChooseRightAlgorithm()
    {
        _localQuizScore.ChooseAlgorithm(QuizScore.QuizChoice.Right);
        leftButton.image.color = defaultButtonColor;
        rightButton.image.color = selectedButtonColor;
    }

    public void SortingStarted()
    {
        ShowAllChoices();
        leftButton.interactable = false;
        rightButton.interactable = false;
    }

    private void ShowAllChoices()
    {
        foreach (var score in _allScores)
        {
            score.ShowChoice();
        }
    }

    public void CorrectChoice(QuizScore.QuizChoice correct)
    {
        if (_isServer)
        {
            foreach (var score in _allScores)
            {
                if(score.Choice == correct)
                    score.IncreaseScore();
            }
        }
    }

    public void ResetAllChoices()
    {
        if (_isServer)
        {
            foreach (var score in _allScores)
            {
                score.ResetChoice();
            }
        }
        leftButton.interactable = true;
        rightButton.interactable = true;
        leftButton.image.color = defaultButtonColor;
        rightButton.image.color = defaultButtonColor;
    }

    public void FullReset()
    {
        if (_isServer)
        {
            foreach (var score in _allScores)
            {
                score.ResetScore();
            }
        }
        ResetAllChoices();
    }
}
