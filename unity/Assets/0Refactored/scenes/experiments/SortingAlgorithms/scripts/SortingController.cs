using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlatformControls.PC;
using UnityEngine;
using Random = System.Random;

public class SortingController : MonoBehaviour, IResetObject
{
    public enum ArrangementMode
    {
        AM_Random,
        AM_Sorted,
        AM_Reversed
    }

    private ArrangementMode _arrangementMode;
    [SerializeField] private PC_LocalizedDropDown arrangementDropdown;
    
    [SerializeField] private int battleArraySize;
    
    [SerializeField] private GameObject detailSortingArray;
    [SerializeField] private GameObject battleArrays;
    
    [SerializeField] private GameObject detailOptionsUi;
    [SerializeField] private GameObject battleOptionsUi;
    
    [SerializeField] private GameObject detailDescriptionUi;
    //[SerializeField] private GameObject battleBettingUi;

    [SerializeField] private GameObject battlePanels;

    [SerializeField] private GameObject forwardButton;
    [SerializeField] private GameObject backwardButton;
    
    private void Start()
    {
        foreach (var vis in GetComponentsInChildren<SortingVisualization>())
        {
            vis.Init(battleArraySize);
        }
        EnterDetailMode();
        arrangementDropdown.onValueChanged.AddListener(NewArrangementSelected);
        arrangementDropdown.allowReset = false;
        _arrangementMode = (ArrangementMode)arrangementDropdown.value;
    }

    public void EnterDetailMode(int chosenAlgorithm = -1)
    {
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
        
        foreach (var array in battleArrays.GetComponentsInChildren<SortingSpriteArray>())
        {
            array.Size = battleArraySize;
            array.ReorderElements(order);
        }
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
            array.RestoreOrder();
        }
    }

    public void NewArrangementSelected(int arr)
    {
        _arrangementMode = (ArrangementMode)arr;
        SetBattleOrder();
    }

    public void SetExecutionSpeed(float speed)
    {
        Time.timeScale = speed;
    }

    public void ResetObject()
    {
        SetBattleOrder();
    }
}
