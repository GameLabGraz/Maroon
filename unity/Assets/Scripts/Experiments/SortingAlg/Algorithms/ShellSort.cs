using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellSort : SortingAlgorithm
{
    public override List<string> pseudocode
    {
        get => new List<string>()
        {
            "<style=\"header\">Shell Sort:</style>",
            "gap = <style=\"command\">len</style>(A)/<style=\"number\">2</style>",
            "<style=\"command\">while</style> gap><style=\"number\">0</style>:",
            "    <style=\"command\">for</style> i = gap .. <style=\"command\">len</style>(A)-<style=\"number\">1</style>:",
            "        j = i",
            "        <style=\"command\">while</style> A[j-gap]>A[j] <style=\"command\">and</style> j>=gap:",
            "            <style=\"function\">swap</style>(j,j-gap)",
            "            j = j-gap",
            "    gap = gap/<style=\"number\">2</style>"
        };
    }
    
    public ShellSort(SortingLogic logic, int n) : base(logic)
    {
        _nextState = new ShellSortingState(this, n);
    }

    private class ShellSortingState : SortingState
    {
        public ShellSortingState(ShellSortingState old) : base(old) {}

        public ShellSortingState(ShellSort algorithm, int n): base(algorithm)
        {
            _variables.Add("n", n);
            _variables.Add("i", -1);
            _variables.Add("j", -1);
            _variables.Add("gap", -1);
        }

        public override SortingState Next()
        {
            ShellSortingState next = new ShellSortingState(this);
            next._line = _nextLine;
            return next;
        }
        
        public override SortingState Copy()
        {
            ShellSortingState copy = new ShellSortingState(this);
            return copy;
        }

        public override void Execute()
        {
            int n = _variables["n"];
            int i = _variables["i"];
            int j = _variables["j"];
            int gap = _variables["gap"];
            switch (_line)
            {
                case SortingStateLine.SS_Line1: // gap = len(A)/2
                    gap = n / 2;
                    _nextLine = SortingStateLine.SS_Line2;
                    break;
                case SortingStateLine.SS_Line2: // while gap>0:
                    if (gap > 0)
                    {
                        _nextLine = SortingStateLine.SS_Line3;
                        i = gap - 1;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_None;
                    }
                    break;
                case SortingStateLine.SS_Line3: // for i = gap .. len(A)-1:
                    i++;
                    if (i < n)
                    {
                        _nextLine = SortingStateLine.SS_Line4;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_Line8;
                    }
                    break;
                case SortingStateLine.SS_Line4: // j = i
                    j = i;
                    _nextLine = SortingStateLine.SS_Line5;
                    break;
                case SortingStateLine.SS_Line5: // while A[j-gap]>A[j] and j>=gap:
                    if (j < gap)
                    {
                        _nextLine = SortingStateLine.SS_Line3;
                        break;
                    }
                    _requireWait = true;
                    if (_algorithm.CompareGreater(j-gap, j))
                    {
                        _nextLine = SortingStateLine.SS_Line6;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_Line3;
                    }
                    break;
                case SortingStateLine.SS_Line6: // swap(j,j-gap)
                    _requireWait = true;
                    _algorithm.Swap(j, j-gap);
                    _nextLine = SortingStateLine.SS_Line7;
                    break;
                case SortingStateLine.SS_Line7: // j = j-gap
                    j = j - gap;
                    _nextLine = SortingStateLine.SS_Line5;
                    break;
                case SortingStateLine.SS_Line8: // gap = gap/2"
                    gap = gap / 2;
                    _nextLine = SortingStateLine.SS_Line2;
                    break;
                case SortingStateLine.SS_None:
                    break;
            }

            _variables["n"] = n;
            _variables["i"] = i;
            _variables["j"] = j;
            _variables["gap"] = gap;
        }
        
        public override void Undo()
        {
            int j = _variables["j"];
            int gap = _variables["gap"];
            _requireWait = false;
            switch (_line)
            {
                case SortingStateLine.SS_Line1: // gap = len(A)/2
                    break;
                case SortingStateLine.SS_Line2: // while gap>0:
                    break;
                case SortingStateLine.SS_Line3: // for i = gap .. len(A)-1:
                    break;
                case SortingStateLine.SS_Line4: // j = i
                    break;
                case SortingStateLine.SS_Line5: // while A[j-gap]>A[j] and j>=gap:
                    break;
                case SortingStateLine.SS_Line6: // swap(j,j-gap)
                    _requireWait = true;
                    _algorithm.UndoSwap(j, j-gap);
                    break;
                case SortingStateLine.SS_Line7: // j = j-gap
                    break;
                case SortingStateLine.SS_Line8: // gap = gap/2"
                    break;
                case SortingStateLine.SS_None:
                    break;
            }
        }
    }
}
