using System.Collections.Generic;
using System.Linq;
using GEAR.Localization;
using UnityEngine;

public abstract class SortingAlgorithm
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
    
    private SortingLogic sortingLogic;
    public abstract List<string> Pseudocode { get; }
    
    protected LinkedList<SortingState> _executedStates = new LinkedList<SortingState>();
    private SortingState _currentState;

    private int _comparisons;
    private int _swaps;

    protected enum SortingStateLine
    {
        SS_None,
        SS_Line1,
        SS_Line2,
        SS_Line3,
        SS_Line4,
        SS_Line5,
        SS_Line6,
        SS_Line7,
        SS_Line8,
        SS_Line9,
        SS_Line10,
        SS_Line11,
        SS_Line12,
        SS_Line13,
        SS_Line14,
        SS_Line15,
        SS_Line16,
        SS_Line17,
        SS_Line18,
        SS_Line19,
        SS_Line20,
        SS_Line21
    }

    protected abstract class SortingState
    {
        protected SortingAlgorithm _algorithm;
        
        protected SortingStateLine _line;
        protected SortingStateLine _nextLine;
        protected Dictionary<string, int> _variables = new Dictionary<string, int>();
        
        //Stacks for subroutine calling
        private Stack<Dictionary<string, int>> _valueStore = new Stack<Dictionary<string, int>>();
        private Stack<SortingStateLine> _continueLine = new Stack<SortingStateLine>();
        //when this variable is set, the next state will start with _variables set to _nextValues
        protected Dictionary<string, int> _nextValues = null;

        public virtual SortingState Next() {return this;}
        public virtual SortingState Copy() {return this;}

        public virtual void Execute() {}
        public virtual void Undo() {}
        
        public virtual int GetSubsetStart() {return -1;}
        public virtual int GetSubsetEnd() {return -1;}
        
        public virtual Dictionary<string, int> GetIndexVariables() { return null; }
        
        public virtual Dictionary<string, int> GetExtraVariables() { return null; }

        protected SortingState()
        {
            _line = SortingStateLine.SS_None;
        }

        protected SortingState(SortingAlgorithm algorithm)
        {
            _algorithm = algorithm;
            
            _line = SortingStateLine.SS_None;
            _nextLine = SortingStateLine.SS_Line1;
            _nextValues = null;
        }

        protected SortingState(SortingState old)
        {
            _algorithm = old._algorithm;
            _line = old._line;
            _nextLine = old._nextLine;
            _variables = new Dictionary<string, int>(old._variables);
            
            //subroutine
            _nextValues = old._nextValues != null ? new Dictionary<string, int>(old._nextValues) : null;
            _continueLine = new Stack<SortingStateLine>(old._continueLine.Reverse()); //reverse because this method iteratively pops the stack items
            
            //Deep copy _valueStore
            _valueStore = new Stack<Dictionary<string, int>>();
            foreach (var dict in old._valueStore.Reverse())
            {
                _valueStore.Push(new Dictionary<string, int>(dict));
            }
            
        }

        protected SortingState InitializeNext(SortingState next)
        {
            next._line = _nextLine;
            next._nextValues = null;
            if (_nextValues != null)
            {
                next._variables = new Dictionary<string, int>(_nextValues);
            }

            return next;
        }

        public SortingStateLine GetLine()
        {
            return _line;
        }
        
        protected void EnterSubroutineWithExitLine(SortingStateLine line)
        {
            _continueLine.Push(line);
            _valueStore.Push(new Dictionary<string, int>(_variables));
        }

        protected void LeaveSubroutine()
        {
            if (_continueLine.Count == 0)
            {
                _nextLine = SortingStateLine.SS_None;
                return;
            }
            _nextLine = _continueLine.Pop();
            _nextValues = _valueStore.Pop();
            if (_nextLine == SortingStateLine.SS_None)
            {
                //leave more than one subroutine at once
                LeaveSubroutine();
            }
        }
    }

    protected SortingAlgorithm(SortingLogic logic)
    {
        sortingLogic = logic;
        
        _comparisons = 0;
        _swaps = 0;
        
        LanguageManager.Instance.OnLanguageChanged.AddListener(OnLanguageChanged);
    }

    public void ExecuteNextState()
    {
        SortingState newState = _executedStates.Last.Value.Next();
        sortingLogic.SetPseudocode((int)newState.GetLine(), newState.GetExtraVariables());
        if (newState.GetLine() != SortingStateLine.SS_None)
        {
            newState.Execute();
            _executedStates.AddLast(newState);
            sortingLogic.MarkCurrentSubset(newState.GetSubsetStart(), newState.GetSubsetEnd());
            sortingLogic.DisplayIndices(newState.GetIndexVariables());
        }
        else
        {
            sortingLogic.SortingFinished();
        }
        sortingLogic.SetSwapsComparisons(_swaps, _comparisons);
    }

    public void ExecutePreviousState()
    {
        if (_executedStates.Count == 1)
        {
            _comparisons = 0;
            _swaps = 0;
            sortingLogic.MarkCurrentSubset(-1,-1);
            return;
        }
        SortingState stateToUndo = _executedStates.Last.Value;
        stateToUndo.Undo();
        _executedStates.RemoveLast();
        SortingState currentState = _executedStates.Last.Value;
        sortingLogic.SetPseudocode((int)currentState.GetLine(), currentState.GetExtraVariables());
        sortingLogic.MarkCurrentSubset(currentState.GetSubsetStart(), currentState.GetSubsetEnd());
        sortingLogic.DisplayIndices(currentState.GetIndexVariables());
        sortingLogic.SetSwapsComparisons(_swaps, _comparisons);
    }

    public void Swap(int ind1, int ind2)
    {
        if (ind1 == ind2)
        {
            return;   
        }
        _swaps++;
        sortingLogic.Swap(ind1, ind2);
    }
    
    public void UndoSwap(int ind1, int ind2)
    {
        if (ind1 == ind2)
        {
            return;   
        }
        _swaps--;
        sortingLogic.Swap(ind2, ind1);
    }
    
    public void Insert(int ind1, int ind2)
    {
        if (ind1 == ind2)
        {
            return;   
        }
        _swaps++;
        sortingLogic.Insert(ind1, ind2);
    }
    
    public void UndoInsert(int ind1, int ind2)
    {
        if (ind1 == ind2)
        {
            return;   
        }
        _swaps--;
        sortingLogic.Insert(ind2, ind1);
    }

    public bool CompareGreater(int ind1, int ind2)
    {
        if (ind1 == ind2)
        {
            return false;   
        }
        _comparisons++;
        return sortingLogic.CompareGreater(ind1, ind2);
    }

    public void UndoGreater(int ind1, int ind2)
    {
        if (ind1 == ind2)
        {
            return;   
        }
        _comparisons--;
        sortingLogic.CompareGreater(ind1, ind2);
    }

    public int GetMaxValue()
    {
        _comparisons++;
        return sortingLogic.GetMaxValue();
    }
    
    public void UndoGetMaxValue()
    {
        _comparisons--;
        sortingLogic.GetMaxValue();
    }
    
    public int GetBucketNumber(int ind, int exp)
    {
        _comparisons++;
        return sortingLogic.GetBucketNumber(ind, exp);
    }
    
    public void UndoGetBucketNumber(int ind, int exp)
    {
        _comparisons--;
        sortingLogic.GetBucketNumber(ind, exp);
    }

    public void MoveToBucket(int ind, int bucket)
    {
        _swaps++;
        sortingLogic.MoveToBucket(ind, bucket);
    }
    
    public void MoveFromBucket(int ind, int bucket)
    {
        _swaps++;
        sortingLogic.MoveFromBucket(ind, bucket);
    }

    public void UndoMoveToBucket(int ind, int bucket)
    {
        _swaps--;
        sortingLogic.UndoMoveToBucket(ind, bucket);
    }
    
    public void UndoMoveFromBucket(int ind, int bucket)
    {
        _swaps--;
        sortingLogic.UndoMoveFromBucket(ind, bucket);
    }
    
    public bool BucketEmpty(int bucket)
    {
        return sortingLogic.BucketEmpty(bucket);
    }

    private void OnLanguageChanged(SystemLanguage language)
    {
        sortingLogic.SetSwapsComparisons(_swaps, _comparisons);
    }
}