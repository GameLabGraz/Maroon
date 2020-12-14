using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GEAR.Localization;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class BattleSorting : MonoBehaviour
{
    [SerializeField] private GameObject columnPrefab;
    
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI operationsText;
    
    
    private int _size;
    private List<SortingColumn> _elements = new List<SortingColumn>();
    private List<Queue<int>> _buckets = new List<Queue<int>>();
    private Stack<SortingColumn> _highlightedColumns = new Stack<SortingColumn>();

    private float _operationsPerSecond;
    public float OperationsPerSecond
    {
        get => _operationsPerSecond;
        set => _operationsPerSecond = value;
    }
    private float _secondsCarry;
    private int _operationsToExecute;
    private int _executedOperations;

    private List<int> _currentOrder;
    private SortingAlgorithm.SortingAlgorithmType _selectedAlgorithm;
    
    public void Init(int size)
    {
        _size = size;
        
        for (int i = 0; i < 10; ++i)
        {
            _buckets.Add(new Queue<int>());
        }
        
        CreateColumnArray();
        
        LanguageManager.Instance.OnLanguageChanged.AddListener(OnLanguageChanged);
    }
    
    private void Update()
    {
        if (SimulationController.Instance.SimulationRunning)
        {
            _secondsCarry += Time.deltaTime;
            int newOperationsToExecute = (int) (_secondsCarry * _operationsPerSecond);
            if(_operationsPerSecond > 0)
                _secondsCarry -= newOperationsToExecute / _operationsPerSecond;
            _operationsToExecute += newOperationsToExecute;
        }
    }
    
    private void CreateColumnArray()
    {
        for (int i = 0; i < _size; ++i)
        {
            var newColumn = Instantiate(columnPrefab, transform, false).GetComponent<SortingColumn>();
            newColumn.Init(_size, i, i);
            _elements.Add(newColumn);
        }
    }

    public void SetOrder(List<int> order)
    {
        _currentOrder = order;
        for(int i = 0; i < _size; ++i)
        {
            _elements[i].Value = order[i];
        }
        ResetVisualization();
        ResetAlgorithm();
    }

    public void RestoreOrder()
    {
        SetOrder(_currentOrder);
    }

    public void SetAlgorithm(SortingAlgorithm.SortingAlgorithmType type)
    {
        _selectedAlgorithm = type;
        ResetAlgorithm();
    }

    private void ResetAlgorithm()
    {
        StopAllCoroutines();
        _executedOperations = 0;
        _operationsToExecute = 0;
        _secondsCarry = 0;
        DisplayOperations();
        if(isActiveAndEnabled)
            StartCoroutine(SortingStarter());
    }

    private IEnumerator SortingStarter()
    {
        switch (_selectedAlgorithm)
        {
            case SortingAlgorithm.SortingAlgorithmType.SA_InsertionSort:
                titleText.text = "<style=\"sortingTitle\">Insertion Sort</style>";
                yield return InsertionSort();
                break;
            case SortingAlgorithm.SortingAlgorithmType.SA_MergeSort:
                titleText.text = "<style=\"sortingTitle\">Merge Sort</style>";
                yield return MergeSort(0, _size-1);
                break;
            case SortingAlgorithm.SortingAlgorithmType.SA_HeapSort:
                titleText.text = "<style=\"sortingTitle\">Heap Sort</style>";
                yield return HeapSort();
                break;
            case SortingAlgorithm.SortingAlgorithmType.SA_QuickSort:
                titleText.text = "<style=\"sortingTitle\">Quick Sort</style>";
                yield return QuickSort(0, _size-1);
                break;
            case SortingAlgorithm.SortingAlgorithmType.SA_SelectionSort:
                titleText.text = "<style=\"sortingTitle\">Selection Sort</style>";
                yield return SelectionSort();
                break;
            case SortingAlgorithm.SortingAlgorithmType.SA_BubbleSort:
                titleText.text = "<style=\"sortingTitle\">Bubble Sort</style>";
                yield return BubbleSort();
                break;
            case SortingAlgorithm.SortingAlgorithmType.SA_GnomeSort:
                titleText.text = "<style=\"sortingTitle\">Gnome Sort</style>";
                yield return GnomeSort();
                break;
            case SortingAlgorithm.SortingAlgorithmType.SA_RadixSort:
                titleText.text = "<style=\"sortingTitle\">Radix Sort</style>";
                yield return RadixSort();
                break;
            case SortingAlgorithm.SortingAlgorithmType.SA_ShellSort:
                titleText.text = "<style=\"sortingTitle\">Shell Sort</style>";
                yield return ShellSort();
                break;
        }
        
        FinishedSorting();
    }

    private void FinishedSorting()
    {
        ResetVisualization();
        //TODO: Finished!
    }
    
    public void EnterDetailMode()
    {
        int selectedAlgorithm = (int)_selectedAlgorithm;
        GetComponentInParent<SortingController>().EnterDetailMode(selectedAlgorithm);
    }

    private void OnLanguageChanged(SystemLanguage language)
    {
        DisplayOperations();
    }

    #region Operations
    
    private void DisplayOperations()
    {
        operationsText.text = LanguageManager.Instance.GetString("OperationsPrefix") +  _executedOperations;
    }
    
    private bool OperationsAvailable()
    {
        return _operationsToExecute > 0;
    }

    private bool WaitForNextOperation()
    {
        _executedOperations++;
        DisplayOperations();
        _operationsToExecute--;
        if (_operationsToExecute <= 0)
            return true;
        return false;
    }

    private void Insert(int fromIdx, int toIdx)
    {
        ClearHighlights();
        int fromValue = _elements[fromIdx].Value;
        int i = fromIdx;
        if (fromIdx > toIdx)
        {
            while (i > toIdx)
            {
                _elements[i].Value = _elements[i-1].Value;
                i--;
            }

            _elements[toIdx].Value = fromValue;
        }
        else
        {
            while (i < toIdx)
            {
                _elements[i].Value = _elements[i+1].Value;
                i++;
            }

            _elements[toIdx].Value = fromValue;
        }
    }

    private void Swap(int idx1, int idx2)
    {
        ClearHighlights();
        int fromValue = _elements[idx1].Value;
        _elements[idx1].Value = _elements[idx2].Value;
        _elements[idx2].Value = fromValue;
    }
    
    private void MoveToBucket(int idx, int bucket)
    {
        _buckets[bucket].Enqueue(_elements[idx].Value);
        _elements[idx].Hidden = true;
    }
    
    private void MoveFromBucket(int idx, int bucket)
    {
        _elements[idx].Value = _buckets[bucket].Dequeue();
        _elements[idx].Hidden = false;
    }
    
    private bool CompareGreater(int idx1, int idx2)
    {
        ClearHighlights();
        _elements[idx1].Marked = true;
        _elements[idx2].Marked = true;
        _highlightedColumns.Push(_elements[idx1]);
        _highlightedColumns.Push(_elements[idx2]);
        return _elements[idx1].Value > _elements[idx2].Value;
    }
    
    #endregion

    #region Visualisation
    
    private void ClearHighlights()
    {
        while (_highlightedColumns.Count > 0)
        {
            _highlightedColumns.Pop().Marked = false;
        }
    }
    
    private void MarkSubset(int from, int to)
    {
        for (int i = 0; i < _size; ++i)
        {
            if (i >= from && i <= to)
            {
                _elements[i].Hidden = false;
            }
            else
            {
                _elements[i].Hidden = true;
            }
        }
    }

    private void ResetVisualization()
    {
        ClearHighlights();
        MarkSubset(0,_size-1);
    }
    
    #endregion

    #region SortingCoroutines

    #region Insertion Sort
    private IEnumerator InsertionSort()
    {
        yield return new WaitUntil(OperationsAvailable);
        for (int i = 0; i < _size; ++i)
        {
            int j = i - 1;
            while (j >= 0)
            {
                bool result = CompareGreater(j, i);
                if(WaitForNextOperation())
                    yield return new WaitUntil(OperationsAvailable);
                if (!result)
                    break;
                j = j - 1;
            }

            if (i != j + 1)
            {
                Insert(i, j + 1);
                if (WaitForNextOperation())
                    yield return new WaitUntil(OperationsAvailable);
            }
        }
    }
    #endregion
    
    #region Merge Sort
    private IEnumerator MergeSort(int i, int j)
    {
        MarkSubset(i,j);
        if(_operationsToExecute <= 0)
            yield return new WaitUntil(OperationsAvailable);
        if (i < j)
        {
            int k = (i + j) / 2;
            yield return MergeSort(i, k);
            MarkSubset(i,j);
            yield return MergeSort(k+1, j);
            MarkSubset(i,j);
            yield return Merge(i, k, j);
            MarkSubset(i,j);
        }
    }

    private IEnumerator Merge(int i, int k, int j)
    {
        int r = k + 1;
        int l = i;
        while (l < r && r <= j)
        {
            bool result = CompareGreater(l, r);
            if(WaitForNextOperation())
                yield return new WaitUntil(OperationsAvailable);
            if (result)
            {
                Insert(r,l);
                if(WaitForNextOperation())
                    yield return new WaitUntil(OperationsAvailable);
                r++;
            }
            l++;
        }
    }
    #endregion

    #region Heap Sort
    private IEnumerator HeapSort()
    {
        yield return new WaitUntil(OperationsAvailable);
        int n = _size;
        for (int i = n / 2 + 1; i >= 0; --i)
        {
            yield return Heapify(i, n);
        }

        for (int i = _size-1; i > 0; --i)
        {
            Swap(0,i);
            if(WaitForNextOperation())
                yield return new WaitUntil(OperationsAvailable);
            n--;
            yield return Heapify(0, n);
        }
    }

    private IEnumerator Heapify(int j, int n)
    {
        int l = 2 * j + 1;
        int r = 2 * j + 2;
        int m = j;
        if (l < n)
        {
            bool result = CompareGreater(l, j);
            if(WaitForNextOperation())
                yield return new WaitUntil(OperationsAvailable);
            if (result)
            {
                m = l;
            }
        }
        if (r < n)
        {
            bool result = CompareGreater(r, m);
            if(WaitForNextOperation())
                yield return new WaitUntil(OperationsAvailable);
            if (result)
            {
                m = r;
            }
        }
        if (m != j)
        {
            Swap(j,m);
            if(WaitForNextOperation())
                yield return new WaitUntil(OperationsAvailable);
            yield return Heapify(m, n);
        }
    }
    #endregion
    
    #region Quick Sort

    private int _k;
    
    private IEnumerator QuickSort(int l, int r)
    {
        MarkSubset(l,r);
        if(_operationsToExecute <= 0)
            yield return new WaitUntil(OperationsAvailable);
        if (l < r)
        {
            yield return Partition(l, r);
            int k = _k; //Workaround for return value
            yield return QuickSort(l, k - 1);
            MarkSubset(l,r);
            yield return QuickSort(k + 1, r);
            MarkSubset(l,r);
        }
    }

    private IEnumerator Partition(int l, int r)
    {
        int pInd = r;
        int k = l;
        for(int j = l; j < r; ++j)
        {
            bool result = !CompareGreater(j, pInd);
            if (WaitForNextOperation())
                yield return new WaitUntil(OperationsAvailable);
            if (result)
            {
                Swap(j,k);
                if (WaitForNextOperation())
                    yield return new WaitUntil(OperationsAvailable);
                k++;
            }
        }
        Swap(k,r);
        if (WaitForNextOperation())
            yield return new WaitUntil(OperationsAvailable);
        _k = k;
    }
    #endregion

    #region Selection Sort

    private IEnumerator SelectionSort()
    {
        yield return new WaitUntil(OperationsAvailable);
        for (int i = 0; i < _size; ++i)
        {
            int m = i;
            for (int j = i + 1; j < _size; ++j)
            {
                bool result = CompareGreater(m, j);
                if(WaitForNextOperation())
                    yield return new WaitUntil(OperationsAvailable);
                if (result)
                {
                    m = j;
                }
            }
            if (i != m)
            {
                Swap(i,m);
                if(WaitForNextOperation())
                    yield return new WaitUntil(OperationsAvailable);
            }
        }
    }

    #endregion

    #region Bubble Sort

    private IEnumerator BubbleSort()
    {
        yield return new WaitUntil(OperationsAvailable);
        for (int i = 0; i < _size; ++i)
        {
            for (int j = 0; j < _size - i - 1; ++j)
            {
                bool result = CompareGreater(j, j + 1);
                if (WaitForNextOperation())
                    yield return new WaitUntil(OperationsAvailable);
                if (result)
                {
                    Swap(j, j+1);
                    if (WaitForNextOperation())
                        yield return new WaitUntil(OperationsAvailable);
                }
            }
        }
    }

    #endregion

    #region Gnome Sort

    private IEnumerator GnomeSort()
    {
        yield return new WaitUntil(OperationsAvailable);
        int i = 0;
        while (i < _size)
        {
            if (i == 0)
            {
                i++;
            }

            bool result = CompareGreater(i, i - 1);
            if (WaitForNextOperation())
                yield return new WaitUntil(OperationsAvailable);
            if (result)
            {
                i++;
            }
            else
            {
                Swap(i, i - 1);
                if (WaitForNextOperation())
                    yield return new WaitUntil(OperationsAvailable);
                i--;
            }
        }
    }

    #endregion

    #region Radix Sort
    private IEnumerator RadixSort()
    {
        yield return new WaitUntil(OperationsAvailable);
        int key = _size-1;
        if(WaitForNextOperation())
            yield return new WaitUntil(OperationsAvailable);
        int exp = 1;
        while (key / exp > 1)
        {
            yield return CountingSort(exp);
            exp *= 10;
        }
    }

    private IEnumerator CountingSort(int exp)
    {
        for (int i = 0; i < _size; ++i)
        {
            int b = (_elements[i].Value / exp) % 10;
            if(WaitForNextOperation())
                yield return new WaitUntil(OperationsAvailable);
            MoveToBucket(i,b);
            if(WaitForNextOperation())
                yield return new WaitUntil(OperationsAvailable);
        }

        int j = 0;
        for (int b = 0; b < 10; ++b)
        {
            while (_buckets[b].Count > 0)
            {
                MoveFromBucket(j, b);
                if(WaitForNextOperation())
                    yield return new WaitUntil(OperationsAvailable);
                j++;
            }
        }
    }
    #endregion

    #region Shell Sort

    private IEnumerator ShellSort()
    {
        yield return new WaitUntil(OperationsAvailable);
        int gap = _size / 2;
        while (gap > 0)
        {
            for (int i = gap; i < _size; ++i)
            {
                int j = i;
                while (j >= gap)
                {
                    bool result = CompareGreater(j - gap, j);
                    if (WaitForNextOperation())
                        yield return new WaitUntil(OperationsAvailable);
                    if (!result)
                        break;
                    
                    Swap(j, j-gap);
                    if (WaitForNextOperation())
                        yield return new WaitUntil(OperationsAvailable);
                    j -= gap;
                }
            }
            gap /= 2;
        }
    }

    #endregion
    
    #endregion
}
