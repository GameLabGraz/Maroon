using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GEAR.Localization;
using Maroon.UI;
using Mirror;
using PlatformControls.PC;
using UnityEngine;
using Random = System.Random;

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
    private bool _initialized = false;

    [SerializeField] private QuizManager quizManager;
    public QuizManager TheQuizManager => quizManager;

    private DialogueManager _dialogueManager;

    private void Start()
    {
        _isOnline = NetworkClient.active;
        detailSortingLogic.Init(_detailArraySize);
        leftBattleSorting.Init(battleArraySize, this);
        rightBattleSorting.Init(battleArraySize, this);
        RandomizeDetailArray();
        SetDetailArraySize(sizeSlider.value);
        EnterDetailMode();
        arrangementDropdown.allowReset = false;
        _arrangementMode = (ArrangementMode)arrangementDropdown.value;
        SetBattleOperationsPerSeconds(speedSlider.value);
        
        SimulationController.Instance.onStartRunning.AddListener(SortingStarted);
        
        DisplayMessageByKey("EnterSortingExperiment");
        
        //Only after delay, so all clients are ready
        Invoke(nameof(DistributeDetailArray), 1.0f);
        Invoke(nameof(SetBattleOrder), 1.0f);
        _initialized = true;
    }

    public void GoOffline()
    {
        _isOnline = false;
        quizManager.gameObject.SetActive(false);
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
        
        if(_isOnline)
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
    
    private int _detailArraySize;
    [SerializeField] private PC_Slider sizeSlider;
    
    [SerializeField] private GameObject detailSortingArray;
    [SerializeField] private GameObject detailOptionsUi;
    [SerializeField] private GameObject detailDescriptionUi;
    [SerializeField] private GameObject forwardButton;
    [SerializeField] private GameObject backwardButton;
    
    [SerializeField] private SortingLogic detailSortingLogic;
    
    public void EnterDetailMode(int chosenAlgorithm = -1)
    {
        _sortingMode = SortingMode.SM_DetailMode;
        
        detailSortingArray.SetActive(true);
        battleArrays.SetActive(false);

        detailOptionsUi.SetActive(true);
        battleOptionsUi.SetActive(false);
        
        detailDescriptionUi.SetActive(true);
        quizManager.gameObject.SetActive(false);
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
        _detailArraySize = (int)size;
        detailSortingLogic.SortingValues = _detailOrder.GetRange(0, _detailArraySize);
    }
    
    private void RandomizeDetailArray()
    {
        if (_initialized && _isOnline && !MaroonNetworkManager.Instance.IsInControl)
            return;
        _detailOrder = new List<int>();
        for (int i = 0; i < 10; ++i)
        {
            _detailOrder.Add(rng.Next(100));
        }
        detailSortingLogic.SortingValues = _detailOrder.GetRange(0, _detailArraySize);
        
        DistributeDetailArray();
    }

    private void DistributeDetailArray()
    {
        if(_initialized && _isOnline && MaroonNetworkManager.Instance.IsInControl)
            networkSync.SynchronizeArray(_detailOrder);
    }

    public void SetDetailArray(List<int> array)
    {
        _detailOrder = array;
        detailSortingLogic.SortingValues = _detailOrder.GetRange(0, _detailArraySize);
    }

    #endregion

    #region BattleMode
    
    [SerializeField] private int battleArraySize;

    private enum ArrangementMode
    {
        AM_Random,
        AM_Sorted,
        AM_Reversed
    }
    private ArrangementMode _arrangementMode;
    [SerializeField] private PC_LocalizedDropDown arrangementDropdown;
    
    [SerializeField] private PC_LocalizedDropDown battleAlgorithmDropdownLeft;
    [SerializeField] private PC_LocalizedDropDown battleAlgorithmDropdownRight;
    
    [SerializeField] private PC_Slider speedSlider;
    
    [SerializeField] private GameObject battleArrays;
    [SerializeField] private GameObject battleOptionsUi;
    //[SerializeField] private GameObject battleBettingUi;
    [SerializeField] private GameObject battlePanels;
    
    [SerializeField] private BattleSorting leftBattleSorting;
    [SerializeField] private BattleSorting rightBattleSorting;

    private bool _enteredOnce;
    public void EnterBattleMode()
    {
        _sortingMode = SortingMode.SM_BattleMode;
        
        detailSortingArray.SetActive(false);
        battleArrays.SetActive(true);

        detailOptionsUi.SetActive(false);
        battleOptionsUi.SetActive(true);
        
        detailDescriptionUi.SetActive(false);
        if (_isOnline)
            quizManager.gameObject.SetActive(true);
        
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
    
    public void SetBattleOperationsPerSeconds(float value)
    {
        leftBattleSorting.OperationsPerSecond = value;
        rightBattleSorting.OperationsPerSecond = value;
    }
    
    public void SetLeftBattleAlgorithm(int value)
    {
        leftBattleSorting.SetAlgorithm((SortingAlgorithm.SortingAlgorithmType)value);
        NewAlgorithmSelected();
        if(_isOnline)
            quizManager.SetLeftButtonText(GetAlgorithmName((SortingAlgorithm.SortingAlgorithmType)value));
    }
    
    public void SetRightBattleAlgorithm(int value)
    {
        rightBattleSorting.SetAlgorithm((SortingAlgorithm.SortingAlgorithmType)value);
        NewAlgorithmSelected();
        if(_isOnline)
            quizManager.SetRightButtonText(GetAlgorithmName((SortingAlgorithm.SortingAlgorithmType)value));
    }
    
    public void SetArrangement(int arr)
    {
        SimulationController.Instance.StopSimulation();
        _arrangementMode = (ArrangementMode)arr;
        SetBattleOrder();
    }
    
    private void NewAlgorithmSelected()
    {
        SimulationController.Instance.StopSimulation();
        leftBattleSorting.RestoreOrder();
        rightBattleSorting.RestoreOrder();
        
        if(_isOnline)
            ResetQuiz();
    }
    
    private void SetBattleOrder()
    {
        if (_initialized && _isOnline && !MaroonNetworkManager.Instance.IsInControl)
            return;
        
        List<int> order = Enumerable.Range(0, battleArraySize).ToList();
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
        
        if(_initialized && _isOnline && MaroonNetworkManager.Instance.IsInControl)
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
        int n = list.Count;  
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
                return "Heap Sort";
            case SortingAlgorithm.SortingAlgorithmType.SA_QuickSort:
                return "Quick Sort";
            case SortingAlgorithm.SortingAlgorithmType.SA_SelectionSort:
                return "Selection Sort";
            case SortingAlgorithm.SortingAlgorithmType.SA_BubbleSort:
                return "Bubble Sort";
            case SortingAlgorithm.SortingAlgorithmType.SA_GnomeSort:
                return "Gnome Sort";
            case SortingAlgorithm.SortingAlgorithmType.SA_RadixSort:
                return "Radix Sort";
            case SortingAlgorithm.SortingAlgorithmType.SA_ShellSort:
                return "Shell Sort";
        }

        return "";
    }

    #endregion

    #region Quiz

    private bool _leftFinished;
    private bool _rightFinished;
    private bool _winnerEvaluated;

    private void SortingStarted()
    {
        if(!_isOnline || _sortingMode != SortingMode.SM_BattleMode)
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
        if (_winnerEvaluated || !_isOnline)
            return;
        if (_leftFinished)
        {
            if (leftBattleSorting.Operations < rightBattleSorting.Operations)
            {
                quizManager.CorrectChoice(QuizScore.QuizChoice.Left);
                _winnerEvaluated = true;
            }
        }
        if(_rightFinished)
        {
            if (rightBattleSorting.Operations < leftBattleSorting.Operations)
            {
                quizManager.CorrectChoice(QuizScore.QuizChoice.Right);
                _winnerEvaluated = true;
            }
        }
    }

    private void ResetQuiz()
    {
        _winnerEvaluated = false;
        _leftFinished = false;
        _rightFinished = false;
        if(_isOnline)
            quizManager.ResetAllChoices();
    }

    #endregion
}
