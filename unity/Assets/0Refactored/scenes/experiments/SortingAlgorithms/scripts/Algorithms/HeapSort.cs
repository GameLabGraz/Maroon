using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeapSort : SortingAlgorithm
{
    public override List<string> Pseudocode
    {
        get => new List<string>()
        {
            "<style=\"sortingTitle\">Heap Sort:</style>",
            "n = <style=\"sortingKeyword\">len</style>(A)",
            "<i><color=#005500>//Build heap:</color></i>",
            "<style=\"sortingKeyword\">for</style> i = <style=\"sortingKeyword\">len</style>(A)/<style=\"sortingNumber\">2</style>+<style=\"sortingNumber\">1</style> .. <style=\"sortingNumber\">0</style>:",
            "    <style=\"sortingFunction\">heapify</style>(i,n)",
            "<i><color=#005500>//Iteratively take maximum:</color></i>",
            "<style=\"sortingKeyword\">for</style> i = <style=\"sortingKeyword\">len</style>(A)-<style=\"sortingNumber\">1</style> .. <style=\"sortingNumber\">1</style>:",
            "    <style=\"sortingFunction\">swap</style>(0,i)",
            "    n = n-<style=\"sortingNumber\">1</style>",
            "    <style=\"sortingFunction\">heapify</style>(0,n)",
            "",
            "<style=\"sortingFunction\">heapify</style>(j, n):",
            "    l = <style=\"sortingNumber\">2</style>*j+<style=\"sortingNumber\">1</style>",
            "    r = <style=\"sortingNumber\">2</style>*j+<style=\"sortingNumber\">2</style>",
            "    m = j",
            "    <style=\"sortingKeyword\">if</style> l<n <style=\"sortingKeyword\">and</style> A[l]>A[j]:",
            "        m = l",
            "    <style=\"sortingKeyword\">if</style> r<n <style=\"sortingKeyword\">and</style> A[r]>A[m]:",
            "        m = r",
            "    <style=\"sortingKeyword\">if</style> m!=j:",
            "        <style=\"sortingFunction\">swap</style>(j,m)",
            "        <style=\"sortingFunction\">heapify</style>(m,n)"
        };
    }
    
    public HeapSort(SortingLogic logic, int n, bool battleMode) : base(logic, battleMode)
    {
        _executedStates.AddFirst(new HeapSortingState(this, n));
    }

    private class HeapSortingState : SortingState
    {
        public HeapSortingState(HeapSortingState old) : base(old) {}

        public HeapSortingState(HeapSort algorithm, int n): base(algorithm)
        {
            _variables.Add("n", n);
            _variables.Add("i", n/2 + 2);
            _variables.Add("j", -1);
            _variables.Add("l", -1);
            _variables.Add("r", -1);
            _variables.Add("m", -1);
        }

        public override SortingState Next()
        {
            HeapSortingState next = new HeapSortingState(this);
            return InitializeNext(next);
        }
        
        public override SortingState Copy()
        {
            HeapSortingState copy = new HeapSortingState(this);
            return copy;
        }

        public override void Execute()
        {
            int n = _variables["n"];
            int i = _variables["i"];
            int j = _variables["j"];
            int l = _variables["l"];
            int r = _variables["r"];
            int m = _variables["m"];
            switch (_line)
            {
                case SortingStateLine.SS_Line1: // n = len(A)
                    // already done at initialisation
                    _nextLine = SortingStateLine.SS_Line3;
                    break;
                case SortingStateLine.SS_Line2: // Build heap:
                    break;
                case SortingStateLine.SS_Line3: // for i = len(A)/2+1 .. 0:
                    i--;
                    if (i < 0)
                    {
                        _nextLine = SortingStateLine.SS_Line6;
                        _nextValues = new Dictionary<string, int>(_variables);
                        _nextValues["i"] = n;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_Line4;
                    }
                    break;
                case SortingStateLine.SS_Line4: // heapify(i,n)
                    EnterSubroutineWithExitLine(SortingStateLine.SS_Line3);
                    _nextValues = new Dictionary<string, int>(_variables);
                    _nextValues["j"] = i;
                    _nextLine = SortingStateLine.SS_Line11;
                    break;
                case SortingStateLine.SS_Line5: // Iteratively take maximum:
                    break;
                case SortingStateLine.SS_Line6: // for i = len(A)-1 .. 1:
                    i--;
                    if (i < 1)
                    {
                        _nextLine = SortingStateLine.SS_None;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_Line7;
                    }
                    break;
                case SortingStateLine.SS_Line7: // swap(0,i)
                    _algorithm.Swap(0,i);
                    _nextLine = SortingStateLine.SS_Line8;
                    break;
                case SortingStateLine.SS_Line8: // n = n-1
                    n--;
                    _nextLine = SortingStateLine.SS_Line9;
                    break;
                case SortingStateLine.SS_Line9: // heapify(0,n)
                    EnterSubroutineWithExitLine(SortingStateLine.SS_Line6);
                    _nextValues = new Dictionary<string, int>(_variables);
                    _nextValues["j"] = 0;
                    _nextLine = SortingStateLine.SS_Line11;
                    break;
                case SortingStateLine.SS_Line10: // ""
                    break;
                case SortingStateLine.SS_Line11: // heapify(j, n):
                    _nextLine = SortingStateLine.SS_Line12;
                    break;
                case SortingStateLine.SS_Line12: // l = 2*j+1
                    l = 2 * j + 1;
                    _nextLine = SortingStateLine.SS_Line13;
                    break;
                case SortingStateLine.SS_Line13: // r = 2*j+2
                    r = 2 * j + 2;
                    _nextLine = SortingStateLine.SS_Line14;
                    break;
                case SortingStateLine.SS_Line14: // m = j
                    m = j;
                    _nextLine = SortingStateLine.SS_Line15;
                    break;
                case SortingStateLine.SS_Line15: // if l<n and A[l]>A[j]:
                    if (l >= n)
                    {
                        _nextLine = SortingStateLine.SS_Line17;
                        break;
                    }
                    if (_algorithm.CompareGreater(l, j))
                    {
                        _nextLine = SortingStateLine.SS_Line16;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_Line17;
                    }
                    break;
                case SortingStateLine.SS_Line16: // m = l
                    m = l;
                    _nextLine = SortingStateLine.SS_Line17;
                    break;
                case SortingStateLine.SS_Line17: // if r<n and A[r]>A[m]:
                    if (r >= n)
                    {
                        _nextLine = SortingStateLine.SS_Line19;
                        break;
                    }
                    if (_algorithm.CompareGreater(r, m))
                    {
                        _nextLine = SortingStateLine.SS_Line18;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_Line19;
                    }
                    break;
                case SortingStateLine.SS_Line18: // m = r
                    m = r;
                    _nextLine = SortingStateLine.SS_Line19;
                    break;
                case SortingStateLine.SS_Line19: // if m!=j:
                    if (m != j)
                    {
                        _nextLine = SortingStateLine.SS_Line20;
                    }
                    else
                    {
                        LeaveSubroutine();
                    }
                    break;
                case SortingStateLine.SS_Line20: // swap(j,m)
                    _algorithm.Swap(j,m);
                    _nextLine = SortingStateLine.SS_Line21;
                    break;
                case SortingStateLine.SS_Line21: // heapify(m,n)
                    EnterSubroutineWithExitLine(SortingStateLine.SS_None);
                    _nextValues = new Dictionary<string, int>(_variables);
                    _nextValues["j"] = m;
                    _nextLine = SortingStateLine.SS_Line11;
                    break;
                case SortingStateLine.SS_None:
                    break;
            }

            _variables["n"] = n;
            _variables["i"] = i;
            _variables["j"] = j;
            _variables["l"] = l;
            _variables["r"] = r;
            _variables["m"] = m;
            
        }
        
        public override void Undo()
        {
            int i = _variables["i"];
            int j = _variables["j"];
            int m = _variables["m"];
            int l = _variables["l"];
            int r = _variables["r"];
            switch (_line)
            {
                case SortingStateLine.SS_Line1: // n = <style=\"command\">len</style>(A)
                    break;
                case SortingStateLine.SS_Line2: // Build heap:
                    break;
                case SortingStateLine.SS_Line3: // for i = len(A)/2+1 .. 0:
                    break;
                case SortingStateLine.SS_Line4: // heapify(i,n)
                    break;
                case SortingStateLine.SS_Line5: // Iteratively take maximum:
                    break;
                case SortingStateLine.SS_Line6: // for i = len(A)-1 .. 1:
                    break;
                case SortingStateLine.SS_Line7: // swap(0,i)
                    _algorithm.UndoSwap(0,i);
                    break;
                case SortingStateLine.SS_Line8: // n = n-1
                    break;
                case SortingStateLine.SS_Line9: // heapify(0,n)
                    break;
                case SortingStateLine.SS_Line10: // ""
                    break;
                case SortingStateLine.SS_Line11: // heapify(j, n):
                    break;
                case SortingStateLine.SS_Line12: // l = 2*j+1
                    break;
                case SortingStateLine.SS_Line13: // r = 2*j+2
                    break;
                case SortingStateLine.SS_Line14: // m = j
                    break;
                case SortingStateLine.SS_Line15: // if l<n and A[l]>A[j]:
                    _algorithm.UndoGreater(l,j);
                    break;
                case SortingStateLine.SS_Line16: // m = l
                    break;
                case SortingStateLine.SS_Line17: // if r<n and A[r]>A[m]:
                    _algorithm.UndoGreater(r,m);
                    break;
                case SortingStateLine.SS_Line18: // m = r
                    break;
                case SortingStateLine.SS_Line19: // if m!=j:
                    break;
                case SortingStateLine.SS_Line20: // swap(j,m)
                    _algorithm.UndoSwap(j,m);
                    break;
                case SortingStateLine.SS_Line21: // heapify(m,n)
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
