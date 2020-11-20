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

    private SortingArray _sortingArray;
    
    private bool _waitForMove;

    private void Awake()
    {
        _sortingArray = GetComponent<SortingArray>();
        SetAlgorithmDropdown(algorithmDropDown.value);
    }

    private void Update()
    {
        if (SimulationController.Instance.SimulationRunning && !_waitForMove)
        {
            while (_waitForMove == false && SimulationController.Instance.SimulationRunning)
            {
                _waitForMove = true;
                _algorithm.ExecuteNextState();
            }
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
        ResetAlgorithm();
    }
    
    #endregion

    public void ResetAlgorithm()
    {
        _sortingArray.HideBuckets();
        switch (_sortingAlgorithm)
        {
            case SortingAlgorithmType.SA_InsertionSort:
                _algorithm = new InsertionSort(this, _sortingArray.Size);
                break;
            case SortingAlgorithmType.SA_MergeSort:
                _algorithm = new MergeSort(this, _sortingArray.Size);
                break;
            case SortingAlgorithmType.SA_HeapSort:
                _algorithm = new HeapSort(this, _sortingArray.Size);
                break;
            case SortingAlgorithmType.SA_QuickSort:
                _algorithm = new QuickSort(this, _sortingArray.Size);
                break;
            case SortingAlgorithmType.SA_SelectionSort:
                _algorithm = new SelectionSort(this, _sortingArray.Size);
                break;
            case SortingAlgorithmType.SA_BubbleSort:
                _algorithm = new BubbleSort(this, _sortingArray.Size);
                break;
            case SortingAlgorithmType.SA_GnomeSort:
                _algorithm = new GnomeSort(this, _sortingArray.Size);
                break;
            case SortingAlgorithmType.SA_RadixSort:
                _sortingArray.ShowBuckets();
                _algorithm = new RadixSort(this, _sortingArray.Size);
                break;
            case SortingAlgorithmType.SA_ShellSort:
                _algorithm = new ShellSort(this, _sortingArray.Size);
                break;
            default:
                //No algorithm selected
                break;
        }
        
        SetPseudocode(-1, null);
        SetSwapsComparisons(0,0);
        DisplayIndices(new Dictionary<string, int>());
        SetDescription();
    }

    public void MoveFinished()
    {
        _waitForMove = false;
    }

    public void SortingFinished()
    {
        SimulationController.Instance.StopSimulation();
        MarkCurrentSubset(0 , _sortingArray.Size - 1);
    }

    public void Insert(int fromIdx, int toIdx)
    {
        if (fromIdx < 0 || fromIdx >= _sortingArray.Size ||
            toIdx < 0 || toIdx >= _sortingArray.Size ||
            fromIdx == toIdx)
        {
            MoveFinished();
            return;
        }
        _sortingArray.Insert(fromIdx, toIdx);
    }
    
    public void Swap(int fromIdx, int toIdx)
    {
        if (fromIdx < 0 || fromIdx >= _sortingArray.Size ||
            toIdx < 0 || toIdx >= _sortingArray.Size ||
            fromIdx == toIdx)
        {
            MoveFinished();
            return;
        }
        _sortingArray.Swap(fromIdx, toIdx);
    }

    public bool CompareGreater(int idx1, int idx2)
    {
        if (idx1 < 0 || idx1 >= _sortingArray.Size ||
            idx2 < 0 || idx2 >= _sortingArray.Size ||
            idx1 == idx2)
        {
            MoveFinished();
            return false;
        }
        return _sortingArray.CompareGreater(idx1, idx2);
    }

    public int GetMaxValue()
    {
        return _sortingArray.GetMaxValue();
    }
    
    public int GetBucketNumber(int ind, int exp)
    {
        if (ind < 0 || ind > _sortingArray.Size)
        {
            MoveFinished();
            return -1;
        }
        return _sortingArray.GetBucketNumber(ind, exp);
    }
    
    public void MoveToBucket(int idx, int bucket)
    {
        if (idx < 0 || idx >= _sortingArray.Size ||
            bucket < 0 || bucket >= 10)
        {
            MoveFinished();
            return;
        }
        _sortingArray.MoveToBucket(idx, bucket);
    }
    
    public void UndoMoveToBucket(int idx, int bucket)
    {
        if (idx < 0 || idx >= _sortingArray.Size ||
            bucket < 0 || bucket >= 10 ||
            _sortingArray.BucketEmpty(bucket))
        {
            MoveFinished();
            return;
        }
        _sortingArray.UndoMoveToBucket(idx, bucket);
    }
    
    public void MoveFromBucket(int idx, int bucket)
    {
        if (idx < 0 || idx >= _sortingArray.Size ||
            bucket < 0 || bucket >= 10 ||
            _sortingArray.BucketEmpty(bucket))
        {
            MoveFinished();
            return;
        }
        _sortingArray.MoveFromBucket(idx, bucket);
    }
    
    public void UndoMoveFromBucket(int idx, int bucket)
    {
        if (idx < 0 || idx >= _sortingArray.Size ||
            bucket < 0 || bucket >= 10)
        {
            MoveFinished();
            return;
        }
        _sortingArray.UndoMoveFromBucket(idx, bucket);
    }

    public bool BucketEmpty(int bucket)
    {
        return _sortingArray.BucketEmpty(bucket);
    }

    public void SetPseudocode(int highlightLine, Dictionary<string, int> extraVars)
    {
        _sortingArray.DisplayPseudocode(_algorithm.Pseudocode, highlightLine, extraVars);
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
        
        _sortingArray.SetDescription(descriptionKey);
    }

    public void MarkCurrentSubset(int from, int to)
    {
        if (from < 0 || from >= _sortingArray.Size ||
            to < 0 || to >= _sortingArray.Size)
        {
            return;
        }
        _sortingArray.MarkCurrentSubset(from, to);
    }

    public void SetSwapsComparisons(int swaps, int comparisons)
    {
        _sortingArray.SetSwapsComparisons(swaps, comparisons);
    }
    
    public void DisplayIndices(Dictionary<string, int> indices)
    {
        _sortingArray.DisplayIndices(indices);
    }
}
