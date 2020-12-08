using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlatformControls.PC;
using UnityEngine;
using Random = System.Random;

public class SortingController : MonoBehaviour, IResetObject
{
    public enum SortingMode
    {
        SM_DetailMode,
        SM_BattleMode
    }
    
    public enum ArrangementMode
    {
        AM_Random,
        AM_Sorted,
        AM_Reversed
    }

    private SortingMode _sortingMode;
    private ArrangementMode _arrangementMode;
    
    [SerializeField] private PC_LocalizedDropDown arrangementDropdown;
    
    [SerializeField] private int battleArraySize;
    private int _detailArraySize;
    
    [SerializeField] private GameObject detailSortingArray;
    [SerializeField] private GameObject battleArrays;
    
    [SerializeField] private GameObject detailOptionsUi;
    [SerializeField] private GameObject battleOptionsUi;
    
    [SerializeField] private GameObject detailDescriptionUi;
    //[SerializeField] private GameObject battleBettingUi;

    [SerializeField] private GameObject battlePanels;

    //TODO: Buttons also triggered here!
    [SerializeField] private GameObject forwardButton;
    [SerializeField] private GameObject backwardButton;
    
    [SerializeField] private PC_Slider sizeSlider;

    [SerializeField] private SortingLogic detailSortingLogic;
    [SerializeField] private SortingLogic leftBattleSortingLogic;
    [SerializeField] private SortingLogic rightBattleSortingLogic;
    
    private void Start()
    {
        detailSortingLogic.Init(_detailArraySize, 1);
        leftBattleSortingLogic.Init(battleArraySize, 20);//TODO
        rightBattleSortingLogic.Init(battleArraySize, 20);
        SetDetailArraySize(sizeSlider.value);
        EnterDetailMode();
        arrangementDropdown.onValueChanged.AddListener(NewArrangementSelected);
        arrangementDropdown.allowReset = false;
        _arrangementMode = (ArrangementMode)arrangementDropdown.value;
    }
    
    public void SetDetailArraySize(float size)
    {
        _detailArraySize = (int)size;
        RandomizeDetailArray();
    }

    private void RandomizeDetailArray()
    {
        List<int> newValues = new List<int>();
        for (int i = 0; i < _detailArraySize; ++i)
        {
            newValues.Add(rng.Next(100));
        }
        detailSortingLogic.SortingValues = newValues;
    }

    public void EnterDetailMode(int chosenAlgorithm = -1)
    {
        _sortingMode = SortingMode.SM_DetailMode;
        
        detailSortingArray.SetActive(true);
        battleArrays.SetActive(false);

        detailOptionsUi.SetActive(true);
        battleOptionsUi.SetActive(false);
        
        detailDescriptionUi.SetActive(true);
        //battleBettingUI.SetActive(false);
        
        battlePanels.SetActive(false);
        
        forwardButton.SetActive(true);
        backwardButton.SetActive(true);

        if (chosenAlgorithm != -1)
        {
            detailSortingArray.GetComponent<SortingLogic>().SetAlgorithmDropdown(chosenAlgorithm);
        }
        
        detailSortingArray.GetComponent<SortingArray>().ResetObject();
    }
    
    public void EnterBattleMode()
    {
        _sortingMode = SortingMode.SM_BattleMode;
        
        detailSortingArray.SetActive(false);
        battleArrays.SetActive(true);

        detailOptionsUi.SetActive(false);
        battleOptionsUi.SetActive(true);
        
        detailDescriptionUi.SetActive(false);
        //battleBettingUI.SetActive(true);
        
        battlePanels.SetActive(true);
        
        forwardButton.SetActive(false);
        backwardButton.SetActive(false);
        
        SetBattleOrder();
    }

    private void SetBattleOrder()
    {
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

        leftBattleSortingLogic.SortingValues = order;
        rightBattleSortingLogic.SortingValues = order;
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

    public void NewAlgorithmSelected()
    {
        foreach (var array in battleArrays.GetComponentsInChildren<SortingSpriteArray>())
        {
            //TODO: array.RestoreOrder();
        }
    }

    public void NewArrangementSelected(int arr)
    {
        _arrangementMode = (ArrangementMode)arr;
        SetBattleOrder();
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
    }
}
