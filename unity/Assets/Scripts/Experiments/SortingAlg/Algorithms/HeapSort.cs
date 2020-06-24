using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeapSort : SortingAlgorithm
{
    public override List<string> pseudocode
    {
        get => new List<string>()
        {
            "<style=\"header\">Heap Sort:</style>",
            "n = <style=\"command\">len</style>(A)",
            "<i><color=#005500>//Build heap:</color></i>",
            "<style=\"command\">for</style> i = <style=\"command\">len</style>(A)/<style=\"number\">2</style>+<style=\"number\">1</style> .. <style=\"number\">0</style>:",
            "    <style=\"function\">heapify</style>(i,n)",
            "<i><color=#005500>//Iteratively take maximum:</color></i>",
            "<style=\"command\">for</style> i = <style=\"command\">len</style>(A)-<style=\"number\">1</style> .. <style=\"number\">1</style>:",
            "    <style=\"function\">swap</style>(0,i)",
            "    n = n-<style=\"number\">1</style>",
            "    <style=\"function\">heapify</style>(0,n)",
            "",
            "<style=\"function\">heapify</style>(j, n):",
            "    l = <style=\"number\">2</style>*j+<style=\"number\">1</style>",
            "    r = <style=\"number\">2</style>*j+<style=\"number\">2</style>",
            "    m = j",
            "    <style=\"command\">if</style> l<n <style=\"command\">and</style> A[l]>A[j]:",
            "        m = l",
            "    <style=\"command\">if</style> r<n <style=\"command\">and</style> A[r]>A[m]:",
            "        m = r",
            "    <style=\"command\">if</style> m!=j:",
            "        <style=\"function\">swap</style>(j,m)",
            "        <style=\"function\">heapify</style>(m,n)"
        };
    }
    
    public HeapSort(SortingLogic logic, int n) : base(logic)
    {
        _nextState = new HeapSortingState(this, n);
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
            next._line = _nextLine;
            if (_nextValues != null)
            {
                next._variables = _nextValues;
            }
            return next;
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
                case SortingStateLine.SS_Line1: // n = <style=\"command\">len</style>(A)
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
                    enterSubroutineWithExitLine(SortingStateLine.SS_Line3);
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
                    _requireWait = true;
                    _algorithm.Swap(0,i);
                    _nextLine = SortingStateLine.SS_Line8;
                    break;
                case SortingStateLine.SS_Line8: // n = n-1
                    n--;
                    _nextLine = SortingStateLine.SS_Line9;
                    break;
                case SortingStateLine.SS_Line9: // heapify(0,n)
                    enterSubroutineWithExitLine(SortingStateLine.SS_Line6);
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

                    _requireWait = true;
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

                    _requireWait = true;
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
                        leaveSubroutine();
                    }
                    break;
                case SortingStateLine.SS_Line20: // swap(j,m)
                    _requireWait = true;
                    _algorithm.Swap(j,m);
                    _nextLine = SortingStateLine.SS_Line21;
                    break;
                case SortingStateLine.SS_Line21: // heapify(m,n)
                    enterSubroutineWithExitLine(SortingStateLine.SS_None);
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
            _requireWait = false;
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
                    _requireWait = true;
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
                    break;
                case SortingStateLine.SS_Line16: // m = l
                    break;
                case SortingStateLine.SS_Line17: // if r<n and A[r]>A[m]:
                    break;
                case SortingStateLine.SS_Line18: // m = r
                    break;
                case SortingStateLine.SS_Line19: // if m!=j:
                    break;
                case SortingStateLine.SS_Line20: // swap(j,m)
                    _requireWait = true;
                    _algorithm.UndoSwap(j,m);
                    break;
                case SortingStateLine.SS_Line21: // heapify(m,n)
                    break;
                case SortingStateLine.SS_None:
                    break;
            }
        }
    }
}
