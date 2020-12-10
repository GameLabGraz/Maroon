using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsertionSort : SortingAlgorithm
{
    public override List<string> Pseudocode
    {
        get => new List<string>()
        {
            "<style=\"sortingTitle\">Insertion Sort:</style>",
            "<style=\"sortingKeyword\">for</style> i = <style=\"sortingNumber\">1</style> .. <style=\"sortingKeyword\">len</style>(A)-<style=\"sortingNumber\">1</style>:",
            "    j = i-<style=\"sortingNumber\">1</style>",
            "    <style=\"sortingKeyword\">while</style> A[j]>A[i] <style=\"sortingKeyword\">and</style> j>=<style=\"sortingNumber\">0</style>:",
            "        j = j-<style=\"sortingNumber\">1</style>",
            "    <style=\"sortingFunction\">insert</style>(i,j+<style=\"sortingNumber\">1</style>)"
        };
    }
    
    public InsertionSort(SortingLogic logic, int n) : base(logic)
    {
        _executedStates.AddFirst(new InsertionSortingState(this, n));
    }

    private class InsertionSortingState : SortingState
    {
        public InsertionSortingState(InsertionSortingState old) : base(old) {}

        public InsertionSortingState(InsertionSort algorithm, int n): base(algorithm)
        {
            _variables.Add("n", n);
            _variables.Add("i", 0);
            _variables.Add("j", -1);
        }

        public override SortingState Next()
        {
            InsertionSortingState next = new InsertionSortingState(this);
            return InitializeNext(next);
        }
        
        public override SortingState Copy()
        {
            InsertionSortingState copy = new InsertionSortingState(this);
            return copy;
        }

        public override void Execute()
        {
            int n = _variables["n"];
            int i = _variables["i"];
            int j = _variables["j"];
            switch (_line)
            {
                case SortingStateLine.SS_Line1: // for i = 1 .. len(A)-1:
                    i++;
                    
                    if (i < n)
                    {
                        _nextLine = SortingStateLine.SS_Line2;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_None;
                    }
                    break;
                case SortingStateLine.SS_Line2: // j = i-1
                    j = i - 1;
                    
                    _nextLine = SortingStateLine.SS_Line3;
                    break;
                case SortingStateLine.SS_Line3: // while A[j]>A[i] and j>=0:
                    if (j < 0)
                    {
                        _nextLine = SortingStateLine.SS_Line5;
                        break;
                    }
                    if (_algorithm.CompareGreater(j, i))
                    {
                        _nextLine = SortingStateLine.SS_Line4;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_Line5;
                    }
                    break;
                case SortingStateLine.SS_Line4: // j = j-1
                    j--;
                    _nextLine = SortingStateLine.SS_Line3;
                    break;
                case SortingStateLine.SS_Line5: //insert(i,j+1)
                    _algorithm.Insert(i,j+1);
                    _nextLine = SortingStateLine.SS_Line1;
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
            int i = _variables["i"];
            switch (_line)
            {
                case SortingStateLine.SS_Line1: // for i = 1 .. len(A)-1:
                    break;
                case SortingStateLine.SS_Line2: // j = i-1
                    break;
                case SortingStateLine.SS_Line3: // while A[j]>A[i] and j>=0:
                    _algorithm.UndoGreater(j,i);
                    break;
                case SortingStateLine.SS_Line4: // j = j-1
                    break;
                case SortingStateLine.SS_Line5: // //insert(i,j+1)
                    _algorithm.UndoInsert(i,j+1);
                    break;
                case SortingStateLine.SS_None:
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
