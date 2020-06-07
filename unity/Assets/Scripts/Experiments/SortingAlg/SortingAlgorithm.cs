using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class SortingAlgorithm
{
    public SortingLogic sortingLogic;
    abstract public List<string> pseudocode { get; }
    
    protected Stack<SortingState> _executedStates = new Stack<SortingState>();
    protected SortingState _nextState;
    
    protected int _operations;
    protected int _swaps;
    
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
        SS_Line15
    }

    protected class SortingState
    {
        protected SortingAlgorithm _algorithm;
        
        protected SortingStateLine _line;
        protected SortingStateLine _nextLine;
        protected Dictionary<string, int> _variables = new Dictionary<string, int>();
        public bool _requireWait;
        
        //Stacks for subroutine calling
        protected Stack<Dictionary<string, int>> _valueStore = new Stack<Dictionary<string, int>>();
        protected Stack<SortingStateLine> _continueLine = new Stack<SortingStateLine>();
        //when this variable is set, the next state will start with _variables set to _nextValues
        protected Dictionary<string, int> _nextValues = null;

        public virtual SortingState Next() {return this;}
        public virtual SortingState Copy() {return this;}

        public virtual void Execute() {}
        public virtual void Undo() {}
        
        public virtual int GetSubsetStart() {return -1;}
        public virtual int GetSubsetEnd() {return -1;}
        
        public virtual int GetPivot() {return -1;}

        public SortingState()
        {
            _line = SortingStateLine.SS_None;
        }
        
        public SortingState(SortingAlgorithm algorithm)
        {
            _algorithm = algorithm;
            
            _line = SortingStateLine.SS_Line1;
            _nextLine = SortingStateLine.SS_Line2;
            _requireWait = false;
        }
        
        public SortingState(SortingState old)
        {
            _algorithm = old._algorithm;
            _line = old._line;
            _nextLine = old._nextLine;
            _variables = new Dictionary<string, int>(old._variables);
            _requireWait = false;
            
            //subroutine
            _nextValues = null;
            _continueLine = old._continueLine;
            _valueStore = old._valueStore;
        }

        public SortingStateLine GetLine()
        {
            return _line;
        }

        public Dictionary<string, int> GetVariables()
        {
            return _variables;
        }
        
        protected void enterSubroutineWithExitLine(SortingStateLine line)
        {
            _continueLine.Push(line);
            _valueStore.Push(new Dictionary<string, int>(_variables));
        }

        protected void leaveSubroutine()
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
                leaveSubroutine();
            }
        }
    }

    public SortingAlgorithm(SortingLogic logic)
    {
        sortingLogic = logic;
        
        _operations = 0;
        _swaps = 0;
        
        _executedStates.Push(new SortingState());
    }

    public void ExecuteNextState()
    {
        sortingLogic.setPseudocode((int)_nextState.GetLine());
        if (_nextState.GetLine() != SortingStateLine.SS_None)
        {
            _operations++;
            _executedStates.Push(_nextState.Copy());
            _nextState.Execute();
            sortingLogic.markCurrentSubset(_nextState.GetSubsetStart(), _nextState.GetSubsetEnd());
            sortingLogic.markPivot(_nextState.GetPivot());
            sortingLogic.displayIndices(_nextState.GetVariables());
            if (!_nextState._requireWait)
            {
                sortingLogic.MoveFinished();
            }
            _nextState = _nextState.Next();
        }
        else
        {
            sortingLogic.MoveFinished();
            sortingLogic.sortingFinished();
        }
        sortingLogic.setSwapsOperations(_swaps, _operations);
    }

    public void ExecutePreviousState()
    {
        if (_executedStates.Peek().GetLine() == SortingStateLine.SS_None)
        {
            _operations = 0;
            _swaps = 0;
            sortingLogic.MoveFinished();
            sortingLogic.markCurrentSubset(-1,-1);
            return;
        }
        _operations--;
        _nextState = _executedStates.Pop();
        sortingLogic.setPseudocode((int)_executedStates.Peek().GetLine());
        _nextState.Undo();
        sortingLogic.markCurrentSubset(_executedStates.Peek().GetSubsetStart(), _executedStates.Peek().GetSubsetEnd());
        sortingLogic.markPivot(_nextState.GetPivot());
        sortingLogic.displayIndices(_executedStates.Peek().GetVariables());
        if (!_nextState._requireWait)
        {
            sortingLogic.MoveFinished();
        }
        sortingLogic.setSwapsOperations(_swaps, _operations);
    }

    public void Swap(int ind1, int ind2)
    {
        _swaps++;
        sortingLogic.Swap(ind1, ind2);
    }
    
    public void UndoSwap(int ind1, int ind2)
    {
        _swaps--;
        sortingLogic.Swap(ind2, ind1);
    }
    
    public void Insert(int ind1, int ind2)
    {
        _swaps++;
        sortingLogic.Insert(ind1, ind2);
    }
    
    public void UndoInsert(int ind1, int ind2)
    {
        _swaps--;
        sortingLogic.Insert(ind2, ind1);
    }

    public bool CompareGreater(int ind1, int ind2)
    {
        return sortingLogic.CompareGreater(ind1, ind2);
    }
}