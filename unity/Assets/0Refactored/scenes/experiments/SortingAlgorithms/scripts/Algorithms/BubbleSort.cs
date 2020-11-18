using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSort : SortingAlgorithm
{
    public override List<string> Pseudocode
    {
        get => new List<string>()
        {
            "<style=\"sortingTitle\">Bubble Sort:</style>",
            "<style=\"sortingKeyword\">for</style> i = <style=\"sortingNumber\">0</style> .. <style=\"sortingKeyword\">len</style>(A)-<style=\"sortingNumber\">1</style>:",
            "    <style=\"sortingKeyword\">for</style> j = <style=\"sortingNumber\">0</style> .. <style=\"sortingKeyword\">len(A)</style>-<style=\"sortingNumber\">1</style>-i:",
            "        <style=\"sortingKeyword\">if</style> A[j]>A[j+<style=\"sortingNumber\">1</style>]:",
            "            <style=\"sortingFunction\">swap</style>(j,j+<style=\"sortingNumber\">1</style>)"
        };
    }
    
    public BubbleSort(SortingLogic logic, int n) : base(logic)
    {
        _executedStates.AddFirst(new BubbleSortingState(this, n));
    }

    private class BubbleSortingState : SortingState
    {
        public BubbleSortingState(BubbleSortingState old) : base(old) {}

        public BubbleSortingState(BubbleSort algorithm, int n): base(algorithm)
        {
            _variables.Add("n", n);
            _variables.Add("i", -1);
            _variables.Add("j", -1);
        }

        public override SortingState Next()
        {
            BubbleSortingState next = new BubbleSortingState(this);
            return InitializeNext(next);
        }
        
        public override SortingState Copy()
        {
            BubbleSortingState copy = new BubbleSortingState(this);
            return copy;
        }

        public override void Execute()
        {
            int n = _variables["n"];
            int i = _variables["i"];
            int j = _variables["j"];
            switch (_line)
            {
                case SortingStateLine.SS_Line1: // for i = 0 .. len(A)-1:
                    i++;
                    
                    if (i < n)
                    {
                        _nextLine = SortingStateLine.SS_Line2;
                        _nextValues = new Dictionary<string, int>(_variables);
                        _nextValues["j"] = -1;
                        _nextValues["i"] = i;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_None;
                    }
                    break;
                case SortingStateLine.SS_Line2: // for j = 0 .. len(A)-1-i
                    j++;

                    if (j < n - i)
                    {
                        _nextLine = SortingStateLine.SS_Line3;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_Line1;
                    }
                    break;
                case SortingStateLine.SS_Line3: // if A[j]>A[j+1]:
                    _requireWait = true;
                    if (_algorithm.CompareGreater(j, j+1))
                    {
                        _nextLine = SortingStateLine.SS_Line4;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_Line2;
                    }
                    break;
                case SortingStateLine.SS_Line4: // swap(j,j+1)
                    _requireWait = true;
                    _algorithm.Swap(j, j+1);
                    _nextLine = SortingStateLine.SS_Line2;
                    break;
                case SortingStateLine.SS_None:
                    break;
            }

            _variables["n"] = n;
            _variables["i"] = i;
            _variables["j"] = j;
        }
        
        public override void Undo()
        {
            int j = _variables["j"];
            _requireWait = false;
            switch (_line)
            {
                case SortingStateLine.SS_Line1: // for i = 0 .. len(A)-1:
                    break;
                case SortingStateLine.SS_Line2: // for j = 0 .. len(A)-1-i
                    break;
                case SortingStateLine.SS_Line3: // if A[j]>A[j+1]:
                    _requireWait = true;
                    _algorithm.UndoGreater(j, j+1);
                    break;
                case SortingStateLine.SS_Line4: // swap(j,j+1)
                    _requireWait = true;
                    _algorithm.UndoSwap(j, j+1);
                    break;
            }
        }
        
        public override Dictionary<string, int> GetIndexVariables()
        {
            var indexVariables = new Dictionary<string, int>(_variables);
            indexVariables.Remove("n");
            return indexVariables;
        }
    }
}
