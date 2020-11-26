using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SortingSpriteArray : SortingVisualization
{
    //TODO: ResetObject outside??
    
    [SerializeField] private GameObject columnPrefab;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI operationsText;

    private List<int> _startOrder;

    public override int Size
    {
        get => _size;
        set => _size = value;
    }

    private SortingController _sortingController;
    
    private List<SortingColumn> _elements = new List<SortingColumn>();
    private List<Queue<int>> _buckets = new List<Queue<int>>();
    
    public override void Init(int size)
    {
        base.Init(size);
        _sortingController = GetComponentInParent<SortingController>();
        Size = size;
        CreateColumnArray();
        for (int i = 0; i < 10; ++i)
        {
            _buckets.Add(new Queue<int>());
        }
        _sortingLogic.Init();
    }

    public override void SortingFinished()
    {
        //TODO!
    }

    public override void Insert(int fromIdx, int toIdx)
    {
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

        StartCoroutine(WaitForMove());
    }

    public override void Swap(int fromIdx, int toIdx)
    {
        int fromValue = _elements[fromIdx].Value;
        _elements[fromIdx].Value = _elements[toIdx].Value;
        _elements[toIdx].Value = fromValue;

        StartCoroutine(WaitForMove());
    }

    public override bool CompareGreater(int idx1, int idx2)
    {
        StartCoroutine(WaitForMove());
        _elements[idx1].HighlightForSeconds(timePerMove);
        _elements[idx2].HighlightForSeconds(timePerMove);
        return _elements[idx1].Value > _elements[idx2].Value;
    }

    public override int GetMaxValue()
    {
        StartCoroutine(WaitForMove());
        int max = 0;
        foreach (var element in _elements)
        {
            if (element.Value > max)
            {
                max = element.Value;
            }
        }
        return max;
    }

    public override int GetBucketNumber(int ind, int exp)
    {
        StartCoroutine(WaitForMove());
        
        return (_elements[ind].Value / exp) % 10;
    }

    public override void MoveToBucket(int from, int bucket)
    {
        StartCoroutine(WaitForMove());
        _buckets[bucket].Enqueue(_elements[from].Value);
        _elements[from].MarkNotSubset();
    }

    public override void MoveFromBucket(int to, int bucket)
    {
        StartCoroutine(WaitForMove());
        _elements[to].Value = _buckets[bucket].Dequeue();
        _elements[to].MarkSubset();
    }

    public override bool BucketEmpty(int bucket)
    {
        return _buckets[bucket].Count == 0;
    }

    public override void MarkCurrentSubset(int from, int to)
    {
        for (int i = 0; i < Size; ++i)
        {
            if (i >= from && i <= to)
            {
                _elements[i].MarkSubset();
            }
            else
            {
                _elements[i].MarkNotSubset();
            }
        }
    }

    public override void SetSwapsComparisons(int swaps, int comparisons)
    {
        int operations = swaps + comparisons;
        operationsText.text = "Operations: " + operations;
    }

    public override void SetTitle(string title)
    {
        titleText.text = title;
    }

    private void CreateColumnArray()
    {
        for (int i = 0; i < Size; ++i)
        {
            var newColumn = Instantiate(columnPrefab, transform, false).GetComponent<SortingColumn>();
            newColumn.Init(Size, i, i);
            _elements.Add(newColumn);
        }
    }

    public void EnterDetailMode()
    {
        int selectedAlgorithm = (int)_sortingLogic.SelectedSortingAlgorithm;
        _sortingController.EnterDetailMode(selectedAlgorithm);
    }

    public override void NewAlgorithmSelected()
    {
        base.NewAlgorithmSelected();
        _sortingController.NewAlgorithmSelected();
    }

    public void ReorderElements(List<int> order)
    {
        _startOrder = order;
        for (int i = 0; i < Size; ++i)
        {
            _elements[i].Value = order[i];
        }
        _sortingLogic.ResetAlgorithm();
    }

    public void RestoreOrder()
    {
        ReorderElements(_startOrder);
    }

    public override void ResetVisualization()
    {
        foreach (var column in _elements)
        {
            column.ResetVisualization();
        }
    }
}
