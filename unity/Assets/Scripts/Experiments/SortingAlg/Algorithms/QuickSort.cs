using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuickSort : SortingAlgorithm
{
    public override List<string> pseudocode
    {
        get => new List<string>()
        {
            "<style=\"header\">Quick Sort:</style>",
            "<style=\"function\">quickSort</style>(l, r):",
            "    <style=\"command\">if</style> l<r:",
            "        k = <style=\"function\">partition</style>(l,r)",
            "        <style=\"function\">quickSort</style>(l,k-<style=\"number\">1</style>)",
            "        <style=\"function\">quickSort</style>(k+<style=\"number\">1</style>,r)",
            "",
            "<style=\"function\">partition</style>(l, r):",
            "    p = A[r] <style=\"comment\">//pivot element</style>",
            "    k = l",
            "    <style=\"command\">for</style> j = l .. r-<style=\"number\">1</style>:",
            "        <style=\"command\">if</style> A[j]<=p:",
            "            <style=\"function\">swap</style>(j,k)",
            "            k = k+<style=\"number\">1</style>",
            "    <style=\"function\">swap</style>(k,r)",
            "    <style=\"command\">return</style> k"
        };
    }
    
    public QuickSort(SortingLogic logic, int n) : base(logic)
    {
        _nextState = new QuickSortingState(this, n);
    }
    
    private class QuickSortingState : SortingState
    {
        private int _pInd;

        public QuickSortingState(QuickSortingState old) : base(old)
        {
            _pInd = old._pInd;
        }

        public QuickSortingState(QuickSort algorithm, int n): base(algorithm)
        {
            _variables.Add("j", -1);
            _variables.Add("k", 0);
            _variables.Add("l", 0);
            _variables.Add("r", n-1);
            _pInd = -1;
        }

        public override SortingState Next()
        {
            QuickSortingState next = new QuickSortingState(this);
            next._line = _nextLine;
            if (_nextValues != null)
            {
                next._variables = _nextValues;
            }
            return next;
        }
        
        public override SortingState Copy()
        {
            QuickSortingState copy = new QuickSortingState(this);
            return copy;
        }

        public override void Execute()
        {
            int j = _variables["j"];
            int k = _variables["k"];
            int l = _variables["l"];
            int r = _variables["r"];
            
            switch (_line)
            {
                case SortingStateLine.SS_Line1: // quickSort(l,r):
                    _nextLine = SortingStateLine.SS_Line2;
                    _pInd = -1;
                    break;
                case SortingStateLine.SS_Line2: // if(l<r):
                    if (l < r)
                    {
                        _nextLine = SortingStateLine.SS_Line3;
                    }
                    else
                    {
                        leaveSubroutine();
                    }
                    break;
                case SortingStateLine.SS_Line3: // k = partition(l,r)
                    _nextLine = SortingStateLine.SS_Line7;
                    break;
                case SortingStateLine.SS_Line4: // quickSort(l,k-1)
                    enterSubroutineWithExitLine(SortingStateLine.SS_Line5);
                    _nextValues = new Dictionary<string, int>(_variables);
                    _nextValues["r"] = k-1;
                    _nextLine = SortingStateLine.SS_Line1;
                    break;
                case SortingStateLine.SS_Line5: // quickSort(k+1,r)
                    enterSubroutineWithExitLine(SortingStateLine.SS_None);
                    _nextValues = new Dictionary<string, int>(_variables);
                    _nextValues["l"] = k+1;
                    _nextLine = SortingStateLine.SS_Line1;
                    break;
                case SortingStateLine.SS_Line6: //
                    break;
                case SortingStateLine.SS_Line7: // partition(l, r):
                    _nextLine = SortingStateLine.SS_Line8;
                    break;
                case SortingStateLine.SS_Line8: // p = A[r]
                    //we use the index of p instead
                    //TODO: Set visual for pivot
                    _pInd = r;
                    _nextLine = SortingStateLine.SS_Line9;
                    break;
                case SortingStateLine.SS_Line9: // k = l
                    k = l;
                    _nextValues = new Dictionary<string, int>(_variables);
                    _nextValues["j"] = l-1;
                    _nextValues["k"] = l;
                    _nextLine = SortingStateLine.SS_Line10;
                    break;
                case SortingStateLine.SS_Line10: // for j in range(l,r):
                    j++;
                    if (j < r)
                    {
                        _nextLine = SortingStateLine.SS_Line11;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_Line14;
                    }
                    break;
                case SortingStateLine.SS_Line11: // if A[j] <= p:
                    if (_algorithm.CompareGreater(j, _pInd))
                    {
                        _nextLine = SortingStateLine.SS_Line10;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_Line12;
                    }
                    _requireWait = true;
                    break;
                case SortingStateLine.SS_Line12: // swap(j,k)
                    _nextLine = SortingStateLine.SS_Line13;
                    if (j != k)
                    {
                        _algorithm.Swap(j,k);
                        _requireWait = true;
                    }
                    break;
                case SortingStateLine.SS_Line13: // k = k+1
                    k++;
                    _nextLine = SortingStateLine.SS_Line10;
                    break;
                case SortingStateLine.SS_Line14: // swap(k,r)
                    _nextLine = SortingStateLine.SS_Line15;
                    if (k != r)
                    {
                        if (_pInd == k) //Just for reversing
                        {
                            _pInd = r;
                        }
                        _algorithm.Swap(k,r);
                        _requireWait = true;
                    }
                    break;
                case SortingStateLine.SS_Line15: // return k
                    _pInd = k;
                    _nextLine = SortingStateLine.SS_Line4;
                    break;
                case SortingStateLine.SS_None:
                    break;
            }
            
            _variables["j"] = j;
            _variables["k"] = k;
            _variables["l"] = l;
            _variables["r"] = r;
        }

        public override void Undo()
        {
            _requireWait = false;
            int j = _variables["j"];
            int k = _variables["k"];
            int l = _variables["l"];
            int r = _variables["r"];
            
            switch (_line)
            {
                case SortingStateLine.SS_Line1: // quickSort(l,r):
                    break;
                case SortingStateLine.SS_Line2: // if(l<r):
                    break;
                case SortingStateLine.SS_Line3: // k = partition(l,r)
                    break;
                case SortingStateLine.SS_Line4: // quickSort(l,k-1)
                    break;
                case SortingStateLine.SS_Line5: // quickSort(k+1,r)
                    break;
                case SortingStateLine.SS_Line6: //
                    break;
                case SortingStateLine.SS_Line7: // partition(l, r):
                    break;
                case SortingStateLine.SS_Line8: // p = A[r]
                    break;
                case SortingStateLine.SS_Line9: // k = l
                    break;
                case SortingStateLine.SS_Line10: // for j in range(l,r):
                    break;
                case SortingStateLine.SS_Line11: // if A[j] <= p:
                    break;
                case SortingStateLine.SS_Line12: // swap(j,k)
                    if (j != k)
                    {
                        _algorithm.UndoSwap(j,k);
                        _requireWait = true;
                    }
                    break;
                case SortingStateLine.SS_Line13: // k = k+1
                    break;
                case SortingStateLine.SS_Line14: // swap(k,r)
                    if (k != r)
                    {
                        _pInd = k;
                        _algorithm.UndoSwap(k,r);
                        _requireWait = true;
                    }
                    break;
                case SortingStateLine.SS_Line15: // return k
                    _pInd = k;
                    break;
                case SortingStateLine.SS_None:
                    break;
            }
        }

        public override int GetSubsetStart()
        {
            return _variables["l"];
        }
        
        public override int GetSubsetEnd()
        {
            return _variables["r"];
        }

        public override int GetPivot()
        {
            return _pInd;
        }
    }
}