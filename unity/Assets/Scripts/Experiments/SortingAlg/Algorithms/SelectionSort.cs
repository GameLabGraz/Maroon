using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSort : SortingAlgorithm
{
    public override List<string> pseudocode
    {
        get => new List<string>()
        {
            "<style=\"header\">Selection Sort:</style>",
            "<style=\"command\">for</style> i = <style=\"number\">0</style> .. <style=\"command\">len</style>(A)-<style=\"number\">1</style>:",
            "    m = i",
            "    <style=\"command\">for</style> j = i+<style=\"number\">1</style> .. <style=\"command\">len</style>(A)-<style=\"number\">1</style>:",
            "        <style=\"command\">if</style> A[j]<A[m]:",
            "            m = j",
            "    <style=\"command\">if</style> i != m:",
            "         <style=\"function\">swap</style>(i,m)",
        };
    }
    
    public SelectionSort(SortingLogic logic, int n) : base(logic)
    {
        _nextState = new SelectionSortingState(this, n);
    }

    private class SelectionSortingState : SortingState
    {
        public SelectionSortingState(SelectionSortingState old) : base(old) {}

        public SelectionSortingState(SelectionSort algorithm, int n): base(algorithm)
        {
            _variables.Add("n", n);
            _variables.Add("i", -1);
            _variables.Add("j", -1);
            _variables.Add("m", -1);
        }

        public override SortingState Next()
        {
            SelectionSortingState next = new SelectionSortingState(this);
            next._line = _nextLine;
            return next;
        }
        
        public override SortingState Copy()
        {
            SelectionSortingState copy = new SelectionSortingState(this);
            return copy;
        }

        public override void Execute()
        {
            int n = _variables["n"];
            int i = _variables["i"];
            int j = _variables["j"];
            int m = _variables["m"];
            switch (_line)
            {
                case SortingStateLine.SS_Line1: // for i = 0 .. len(A)-1:
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
                case SortingStateLine.SS_Line2: // m = i
                    m = i;
                    j = i;
                    _nextLine = SortingStateLine.SS_Line3;
                    break;
                case SortingStateLine.SS_Line3: // for j = i+1 .. len(A)-1:
                    j++;
                    if (j < n)
                    {
                        _nextLine = SortingStateLine.SS_Line4;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_Line6;
                    }
                    break;
                case SortingStateLine.SS_Line4: // if A[j]<A[m]:
                    if (_algorithm.CompareGreater(m, j))
                    {
                        _nextLine = SortingStateLine.SS_Line5;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_Line3;
                    }
                    _requireWait = true;
                    break;
                case SortingStateLine.SS_Line5: // m = j
                    m = j;
                    _nextLine = SortingStateLine.SS_Line3;
                    break;
                case SortingStateLine.SS_Line6: // if i != m:
                    if (i != m)
                    {
                        _nextLine = SortingStateLine.SS_Line7;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_Line1;
                    }
                    break;
                case SortingStateLine.SS_Line7: // swap(i,m)
                    _requireWait = true;
                    _algorithm.Swap(i,m);
                    _nextLine = SortingStateLine.SS_Line1;
                    break;
                case SortingStateLine.SS_None:
                    break;
            }

            _variables["n"] = n;
            _variables["i"] = i;
            _variables["j"] = j;
            _variables["m"] = m;
        }
        
        public override void Undo()
        {
            int i = _variables["i"];
            int m = _variables["m"];
            _requireWait = false;
            switch (_line)
            {
                case SortingStateLine.SS_Line1: // for i = 0 .. len(A)-1:
                    break;
                case SortingStateLine.SS_Line2: // m = i
                    break;
                case SortingStateLine.SS_Line3: // for j = i+1 .. len(A)-1:
                    break;
                case SortingStateLine.SS_Line4: // if A[j]<A[m]:
                    break;
                case SortingStateLine.SS_Line5: // m = j
                    break;
                case SortingStateLine.SS_Line6: // if i != m:
                    break;
                case SortingStateLine.SS_Line7: // swap(i,m)
                    _requireWait = true;
                    _algorithm.UndoSwap(i,m);
                    break;
                case SortingStateLine.SS_None:
                    break;
            }
        }
    }
}
