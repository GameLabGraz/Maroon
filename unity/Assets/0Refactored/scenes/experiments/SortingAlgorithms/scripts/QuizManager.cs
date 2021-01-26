using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GEAR.Localization;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [SerializeField] private GameObject scrollViewContent;

    [SerializeField] private Button leftOnlineButton;
    [SerializeField] private Button rightOnlineButton;
    [SerializeField] private Button resetOnlineButton;
    
    [SerializeField] private Button leftOfflineButton;
    [SerializeField] private Button rightOfflineButton;
    [SerializeField] private Button resetOfflineButton;
    
    [SerializeField] private TextMeshProUGUI offlinePointsText;

    [SerializeField] private Color defaultButtonColor;
    [SerializeField] private Color selectedButtonColor;

    private bool _isServer;

    private QuizScore _localQuizScore;

    private List<QuizScore> _allScores = new List<QuizScore>();

    private void Start()
    {
        leftOnlineButton.onClick.AddListener(ChooseLeftAlgorithmOnline);
        rightOnlineButton.onClick.AddListener(ChooseRightAlgorithmOnline);
        resetOnlineButton.onClick.AddListener(FullReset);
        
        leftOfflineButton.onClick.AddListener(ChooseLeftAlgorithmOffline);
        rightOfflineButton.onClick.AddListener(ChooseRightAlgorithmOffline);
        resetOfflineButton.onClick.AddListener(FullReset);

        _isServer = NetworkServer.active;

        leftOnlineButton.image.color = defaultButtonColor;
        rightOnlineButton.image.color = defaultButtonColor;
        
        leftOfflineButton.image.color = defaultButtonColor;
        rightOfflineButton.image.color = defaultButtonColor;
        
        LanguageManager.Instance.OnLanguageChanged.AddListener(DisplayOfflineScore);
        DisplayOfflineScore();
    }

    public void SetLeftButtonText(string text)
    {
        leftOnlineButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
        leftOfflineButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }
    
    public void SetRightButtonText(string text)
    {
        rightOnlineButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
        rightOfflineButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
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

    public void ChooseLeftAlgorithmOnline()
    {
        _localQuizScore.ChooseAlgorithm(QuizScore.QuizChoice.Left);
        leftOnlineButton.image.color = selectedButtonColor;
        rightOnlineButton.image.color = defaultButtonColor;
    }
    
    public void ChooseRightAlgorithmOnline()
    {
        _localQuizScore.ChooseAlgorithm(QuizScore.QuizChoice.Right);
        leftOnlineButton.image.color = defaultButtonColor;
        rightOnlineButton.image.color = selectedButtonColor;
    }

    private QuizScore.QuizChoice _offlineChoice;
    private int _offlineScore;
    private int _offlineTries;
    
    public void ChooseLeftAlgorithmOffline()
    {
        _offlineChoice = QuizScore.QuizChoice.Left;
        leftOfflineButton.image.color = selectedButtonColor;
        rightOfflineButton.image.color = defaultButtonColor;
    }
    
    public void ChooseRightAlgorithmOffline()
    {
        _offlineChoice = QuizScore.QuizChoice.Right;
        leftOfflineButton.image.color = defaultButtonColor;
        rightOfflineButton.image.color = selectedButtonColor;
    }

    public void SortingStarted()
    {
        ShowAllChoices();
        leftOnlineButton.interactable = false;
        rightOnlineButton.interactable = false;
        
        leftOfflineButton.interactable = false;
        rightOfflineButton.interactable = false;
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

        if (_offlineChoice == QuizScore.QuizChoice.Nothing)
            return;

        _offlineTries++;
        if (_offlineChoice == correct)
        {
            _offlineScore++;
        }

        DisplayOfflineScore();
    }

    public int GetRanking(QuizScore score)
    {
        _allScores = _allScores.OrderBy(i => i.Score).ToList();
        _allScores.Reverse();

        return _allScores.IndexOf(score);
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
        leftOnlineButton.interactable = true;
        rightOnlineButton.interactable = true;
        leftOnlineButton.image.color = defaultButtonColor;
        rightOnlineButton.image.color = defaultButtonColor;

        _offlineChoice = QuizScore.QuizChoice.Nothing;
        
        leftOfflineButton.interactable = true;
        rightOfflineButton.interactable = true;
        leftOfflineButton.image.color = defaultButtonColor;
        rightOfflineButton.image.color = defaultButtonColor;
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
        
        _offlineScore = 0;
        _offlineTries = 0;
        DisplayOfflineScore();
    }

    private void DisplayOfflineScore(SystemLanguage language = SystemLanguage.English)
    {
        offlinePointsText.text = _offlineScore + " / " + _offlineTries + LanguageManager.Instance.GetString("PointsPostfix");
    }
}
