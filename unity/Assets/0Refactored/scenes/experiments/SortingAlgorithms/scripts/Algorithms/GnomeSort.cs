using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GnomeSort : SortingAlgorithm
{
    public override List<string> Pseudocode
    {
        get => new List<string>()
        {
            "<style=\"sortingTitle\">Gnome Sort:</style>",
            "<style=\"sortingKeyword\">while</style> i<<style=\"sortingKeyword\">len</style>(A):",
            "    <style=\"sortingKeyword\">if</style> i==<style=\"sortingNumber\">0</style>:",
            "        i = i+<style=\"sortingNumber\">1</style>",
            "    <style=\"sortingKeyword\">if</style> A[i]>=A[i-1]:",
            "        i = i+<style=\"sortingNumber\">1</style>",
            "    <style=\"sortingKeyword\">else:</style>",
            "        <style=\"sortingFunction\">swap</style>(i,i-1)",
            "        i = i-<style=\"sortingNumber\">1</style>"
        };
    }
    
    public GnomeSort(SortingLogic logic, int n, bool battleMode) : base(logic, battleMode)
    {
        _executedStates.AddFirst(new GnomeSortingState(this, n));
    }

    private class GnomeSortingState : SortingState
    {
        public GnomeSortingState(GnomeSortingState old) : base(old) {}

        public GnomeSortingState(GnomeSort algorithm, int n): base(algorithm)
        {
            _variables.Add("n", n);
            _variables.Add("i", 0);
        }

        public override SortingState Next()
        {
            GnomeSortingState next = new GnomeSortingState(this);
            return InitializeNext(next);
        }
        
        public override SortingState Copy()
        {
            GnomeSortingState copy = new GnomeSortingState(this);
            return copy;
        }

        public override void Execute()
        {
            int n = _variables["n"];
            int i = _variables["i"];
            switch (_line)
            {
                case SortingStateLine.SS_Line1: // while i<len(A):
                    if (i < n)
                    {
                        _nextLine = SortingStateLine.SS_Line2;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_None;
                    }
                    break;
                case SortingStateLine.SS_Line2: // if i==0:
                    if (i == 0)
                    {
                        _nextLine = SortingStateLine.SS_Line3;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_Line4;
                    }
                    break;
                case SortingStateLine.SS_Line3: // i = i+1
                    i++;
                    _nextLine = SortingStateLine.SS_Line4;
                    break;
                case SortingStateLine.SS_Line4: // if A[i]>=A[i-1]:
                    if (_algorithm.CompareGreater(i - 1, i)) // Inverted to Greater!
                    {
                        _nextLine = SortingStateLine.SS_Line7;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_Line5;
                    }
                    break;
                case SortingStateLine.SS_Line5: // i = i+1
                    i++;
                    _nextLine = SortingStateLine.SS_Line1;
                    break;
                case SortingStateLine.SS_Line6: // else
                    _nextLine = SortingStateLine.SS_Line7;
                    break;
                case SortingStateLine.SS_Line7: // swap(i,i-1)
                    _algorithm.Swap(i,i-1);
                    _nextLine = SortingStateLine.SS_Line8;
                    break;
                case SortingStateLine.SS_Line8: // i = i-1
                    i--;
                    _nextLine = SortingStateLine.SS_Line1;
                    break;
                case SortingStateLine.SS_None:
                    break;
            }

            _variables["n"] = n;
            _variables["i"] = i;
        }
        
        public override void Undo()
        {
            int i = _variables["i"];
            switch (_line)
            {
                case SortingStateLine.SS_Line1: // while i<len(A):
                    break;
                case SortingStateLine.SS_Line2: // if i==0:
                    break;
                case SortingStateLine.SS_Line3: // i = i+1
                    break;
                case SortingStateLine.SS_Line4: // if A[i]>=A[i-1]:
                    _algorithm.UndoGreater(i,i-1);
                    break;
                case SortingStateLine.SS_Line5: // i = i+1
                    break;
                case SortingStateLine.SS_Line6: // else
                    break;
                case SortingStateLine.SS_Line7: // swap(i,i-1)
                    _algorithm.UndoSwap(i,i-1);
                    break;
                case SortingStateLine.SS_Line8: // i = i-1
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
