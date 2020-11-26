using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SortingVisualization : MonoBehaviour
{
    [SerializeField] protected float timePerMove;
    
    protected int _size;
    
    protected SortingLogic _sortingLogic;
    
    public abstract int Size { get; set; }
    
    public virtual void Init(int size)
    {
        _sortingLogic = GetComponent<SortingLogic>();
    }

    public abstract void SortingFinished();
    
    public virtual void HideBuckets() {}
    
    public virtual void ShowBuckets() {}

    public abstract void Insert(int fromIdx, int toIdx);

    public abstract void Swap(int fromIdx, int toIdx);

    public abstract bool CompareGreater(int idx1, int idx2);

    public abstract int GetMaxValue();

    public abstract int GetBucketNumber(int ind, int exp);

    public abstract void MoveToBucket(int from, int bucket);
    
    public virtual void UndoMoveToBucket(int from, int bucket) {}

    public abstract void MoveFromBucket(int to, int bucket);

    public virtual void UndoMoveFromBucket(int to, int bucket) {}

    public abstract bool BucketEmpty(int bucket);
    
    public virtual void MarkCurrentSubset(int from, int to) {}
    
    public virtual void SetSwapsComparisons(int swaps, int comparisons) {}
    
    public virtual void DisplayPseudocode(List<string> pseudocode, int highlightLine, Dictionary<string, int> extraVars) {}
    
    public virtual void SetDescription(string key) {}
    
    public virtual void SetTitle(string title) {}

    public virtual void DisplayIndices(Dictionary<string, int> indices) {}
    
    protected IEnumerator WaitForMove()
    {
        yield return new WaitForSeconds(timePerMove);
        _sortingLogic.MoveFinished();
    }
}
