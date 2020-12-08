using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuickSort : SortingAlgorithm
{
    public override List<string> Pseudocode
    {
        get => new List<string>()
        {
            "<style=\"sortingTitle\">Quick Sort:</style>",
            "<style=\"sortingFunction\">quickSort</style>(l, r):",
            "    <style=\"sortingKeyword\">if</style> l<r:",
            "        k = <style=\"sortingFunction\">partition</style>(l,r)",
            "        <style=\"sortingFunction\">quickSort</style>(l,k-<style=\"sortingNumber\">1</style>)",
            "        <style=\"sortingFunction\">quickSort</style>(k+<style=\"sortingNumber\">1</style>,r)",
            "",
            "<style=\"sortingFunction\">partition</style>(l, r):",
            "    p = A[r] <style=\"sortingComment\">//pivot element</style>",
            "    k = l",
            "    <style=\"sortingKeyword\">for</style> j = l .. r-<style=\"sortingNumber\">1</style>:",
            "        <style=\"sortingKeyword\">if</style> A[j]<=p:",
            "            <style=\"sortingFunction\">swap</style>(j,k)",
            "            k = k+<style=\"sortingNumber\">1</style>",
            "    <style=\"sortingFunction\">swap</style>(k,r)",
            "    <style=\"sortingKeyword\">return</style> k"
        };
    }
    
    public QuickSort(SortingLogic logic, int n, bool battleMode) : base(logic, battleMode)
    {
        _executedStates.AddFirst(new QuickSortingState(this, n));
    }
    
    private class QuickSortingState : SortingState
    {

        public QuickSortingState(QuickSortingState old) : base(old)
        {
        }

        public QuickSortingState(QuickSort algorithm, int n): base(algorithm)
        {
            _variables.Add("j", -1);
            _variables.Add("k", 0);
            _variables.Add("l", 0);
            _variables.Add("r", n-1);
            _variables.Add("pInd", -1);
        }

        public override SortingState Next()
        {
            QuickSortingState next = new QuickSortingState(this);
            return InitializeNext(next);
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
            int pInd = _variables["pInd"];
            
            switch (_line)
            {
                case SortingStateLine.SS_Line1: // quickSort(l,r):
                    _nextLine = SortingStateLine.SS_Line2;
                    pInd = -1;
                    break;
                case SortingStateLine.SS_Line2: // if(l<r):
                    if (l < r)
                    {
                        _nextLine = SortingStateLine.SS_Line3;
                    }
                    else
                    {
                        LeaveSubroutine();
                    }
                    break;
                case SortingStateLine.SS_Line3: // k = partition(l,r)
                    _nextLine = SortingStateLine.SS_Line7;
                    break;
                case SortingStateLine.SS_Line4: // quickSort(l,k-1)
                    EnterSubroutineWithExitLine(SortingStateLine.SS_Line5);
                    _nextValues = new Dictionary<string, int>(_variables);
                    _nextValues["r"] = k-1;
                    _nextLine = SortingStateLine.SS_Line1;
                    break;
                case SortingStateLine.SS_Line5: // quickSort(k+1,r)
                    EnterSubroutineWithExitLine(SortingStateLine.SS_None);
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
                    pInd = r;
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
                    if (_algorithm.CompareGreater(j, pInd))
                    {
                        _nextLine = SortingStateLine.SS_Line10;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_Line12;
                    }
                    break;
                case SortingStateLine.SS_Line12: // swap(j,k)
                    _nextLine = SortingStateLine.SS_Line13;
                    if (j != k)
                    {
                        _algorithm.Swap(j,k);
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
                        if (pInd == k) //Just for reversing
                        {
                            pInd = r;
                        }
                        _algorithm.Swap(k,r);
                    }
                    break;
                case SortingStateLine.SS_Line15: // return k
                    pInd = k;
                    _nextLine = SortingStateLine.SS_Line4;
                    break;
                case SortingStateLine.SS_None:
                    break;
            }
            
            _variables["j"] = j;
            _variables["k"] = k;
            _variables["l"] = l;
            _variables["r"] = r;
            _variables["pInd"] = pInd;
        }

        public override void Undo()
        {
            int j = _variables["j"];
            int k = _variables["k"];
            int l = _variables["l"];
            int r = _variables["r"];
            int pInd = _variables["pInd"];
            
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
                    _algorithm.UndoGreater(j,pInd);
                    break;
                case SortingStateLine.SS_Line12: // swap(j,k)
                    if (j != k)
                    {
                        _algorithm.UndoSwap(j,k);
                    }
                    break;
                case SortingStateLine.SS_Line13: // k = k+1
                    break;
                case SortingStateLine.SS_Line14: // swap(k,r)
                    if (k != r)
                    {
                        _algorithm.UndoSwap(k,r);
                    }
                    break;
                case SortingStateLine.SS_Line15: // return k
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
        
        public override Dictionary<string, int> GetIndexVariables()
        {
            var indexVariables = new Dictionary<string, int>(_variables);
            indexVariables.Remove("pInd");
            return indexVariables;
        }

        public override Dictionary<string, int> GetExtraVariables()
        {
            var extraVariables = new Dictionary<string, int>();
            extraVariables.Add("pInd", _variables["pInd"]);
            return extraVariables;
        }
    }
}