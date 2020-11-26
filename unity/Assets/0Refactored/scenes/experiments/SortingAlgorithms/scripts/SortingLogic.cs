using System;
using System.Collections;
using System.Collections.Generic;
using GEAR.Localization;
using PlatformControls.PC;
using TMPro;
using UnityEngine;

public class SortingLogic : MonoBehaviour
{
    public enum SortingAlgorithmType
    {
        SA_InsertionSort,
        SA_MergeSort,
        SA_HeapSort,
        SA_QuickSort,
        SA_SelectionSort,
        SA_BubbleSort,
        SA_GnomeSort,
        SA_RadixSort,
        SA_ShellSort
    }

    [SerializeField] private PC_LocalizedDropDown algorithmDropDown;

    private SortingAlgorithmType _sortingAlgorithm;
    private SortingAlgorithm _algorithm;

    private SortingVisualization _sortingVisualization;
    
    private bool _waitForMove;
    private bool _sortingFinished;

    public SortingAlgorithmType SelectedSortingAlgorithm => _sortingAlgorithm;

    public void Init()
    {
        _sortingVisualization = GetComponent<SortingVisualization>();
        algorithmDropDown.onValueChanged.AddListener(SetAlgorithmDropdown);
        algorithmDropDown.allowReset = false;
    }

    private void Update()
    {
        while (SimulationController.Instance.SimulationRunning && !_waitForMove && !_sortingFinished)
        {
            _waitForMove = true;
            _algorithm.ExecuteNextState();
        }
    }

    #region GUI_Functions
    
    public void StepForward()
    {
        if (_waitForMove)
            return;
        _waitForMove = true;
        _algorithm.ExecuteNextState();
    }
    
    public void StepBackward()
    {
        if (_waitForMove)
            return;
        _waitForMove = true;
        _algorithm.ExecutePreviousState();
    }

    public void SetAlgorithmDropdown(int choice)
    {
        _sortingAlgorithm = (SortingAlgorithmType)choice;
        _sortingVisualization.NewAlgorithmSelected();
        ResetAlgorithm();
        algorithmDropDown.value = choice;
    }
    
    #endregion

    public void ResetAlgorithm()
    {
        SimulationController.Instance.StopSimulation();
        
        if (_sortingVisualization == null)
            return;
        
        switch (_sortingAlgorithm)
        {
            case SortingAlgorithmType.SA_InsertionSort:
                _algorithm = new InsertionSort(this, _sortingVisualization.Size);
                break;
            case SortingAlgorithmType.SA_MergeSort:
                _algorithm = new MergeSort(this, _sortingVisualization.Size);
                break;
            case SortingAlgorithmType.SA_HeapSort:
                _algorithm = new HeapSort(this, _sortingVisualization.Size);
                break;
            case SortingAlgorithmType.SA_QuickSort:
                _algorithm = new QuickSort(this, _sortingVisualization.Size);
                break;
            case SortingAlgorithmType.SA_SelectionSort:
                _algorithm = new SelectionSort(this, _sortingVisualization.Size);
                break;
            case SortingAlgorithmType.SA_BubbleSort:
                _algorithm = new BubbleSort(this, _sortingVisualization.Size);
                break;
            case SortingAlgorithmType.SA_GnomeSort:
                _algorithm = new GnomeSort(this, _sortingVisualization.Size);
                break;
            case SortingAlgorithmType.SA_RadixSort:
                _sortingVisualization.ShowBuckets();
                _algorithm = new RadixSort(this, _sortingVisualization.Size);
                break;
            case SortingAlgorithmType.SA_ShellSort:
                _algorithm = new ShellSort(this, _sortingVisualization.Size);
                break;
            default:
                //No algorithm selected
                break;
        }
        
        SetPseudocode(-1, null);
        SetSwapsComparisons(0,0);
        DisplayIndices(new Dictionary<string, int>());
        SetDescription();
        _sortingVisualization.SetTitle(_algorithm.Pseudocode[0]);
        _sortingVisualization.ResetVisualization();
        _sortingFinished = false;
    }

    public void MoveFinished()
    {
        _waitForMove = false;
    }

    public void SortingFinished()
    {
        _sortingFinished = true;
        _sortingVisualization.SortingFinished();
        MarkCurrentSubset(0 , _sortingVisualization.Size - 1);
    }

    public void Insert(int fromIdx, int toIdx)
    {
        if (fromIdx < 0 || fromIdx >= _sortingVisualization.Size ||
            toIdx < 0 || toIdx >= _sortingVisualization.Size ||
            fromIdx == toIdx)
        {
            MoveFinished();
            return;
        }
        _sortingVisualization.Insert(fromIdx, toIdx);
    }
    
    public void Swap(int fromIdx, int toIdx)
    {
        if (fromIdx < 0 || fromIdx >= _sortingVisualization.Size ||
            toIdx < 0 || toIdx >= _sortingVisualization.Size ||
            fromIdx == toIdx)
        {
            MoveFinished();
            return;
        }
        _sortingVisualization.Swap(fromIdx, toIdx);
    }

    public bool CompareGreater(int idx1, int idx2)
    {
        if (idx1 < 0 || idx1 >= _sortingVisualization.Size ||
            idx2 < 0 || idx2 >= _sortingVisualization.Size ||
            idx1 == idx2)
        {
            MoveFinished();
            return false;
        }
        return _sortingVisualization.CompareGreater(idx1, idx2);
    }

    public int GetMaxValue()
    {
        return _sortingVisualization.GetMaxValue();
    }
    
    public int GetBucketNumber(int ind, int exp)
    {
        if (ind < 0 || ind > _sortingVisualization.Size)
        {
            MoveFinished();
            return -1;
        }
        return _sortingVisualization.GetBucketNumber(ind, exp);
    }
    
    public void MoveToBucket(int idx, int bucket)
    {
        if (idx < 0 || idx >= _sortingVisualization.Size ||
            bucket < 0 || bucket >= 10)
        {
            MoveFinished();
            return;
        }
        _sortingVisualization.MoveToBucket(idx, bucket);
    }
    
    public void UndoMoveToBucket(int idx, int bucket)
    {
        if (idx < 0 || idx >= _sortingVisualization.Size ||
            bucket < 0 || bucket >= 10 ||
            _sortingVisualization.BucketEmpty(bucket))
        {
            MoveFinished();
            return;
        }
        _sortingVisualization.UndoMoveToBucket(idx, bucket);
    }
    
    public void MoveFromBucket(int idx, int bucket)
    {
        if (idx < 0 || idx >= _sortingVisualization.Size ||
            bucket < 0 || bucket >= 10 ||
            _sortingVisualization.BucketEmpty(bucket))
        {
            MoveFinished();
            return;
        }
        _sortingVisualization.MoveFromBucket(idx, bucket);
    }
    
    public void UndoMoveFromBucket(int idx, int bucket)
    {
        if (idx < 0 || idx >= _sortingVisualization.Size ||
            bucket < 0 || bucket >= 10)
        {
            MoveFinished();
            return;
        }
        _sortingVisualization.UndoMoveFromBucket(idx, bucket);
    }

    public bool BucketEmpty(int bucket)
    {
        return _sortingVisualization.BucketEmpty(bucket);
    }

    public void SetPseudocode(int highlightLine, Dictionary<string, int> extraVars)
    {
        _sortingVisualization.DisplayPseudocode(_algorithm.Pseudocode, highlightLine, extraVars);
    }

    private void SetDescription()
    {
        string descriptionKey = "";
        switch (_sortingAlgorithm)
        {
            case SortingAlgorithmType.SA_InsertionSort:
                descriptionKey = "Insertion Sort Description";
                break;
            case SortingAlgorithmType.SA_MergeSort:
                descriptionKey = "Merge Sort Description";
                break;
            case SortingAlgorithmType.SA_HeapSort:
                descriptionKey = "Heap Sort Description";
                break;
            case SortingAlgorithmType.SA_QuickSort:
                descriptionKey = "Quick Sort Description";
                break;
            case SortingAlgorithmType.SA_SelectionSort:
                descriptionKey = "Selection Sort Description";
                break;
            case SortingAlgorithmType.SA_BubbleSort:
                descriptionKey = "Bubble Sort Description";
                break;
            case SortingAlgorithmType.SA_GnomeSort:
                descriptionKey = "Gnome Sort Description";
                break;
            case SortingAlgorithmType.SA_RadixSort:
                descriptionKey = "Radix Sort Description";
                break;
            case SortingAlgorithmType.SA_ShellSort:
                descriptionKey = "Shell Sort Description";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(_sortingAlgorithm), _sortingAlgorithm, null);
        }
        
        _sortingVisualization.SetDescription(descriptionKey);
    }

    public void MarkCurrentSubset(int from, int to)
    {
        if (from < 0 || from >= _sortingVisualization.Size ||
            to < 0 || to >= _sortingVisualization.Size)
        {
            return;
        }
        _sortingVisualization.MarkCurrentSubset(from, to);
    }

    public void SetSwapsComparisons(int swaps, int comparisons)
    {
        _sortingVisualization.SetSwapsComparisons(swaps, comparisons);
    }
    
    public void DisplayIndices(Dictionary<string, int> indices)
    {
        _sortingVisualization.DisplayIndices(indices);
    }
}
