using System;
using GameLabGraz.LimeSurvey;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GameLabGraz.LimeSurvey.LimeSurveyEvents))]
public class LimeSurveyStateHandler : MonoBehaviour
{
    public enum LimeSurveyState
    {
        DoLogin,
        LoginFailed,
        LoginSuccessful,
        DoLoadQuestions,
        QuestionsFailed,
        QuestionsSuccessful,
        DoSubmit,
        SubmitFailed,
        SubmitSuccessful
    }
    
    
    [Header("Needed Objects")] 
    [SerializeField] protected GameObject QuestionnairePanel;
    [SerializeField] protected GameObject LoadingPanel;
    [SerializeField] protected TextMeshProUGUI LoadingText;
    [SerializeField] protected GameObject LoadingCircle;
    [SerializeField] protected NotificationPanel ErrorPanel;
    [SerializeField] protected NotificationPanel WarningPanel;
    [SerializeField] protected Button SubmitButton;

    protected LimeSurveyState _state = LimeSurveyState.DoLogin;

    public LimeSurveyState State
    {
        get => _state;
        set
        {
            if (value != _state)
            {
                _state = value;
                HandleStateChanged();
            }
        }
    }
    
    protected void Awake()
    {
        var events = GetComponent<LimeSurveyEvents>();

        events.OnStartLogin.AddListener(() => { State = LimeSurveyState.DoLogin; });
        events.OnLoggedIn.AddListener((sessionKey) => { State = LimeSurveyState.LoginSuccessful; });
        events.OnStartLoadQuestions.AddListener(() => { State = LimeSurveyState.DoLoadQuestions; });
        events.OnQuestionsLoaded.AddListener(() => { State = LimeSurveyState.QuestionsSuccessful; });
        events.OnStartSubmission.AddListener(() => { State = LimeSurveyState.DoSubmit; });
        events.OnSubmissionFinished.AddListener((responseID) => { State = LimeSurveyState.SubmitSuccessful; });
        
        events.OnWarning.AddListener((warning, detail) =>
        {
            if (WarningPanel != null)
                WarningPanel.SetNotification(warning, detail);
        });
        
        events.OnError.AddListener((error, detail) =>
        {
            if (ErrorPanel != null)
                ErrorPanel.SetNotification(error, detail);
            switch (State)
            {
                case LimeSurveyState.DoLogin: State = LimeSurveyState.LoginFailed; break;
                case LimeSurveyState.DoLoadQuestions: State = LimeSurveyState.QuestionsFailed; break;
                case LimeSurveyState.DoSubmit: State = LimeSurveyState.SubmitFailed; break;
                case LimeSurveyState.LoginSuccessful:
                case LimeSurveyState.QuestionsSuccessful:
                case LimeSurveyState.SubmitSuccessful:
                    Debug.Log($"[LimeSurveyStateHandler-ERROR]: Should never receive an error when in state {State}.");
                    break;
                case LimeSurveyState.LoginFailed:
                case LimeSurveyState.QuestionsFailed:
                case LimeSurveyState.SubmitFailed:
                    HandleStateChanged(); // do display the latest error
                    break; //simply produced two errors
            }
        });
    }

    protected void Start()
    {
        HandleStateChanged();
    }

    protected void HandleStateChanged()
    {
        switch (State)
        {
            case LimeSurveyState.DoLogin:
                QuestionnairePanel.SetActive(false);
                LoadingPanel.SetActive(true);
                LoadingCircle.SetActive(true);
                LoadingText.text = "Logging in...";
                break;
            case LimeSurveyState.LoginFailed:
                //error notification is already displayed
                break;
            case LimeSurveyState.LoginSuccessful:
                //do nothing
                break;
            case LimeSurveyState.DoLoadQuestions:
                QuestionnairePanel.SetActive(false);
                LoadingPanel.SetActive(true);
                LoadingCircle.SetActive(true);
                LoadingText.text = "Loading Questions...";
                break;
            case LimeSurveyState.QuestionsFailed:
                //error notification is already displayed
                break;
            case LimeSurveyState.QuestionsSuccessful:
                LoadingPanel.SetActive(false);
                QuestionnairePanel.SetActive(true); //show questions
                break;
            case LimeSurveyState.DoSubmit:
                QuestionnairePanel.SetActive(false);
                LoadingPanel.SetActive(true);
                LoadingCircle.SetActive(true);
                LoadingText.text = "Submitting Answers...";
                break;
            case LimeSurveyState.SubmitFailed:
                QuestionnairePanel.SetActive(true);
                LoadingPanel.SetActive(false);
                SubmitButton.interactable = true; 
                break;
            case LimeSurveyState.SubmitSuccessful:
                QuestionnairePanel.SetActive(false);
                LoadingPanel.SetActive(true);
                LoadingCircle.SetActive(true);
                LoadingText.text = "Answers have been submitted successfully.\nLoading next scene.";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
