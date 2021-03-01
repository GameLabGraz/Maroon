using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GEAR.Localization;
using Maroon.UI;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using Dropdown = Maroon.UI.Dropdown;
using Random = System.Random;
using Slider = Maroon.UI.Slider;

public class SortingController : MonoBehaviour, IResetObject
{
    private enum SortingMode
    {
        SM_DetailMode,
        SM_BattleMode
    }
    private SortingMode _sortingMode;

    [SerializeField] private SortingNetworkSync networkSync;
    private bool _isOnline;
    private bool _initialized;

    [SerializeField] private QuizManager quizManager;
    public QuizManager TheQuizManager => quizManager;

    private DialogueManager _dialogueManager;
    private StudyTracker _studyTracker;

    private void Start()
    {
        _isOnline = NetworkClient.active;
        detailSortingLogic.Init(detailArraySize);
        leftBattleSorting.Init(battleArraySize, this);
        rightBattleSorting.Init(battleArraySize, this);
        RandomizeDetailArray();
        SetDetailArraySize(detailArraySize);
        EnterDetailMode();
        arrangementDropdown.AllowReset = false;
        _arrangementMode = (ArrangementMode)arrangementDropdown.value;
        SetBattleOperationsPerSeconds(speedSlider.value);
        
        SimulationController.Instance.onStartRunning.AddListener(SortingStarted);
        
        DisplayMessageByKey("EnterSortingExperiment");
        
        //Only after delay, so all clients are ready
        Invoke(nameof(DistributeDetailArray), 1.0f);
        Invoke(nameof(SetBattleOrder), 1.0f);
        _initialized = true;

        _studyTracker = FindObjectOfType<StudyTracker>();
    }

    public void GoOffline()
    {
        _isOnline = false;
        battleQuizOnline.SetActive(false);
        if (_sortingMode == SortingMode.SM_BattleMode)
        {
            battleQuizOffline.SetActive(true);
            ResetQuiz();
        }
    }
    
    public void ResetObject()
    {
        switch (_sortingMode)
        {
            case SortingMode.SM_DetailMode:
                RandomizeDetailArray();
                break;
            case SortingMode.SM_BattleMode:
                SetBattleOrder();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        ResetQuiz();
    }

    private void DisplayMessageByKey(string key)
    {
        if (_dialogueManager == null)
            _dialogueManager = FindObjectOfType<DialogueManager>();

        if (_dialogueManager == null)
            return;
        
        var message = LanguageManager.Instance.GetString(key);

        _dialogueManager.ShowMessage(message);
    }

    #region DetailMode

    private List<int> _detailOrder;

    [Header("Detail Mode")]
    [SerializeField] private int detailArraySize = 8;
    [SerializeField] private GameObject detailSortingArray;
    [SerializeField] private GameObject detailOptionsUi;
    [SerializeField] private GameObject detailDescriptionUi;
    [SerializeField] private GameObject forwardButton;
    [SerializeField] private GameObject backwardButton;
    
    [SerializeField] private SortingLogic detailSortingLogic;

    private float _enterTime;
    
    public void EnterDetailMode(int chosenAlgorithm = -1)
    {
        _enterTime = Time.time;
        
        _sortingMode = SortingMode.SM_DetailMode;
        
        detailSortingArray.SetActive(true);
        battleArrays.SetActive(false);

        detailOptionsUi.SetActive(true);
        battleOptionsUi.SetActive(false);
        
        detailDescriptionUi.SetActive(true);
        battleQuizOnline.gameObject.SetActive(false);
        battleQuizOffline.gameObject.SetActive(false);
        ResetQuiz();
        
        battlePanels.SetActive(false);
        
        forwardButton.SetActive(true);
        backwardButton.SetActive(true);

        if (chosenAlgorithm != -1)
        {
            detailSortingArray.GetComponent<SortingLogic>().SetAlgorithmDropdown(chosenAlgorithm);
        }
        
        detailSortingArray.GetComponent<DetailSortingVisualization>().ResetVisualization();
    }
    
    public void SetDetailArraySize(float size)
    {
        detailArraySize = (int)size;
        detailSortingLogic.SortingValues = _detailOrder.GetRange(0, detailArraySize);
    }
    
    private void RandomizeDetailArray()
    {
        if (_initialized && _isOnline && !Maroon.NetworkManager.Instance.IsInControl)
            return;
        _detailOrder = new List<int>();
        for (int i = 0; i < 10; ++i)
        {
            _detailOrder.Add(rng.Next(100));
        }
        detailSortingLogic.SortingValues = _detailOrder.GetRange(0, detailArraySize);
        
        DistributeDetailArray();
    }

    private void DistributeDetailArray()
    {
        if(_initialized && _isOnline && Maroon.NetworkManager.Instance.IsInControl)
            networkSync.SynchronizeArray(_detailOrder);
    }

    public void SetDetailArray(List<int> array)
    {
        _detailOrder = array;
        detailSortingLogic.SortingValues = _detailOrder.GetRange(0, detailArraySize);
    }

    #endregion

    #region BattleMode
    
    [Header("Battle Mode")]
    [SerializeField] private int battleArraySize;

    private enum ArrangementMode
    {
        AM_Random,
        AM_Sorted,
        AM_Reversed
    }
    private ArrangementMode _arrangementMode;
    [SerializeField] private Dropdown arrangementDropdown;
    
    [SerializeField] private Dropdown battleAlgorithmDropdownLeft;
    [SerializeField] private Dropdown battleAlgorithmDropdownRight;
    
    [SerializeField] private Slider speedSlider;
    
    [SerializeField] private GameObject battleArrays;
    [SerializeField] private GameObject battleOptionsUi;
    [SerializeField] private GameObject battleQuizOnline;
    [SerializeField] private GameObject battleQuizOffline;
    [SerializeField] private GameObject battlePanels;
    
    [SerializeField] private BattleSorting leftBattleSorting;
    [SerializeField] private BattleSorting rightBattleSorting;

    private bool _enteredOnce;
    public void EnterBattleMode()
    {
        if(_sortingMode == SortingMode.SM_DetailMode)
            _studyTracker.TimeInDetailMode += Time.time - _enterTime;
        
        _sortingMode = SortingMode.SM_BattleMode;
        
        detailSortingArray.SetActive(false);
        battleArrays.SetActive(true);

        detailOptionsUi.SetActive(false);
        battleOptionsUi.SetActive(true);
        
        detailDescriptionUi.SetActive(false);
        if (_isOnline)
            battleQuizOnline.SetActive(true);
        else
            battleQuizOffline.SetActive(true);
        
        battlePanels.SetActive(true);
        
        forwardButton.SetActive(false);
        backwardButton.SetActive(false);
        
        SetLeftBattleAlgorithm(battleAlgorithmDropdownLeft.value);
        SetRightBattleAlgorithm(battleAlgorithmDropdownRight.value);
        leftBattleSorting.RestoreOrder();
        rightBattleSorting.RestoreOrder();

        if (!_enteredOnce)
        {
            DisplayMessageByKey("EnterBattleModeMessage");
            _enteredOnce = true;
        }
    }

    public string GetLeftAlgorithm()
    {
        return GetAlgorithmName(leftBattleSorting.SelectedAlgorithm);
    }
    
    public string GetRightAlgorithm()
    {
        return GetAlgorithmName(rightBattleSorting.SelectedAlgorithm);
    }
    
    public void SetBattleOperationsPerSeconds(float value)
    {
        leftBattleSorting.OperationsPerSecond = value;
        rightBattleSorting.OperationsPerSecond = value;
    }
    
    public void SetLeftBattleAlgorithm(int value)
    {
        leftBattleSorting.SetAlgorithm((SortingAlgorithm.SortingAlgorithmType)value);
        NewAlgorithmSelected();
        quizManager.SetLeftButtonText(GetAlgorithmName((SortingAlgorithm.SortingAlgorithmType)value));
    }
    
    public void SetRightBattleAlgorithm(int value)
    {
        rightBattleSorting.SetAlgorithm((SortingAlgorithm.SortingAlgorithmType)value);
        NewAlgorithmSelected();
        quizManager.SetRightButtonText(GetAlgorithmName((SortingAlgorithm.SortingAlgorithmType)value));
    }
    
    public void SetArrangement(int arr)
    {
        SimulationController.Instance.StopSimulation();
        _arrangementMode = (ArrangementMode)arr;
        SetBattleOrder();
        
        ResetQuiz();
    }
    
    private void NewAlgorithmSelected()
    {
        SimulationController.Instance.StopSimulation();
        leftBattleSorting.RestoreOrder();
        rightBattleSorting.RestoreOrder();
        
        ResetQuiz();
    }
    
    private void SetBattleOrder()
    {
        if (_initialized && _isOnline && !Maroon.NetworkManager.Instance.IsInControl)
            return;
        
        var order = Enumerable.Range(0, battleArraySize).ToList();
        switch (_arrangementMode)
        {
            case ArrangementMode.AM_Random:
                ShuffleList(order);
                break;
            case ArrangementMode.AM_Sorted:
                //order stays
                break;
            case ArrangementMode.AM_Reversed:
                order.Reverse();
                break;
        }

        SetBattleOrder(order);
        
        if(_initialized && _isOnline && Maroon.NetworkManager.Instance.IsInControl)
            networkSync.SynchronizeBattleOrder(order);
    }

    public void SetBattleOrder(List<int> order)
    {
        leftBattleSorting.SetOrder(order);
        rightBattleSorting.SetOrder(order);
    }
    
    private static Random rng = new Random();
    private void ShuffleList(List<int> list)
    {
        var n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            int value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }
    }

    public string GetAlgorithmName(SortingAlgorithm.SortingAlgorithmType type)
    {
        switch (type)
        {
            case SortingAlgorithm.SortingAlgorithmType.SA_InsertionSort:
                return "Insertion Sort";
            case SortingAlgorithm.SortingAlgorithmType.SA_MergeSort:
                return "Merge Sort";
            case SortingAlgorithm.SortingAlgorithmType.SA_HeapSort:
                return "Heapsort";
            case SortingAlgorithm.SortingAlgorithmType.SA_QuickSort:
                return "Quicksort";
            case SortingAlgorithm.SortingAlgorithmType.SA_SelectionSort:
                return "Selection Sort";
            case SortingAlgorithm.SortingAlgorithmType.SA_BubbleSort:
                return "Bubble Sort";
            case SortingAlgorithm.SortingAlgorithmType.SA_GnomeSort:
                return "Gnome Sort";
            case SortingAlgorithm.SortingAlgorithmType.SA_RadixSort:
                return "Radix Sort";
            case SortingAlgorithm.SortingAlgorithmType.SA_ShellSort:
                return "Shellsort";
        }

        return "";
    }

    #endregion

    #region Quiz

    private bool _leftFinished;
    private bool _rightFinished;
    private bool _winnerEvaluated;

    [Header("Quiz")]
    [SerializeField] private GameObject leftBattlePanel;
    [SerializeField] private GameObject rightBattlePanel;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color winColor;

    private void SortingStarted()
    {
        if(_sortingMode != SortingMode.SM_BattleMode)
            return;
        
        quizManager.SortingStarted();
    }

    public void BattleAlgorithmFinished(BattleSorting algorithm)
    {
        if (algorithm == leftBattleSorting)
            _leftFinished = true;
        
        if (algorithm == rightBattleSorting)
            _rightFinished = true;

        if (_leftFinished && _rightFinished)
        {
            if(!_isOnline)
                StopSimulation();
            else
                Invoke(nameof(StopSimulation), 1.0f); //give other clients time to finish!
        }
    }

    private void StopSimulation()
    {
        SimulationController.Instance.StopSimulation();
    }

    private void Update()
    {
        if (_winnerEvaluated)
            return;
        if (_leftFinished)
        {
            if (leftBattleSorting.Operations < rightBattleSorting.Operations)
            {
                quizManager.CorrectChoice(QuizScore.QuizChoice.Left);
                AnimateWin(leftBattleSorting);
                leftBattlePanel.GetComponent<Image>().color = winColor;
                _winnerEvaluated = true;
            }
        }
        if(_rightFinished)
        {
            if (rightBattleSorting.Operations < leftBattleSorting.Operations)
            {
                quizManager.CorrectChoice(QuizScore.QuizChoice.Right);
                AnimateWin(rightBattleSorting);
                rightBattlePanel.GetComponent<Image>().color = winColor;
                _winnerEvaluated = true;
            }
        }
    }

    private void AnimateWin(BattleSorting winner)
    {
        StartCoroutine(WinAnimation(winner));
    }

    private IEnumerator WinAnimation(BattleSorting winner)
    {
        for (int i = 0; i < 5; ++i)
        {
            winner.HideAllElements();
            yield return new WaitForSeconds(0.1f);
            winner.ShowAllElements();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void ResetQuiz()
    {
        _winnerEvaluated = false;
        _leftFinished = false;
        _rightFinished = false;
        leftBattlePanel.GetComponent<Image>().color = defaultColor;
        rightBattlePanel.GetComponent<Image>().color = defaultColor;
        quizManager.ResetAllChoices();
    }

    #endregion
}
