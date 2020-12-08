using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    
    private List<int> _sortingValues = new List<int>();

    public List<int> SortingValues
    {
        get => _sortingValues;
        set
        {
            _sortingValues = new List<int>(value);
            _sortingVisualization.Size = value.Count;
            _sortingVisualization.ResetVisualization();
            ResetAlgorithm();
        }
    }
    
    private List<LinkedList<int>> _buckets = new List<LinkedList<int>>();

    [SerializeField] private PC_LocalizedDropDown algorithmDropDown;

    private SortingAlgorithmType _sortingAlgorithm;
    private SortingAlgorithm _algorithm;

    private SortingVisualization _sortingVisualization;
    
    private bool _operationPerformed;
    private bool _sortingFinished;

    private float _operationsPerSecond;
    private float _secondsCarry;

    public SortingAlgorithmType SelectedSortingAlgorithm => _sortingAlgorithm;

    public void Init(int size, float operationsPerSecond)
    {
        _sortingVisualization = GetComponent<SortingVisualization>();
        _sortingVisualization.Init(size);
        algorithmDropDown.onValueChanged.AddListener(SetAlgorithmDropdown);
        algorithmDropDown.allowReset = false;
        SetAlgorithmDropdown(algorithmDropDown.value);
        SetOperationsPerSecond(operationsPerSecond);
        for (int i = 0; i < 10; ++i)
        {
            _buckets.Add(new LinkedList<int>());
        }
    }

    public void SetOperationsPerSecond(float value)
    {
        _operationsPerSecond = value;
        //Make time per move shorter, so visualization finishes in time
        _sortingVisualization.TimePerMove = 0.95f / value;
    }

    private void Update()
    {
        if (SimulationController.Instance.SimulationRunning && !_sortingFinished)
        {
            _secondsCarry += Time.deltaTime;
            int operationsToExecute = (int) (_secondsCarry * _operationsPerSecond);
            _secondsCarry -= operationsToExecute / _operationsPerSecond;
            ExecuteOperations(operationsToExecute);
        }
    }

    private void ExecuteOperations(int numberOfOperations)
    {
        for (int i = 0; i < numberOfOperations; ++i)
        {
            ExecuteOneOperation();
        }
    }

    private void ExecuteOneOperation()
    {
        int safety = 0;
        while (!_operationPerformed && !_sortingFinished && safety < 100)
        {
            _algorithm.ExecuteNextState();
            safety++;
        }
        _operationPerformed = false;
    }

    #region GUI_Functions
    
    public void StepForward()
    {
        _algorithm.ExecuteNextState();
        _operationPerformed = false;
    }
    
    public void StepBackward()
    {
        _algorithm.ExecutePreviousState();
        _operationPerformed = false;
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
                _algorithm = new InsertionSort(this, _sortingValues.Count);
                break;
            case SortingAlgorithmType.SA_MergeSort:
                _algorithm = new MergeSort(this, _sortingValues.Count);
                break;
            case SortingAlgorithmType.SA_HeapSort:
                _algorithm = new HeapSort(this, _sortingValues.Count);
                break;
            case SortingAlgorithmType.SA_QuickSort:
                _algorithm = new QuickSort(this, _sortingValues.Count);
                break;
            case SortingAlgorithmType.SA_SelectionSort:
                _algorithm = new SelectionSort(this, _sortingValues.Count);
                break;
            case SortingAlgorithmType.SA_BubbleSort:
                _algorithm = new BubbleSort(this, _sortingValues.Count);
                break;
            case SortingAlgorithmType.SA_GnomeSort:
                _algorithm = new GnomeSort(this, _sortingValues.Count);
                break;
            case SortingAlgorithmType.SA_RadixSort:
                _sortingVisualization.ShowBuckets();
                _algorithm = new RadixSort(this, _sortingValues.Count);
                break;
            case SortingAlgorithmType.SA_ShellSort:
                _algorithm = new ShellSort(this, _sortingValues.Count);
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

    public void SortingFinished()
    {
        _sortingFinished = true;
        _sortingVisualization.SortingFinished();
        MarkCurrentSubset(0 , _sortingVisualization.Size - 1);
    }

    public void Insert(int fromIdx, int toIdx)
    {
        _operationPerformed = true;
        if (fromIdx < 0 || fromIdx >= _sortingVisualization.Size ||
            toIdx < 0 || toIdx >= _sortingVisualization.Size ||
            fromIdx == toIdx)
        {
            return;
        }
        
        _sortingVisualization.Insert(fromIdx, toIdx);
        
        int fromValue = _sortingValues[fromIdx];
        int i = fromIdx;
        if (fromIdx > toIdx)
        {
            while (i > toIdx)
            {
                _sortingValues[i] = _sortingValues[i-1];
                i--;
            }

            _sortingValues[toIdx] = fromValue;
        }
        else
        {
            while (i < toIdx)
            {
                _sortingValues[i] = _sortingValues[i+1];
                i++;
            }

            _sortingValues[toIdx] = fromValue;
        }
    }
    
    public void Swap(int fromIdx, int toIdx)
    {
        _operationPerformed = true;
        if (fromIdx < 0 || fromIdx >= _sortingVisualization.Size ||
            toIdx < 0 || toIdx >= _sortingVisualization.Size ||
            fromIdx == toIdx)
        {
            return;
        }
        
        _sortingVisualization.Swap(fromIdx, toIdx);
        
        int fromValue = _sortingValues[fromIdx];
        _sortingValues[fromIdx] = _sortingValues[toIdx];
        _sortingValues[toIdx] = fromValue;
    }

    public bool CompareGreater(int idx1, int idx2)
    {
        _operationPerformed = true;
        if (idx1 < 0 || idx1 >= _sortingVisualization.Size ||
            idx2 < 0 || idx2 >= _sortingVisualization.Size ||
            idx1 == idx2)
        {
            return false;
        }

        _sortingVisualization.CompareGreater(idx1, idx2);
        
        return _sortingValues[idx1] > _sortingValues[idx2];
    }

    public int GetMaxValue()
    {
        _operationPerformed = true;
        int max = _sortingValues.Max();
        _sortingVisualization.VisualizeMaxValue(_sortingValues.IndexOf(max));
        return max;
    }
    
    public int GetBucketNumber(int ind, int exp)
    {
        _operationPerformed = true;
        if (ind < 0 || ind > _sortingVisualization.Size)
        {
            return -1;
        }

        int b = (_sortingValues[ind] / exp) % 10;
        _sortingVisualization.VisualizeBucketNumber(ind, b);
        return b;
    }
    
    public void MoveToBucket(int idx, int bucket)
    {
        _operationPerformed = true;
        if (idx < 0 || idx >= _sortingVisualization.Size ||
            bucket < 0 || bucket >= 10)
        {
            return;
        }
        _sortingVisualization.MoveToBucket(idx, bucket);
        _buckets[bucket].AddLast(_sortingValues[idx]);
    }
    
    public void UndoMoveToBucket(int idx, int bucket)
    {
        _operationPerformed = true;
        if (idx < 0 || idx >= _sortingVisualization.Size ||
            bucket < 0 || bucket >= 10 ||
            BucketEmpty(bucket))
        {
            return;
        }
        _sortingValues[idx] = _buckets[bucket].Last.Value;
        _sortingVisualization.UndoMoveToBucket(idx, bucket, _sortingValues[idx]);
        _buckets[bucket].RemoveLast();
    }
    
    public void MoveFromBucket(int idx, int bucket)
    {
        _operationPerformed = true;
        if (idx < 0 || idx >= _sortingVisualization.Size ||
            bucket < 0 || bucket >= 10 ||
            BucketEmpty(bucket))
        {
            return;
        }
        _sortingValues[idx] = _buckets[bucket].First.Value;
        _sortingVisualization.MoveFromBucket(idx, bucket, _sortingValues[idx]);
        _buckets[bucket].RemoveFirst();
    }
    
    public void UndoMoveFromBucket(int idx, int bucket)
    {
        _operationPerformed = true;
        if (idx < 0 || idx >= _sortingVisualization.Size ||
            bucket < 0 || bucket >= 10)
        {
            return;
        }
        _sortingVisualization.UndoMoveFromBucket(idx, bucket);
        _buckets[bucket].AddFirst(_sortingValues[idx]);
    }

    public bool BucketEmpty(int bucket)
    {
        return _buckets[bucket].Count == 0;
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
