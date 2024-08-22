using System.Collections.Generic;
using System.Linq;
using GEAR.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [SerializeField] private GameObject scrollViewContent;

    [SerializeField] private Button leftOfflineButton;
    [SerializeField] private Button rightOfflineButton;
    [SerializeField] private Button resetOfflineButton;
    
    [SerializeField] private TextMeshProUGUI offlinePointsText;

    [SerializeField] private Color defaultButtonColor;
    [SerializeField] private Color selectedButtonColor;

    private QuizScore _localQuizScore;

    private List<QuizScore> _allScores = new List<QuizScore>();

    private void Start()
    {
        leftOfflineButton.onClick.AddListener(ChooseLeftAlgorithmOffline);
        rightOfflineButton.onClick.AddListener(ChooseRightAlgorithmOffline);
        resetOfflineButton.onClick.AddListener(FullReset);

        leftOfflineButton.image.color = defaultButtonColor;
        rightOfflineButton.image.color = defaultButtonColor;
        
        LanguageManager.Instance.OnLanguageChanged.AddListener(DisplayOfflineScore);
        DisplayOfflineScore();
    }

    public void SetLeftButtonText(string text)
    {
        leftOfflineButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }
    
    public void SetRightButtonText(string text)
    {
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
        _offlineChoice = QuizScore.QuizChoice.Nothing;
        
        leftOfflineButton.interactable = true;
        rightOfflineButton.interactable = true;
        leftOfflineButton.image.color = defaultButtonColor;
        rightOfflineButton.image.color = defaultButtonColor;
    }

    public void FullReset()
    {
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
