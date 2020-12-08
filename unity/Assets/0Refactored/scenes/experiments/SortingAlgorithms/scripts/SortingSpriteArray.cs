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

    public override int Size
    {
        get => _size;
        set
        {
            _size = value;
        }
    }

    private SortingController _sortingController; //TODO: Not needed?
    
    private List<SortingColumn> _elements = new List<SortingColumn>();
    
    public override void Init(int size)
    {
        base.Init(size);
        _sortingController = GetComponentInParent<SortingController>(); //TODO??
        CreateColumnArray();
    }

    public override void SortingFinished()
    {
        //TODO!
    }

    public override void Insert(int fromIdx, int toIdx)
    {
        StartVisualization();
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

        StartCoroutine(StopVisualizationAfterDelay());
    }

    public override void Swap(int fromIdx, int toIdx)
    {
        StartVisualization();
        int fromValue = _elements[fromIdx].Value;
        _elements[fromIdx].Value = _elements[toIdx].Value;
        _elements[toIdx].Value = fromValue;

        StartCoroutine(StopVisualizationAfterDelay());
    }

    public override void CompareGreater(int idx1, int idx2)
    {
        StartVisualization();
        _elements[idx1].HighlightForSeconds(_timePerMove);
        _elements[idx2].HighlightForSeconds(_timePerMove);
        StartCoroutine(StopVisualizationAfterDelay());
    }

    public override void VisualizeBucketNumber(int ind, int bucket)
    {
        StartVisualization();
        StartCoroutine(StopVisualizationAfterDelay());
    }

    public override void MoveToBucket(int from, int bucket)
    {
        StartVisualization();
        _elements[from].MarkNotSubset();
        StartCoroutine(StopVisualizationAfterDelay());
    }

    public override void MoveFromBucket(int to, int bucket, int value)
    {
        StartVisualization();
        _elements[to].Value = value;
        _elements[to].MarkSubset();
        StartCoroutine(StopVisualizationAfterDelay());
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

    protected override void FinishRunningVisualizations()
    {
        StopAllCoroutines();
        List<int> values = _sortingLogic.SortingValues;
        
        if (values.Count == 0)
            return;
        
        for(int i = 0; i < Size; ++i)
        {
            _elements[i].Value = values[i];
            _elements[i].ResetVisualization();
        }
        _visualizationActive = false;
    }

    public override void ResetVisualization()
    {
        FinishRunningVisualizations();
    }
}
