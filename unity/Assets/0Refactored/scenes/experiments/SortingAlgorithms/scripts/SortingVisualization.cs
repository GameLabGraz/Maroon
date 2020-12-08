using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SortingVisualization : MonoBehaviour
{
    protected float _timePerMove;

    public float TimePerMove
    {
        get => _timePerMove;
        set => _timePerMove = value;
    }

    protected int _size;
    protected bool _visualizationActive;
    
    protected SortingLogic _sortingLogic;
    
    public abstract int Size { get; set; }
    
    public virtual void Init(int size)
    {
        _sortingLogic = GetComponent<SortingLogic>();
        Size = size;
    }
    
    protected void StartVisualization()
    {
        if(_visualizationActive)
            FinishRunningVisualizations();
        _visualizationActive = true;
    }
    
    protected IEnumerator StopVisualizationAfterDelay()
    {
        yield return new WaitForSeconds(_timePerMove);
        _visualizationActive = false;
    }

    public abstract void SortingFinished();
    
    public virtual void HideBuckets() {}
    
    public virtual void ShowBuckets() {}

    public abstract void Insert(int fromIdx, int toIdx);

    public abstract void Swap(int fromIdx, int toIdx);

    public abstract void CompareGreater(int idx1, int idx2);

    public virtual void VisualizeMaxValue(int maxIndex) {}

    public virtual void VisualizeBucketNumber(int ind, int bucket) {}

    public abstract void MoveToBucket(int from, int bucket);
    
    public virtual void UndoMoveToBucket(int from, int bucket, int value) {}

    public abstract void MoveFromBucket(int to, int bucket, int value);

    public virtual void UndoMoveFromBucket(int to, int bucket) {}

    public virtual void MarkCurrentSubset(int from, int to) {}
    
    public virtual void SetSwapsComparisons(int swaps, int comparisons) {}
    
    public virtual void DisplayPseudocode(List<string> pseudocode, int highlightLine, Dictionary<string, int> extraVars) {}
    
    public virtual void SetDescription(string key) {}
    
    public virtual void SetTitle(string title) {}

    public virtual void DisplayIndices(Dictionary<string, int> indices) {}

    public virtual void NewAlgorithmSelected()
    {
        ResetVisualization();
    }

    protected abstract void FinishRunningVisualizations();

    public abstract void ResetVisualization();
}
