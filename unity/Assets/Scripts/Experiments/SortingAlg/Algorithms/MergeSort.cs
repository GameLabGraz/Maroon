using System.Collections;
using System.Collections.Generic;
using OVR.OpenVR;
using UnityEngine;

public class MergeSort : SortingAlgorithm
{
    public override List<string> pseudocode
    {
        get => new List<string>()
        {
            "<style=\"header\">Merge Sort:</style>",
            "<style=\"function\">mergeSort</style>(i, j):",
            "    <style=\"command\">if</style> i<j:",
            "        k = (i+j)/<style=\"number\">2</style>",
            "        <style=\"function\">mergeSort</style>(i,k)",
            "        <style=\"function\">mergeSort</style>(k+<style=\"number\">1</style>,j)",
            "        <style=\"function\">merge</style>(i,k,j)",
            "",
            "<style=\"function\">merge</style>(i, k, j):",
            "    r = k+1",
            "    l = i",
            "    <style=\"command\">while</style> l<r <style=\"command\">and</style> r<=j:",
            "        <style=\"command\">if</style> A[r]<A[l]:",
            "            <style=\"function\">insert</style>(r,l)",
            "            r = r+<style=\"number\">1</style>",
            "        l = l+<style=\"number\">1</style>"
        };
    }
    
    public MergeSort(SortingLogic logic, int n) : base(logic)
    {
        _nextState = new MergeSortingState(this, n);
    }
    
    private class MergeSortingState : SortingState
    {
        public MergeSortingState(MergeSortingState old) : base(old) {}

        public MergeSortingState(MergeSort algorithm, int n): base(algorithm)
        {
            _variables.Add("i", 0);
            _variables.Add("j", n-1);
            _variables.Add("k", 0);
            _variables.Add("l", 0);
            _variables.Add("r", 0);
        }

        public override SortingState Next()
        {
            MergeSortingState next = new MergeSortingState(this);
            next._line = _nextLine;
            if (_nextValues != null)
            {
                next._variables = _nextValues;
            }
            return next;
        }
        
        public override SortingState Copy()
        {
            MergeSortingState copy = new MergeSortingState(this);
            return copy;
        }

        public override void Execute()
        {
            int i = _variables["i"];
            int j = _variables["j"];
            int k = _variables["k"];
            int l = _variables["l"];
            int r = _variables["r"];
            switch (_line)
            {
                case SortingStateLine.SS_Line1: // mergeSort(A, i, j):
                    _nextLine = SortingStateLine.SS_Line2;
                    break;
                case SortingStateLine.SS_Line2: // if i<j:
                    if (i < j)
                    {
                        _nextLine = SortingStateLine.SS_Line3;
                    }
                    else
                    {
                        leaveSubroutine();
                    }
                    break;
                case SortingStateLine.SS_Line3: // k = (i + j) // 2
                    k = (i + j) / 2;
                    _nextLine = SortingStateLine.SS_Line4;
                    break;
                case SortingStateLine.SS_Line4: // mergeSort(A,i,k)
                    enterSubroutineWithExitLine(SortingStateLine.SS_Line5);
                    _nextValues = new Dictionary<string, int>(_variables);
                    _nextValues["j"] = k;
                    _nextLine = SortingStateLine.SS_Line1;
                    break;
                case SortingStateLine.SS_Line5: // mergeSort(A,k+1,j)
                    enterSubroutineWithExitLine(SortingStateLine.SS_Line6);
                    _nextValues = new Dictionary<string, int>(_variables);
                    _nextValues["i"] = k+1;
                    _nextLine = SortingStateLine.SS_Line1;
                    break;
                case SortingStateLine.SS_Line6: // merge(A,i,k,j)
                    _nextLine = SortingStateLine.SS_Line8;
                    break;
                case SortingStateLine.SS_Line7: // 
                    break;
                case SortingStateLine.SS_Line8: // merge(A, i, k, j):
                    _nextLine = SortingStateLine.SS_Line9;
                    break;
                case SortingStateLine.SS_Line9: // r = k+1
                    r = k+1;
                    _nextLine = SortingStateLine.SS_Line10;
                    break;
                case SortingStateLine.SS_Line10: // l = i
                    l = i;
                    _nextLine = SortingStateLine.SS_Line11;
                    break;
                case SortingStateLine.SS_Line11: // while l < r and r <= j:
                    if (l < r && r <= j)
                    {
                        _nextLine = SortingStateLine.SS_Line12;
                    }
                    else
                    {
                        leaveSubroutine();
                    }
                    break;
                case SortingStateLine.SS_Line12: // if A[r] < A[l]:
                    if (_algorithm.CompareGreater(l, r))
                    {
                        _nextLine = SortingStateLine.SS_Line13;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_Line15;
                    }
                    _requireWait = true;
                    break;
                case SortingStateLine.SS_Line13: // insert(A,r,l)
                    _algorithm.Insert(r,l);
                    _nextLine = SortingStateLine.SS_Line14;
                    _requireWait = true;
                    break;
                case SortingStateLine.SS_Line14: // r += 1
                    r++;
                    _nextLine = SortingStateLine.SS_Line15;
                    break;
                case SortingStateLine.SS_Line15: // l += 1
                    l++;
                    _nextLine = SortingStateLine.SS_Line11;
                    break;
                case SortingStateLine.SS_None:
                    break;
            }

            _variables["i"] = i;
            _variables["j"] = j;
            _variables["k"] = k;
            _variables["l"] = l;
            _variables["r"] = r;
        }

        public override void Undo()
        {
            _requireWait = false;
            int i = _variables["i"];
            int j = _variables["j"];
            int l = _variables["l"];
            int r = _variables["r"];
            
            switch (_line)
            {
                case SortingStateLine.SS_Line1: // mergeSort(A, i, j):
                    break;
                case SortingStateLine.SS_Line2: // if i<j:
                    break;
                case SortingStateLine.SS_Line3: // k = (i + j) // 2
                    break;
                case SortingStateLine.SS_Line4: // mergeSort(A,i,k)
                    break;
                case SortingStateLine.SS_Line5: // mergeSort(A,k+1,j)
                    break;
                case SortingStateLine.SS_Line6: // merge(A,i,k,j)
                    break;
                case SortingStateLine.SS_Line7: // 
                    break;
                case SortingStateLine.SS_Line8: // merge(A, i, k, j):
                    break;
                case SortingStateLine.SS_Line9: // r = k+1
                    break;
                case SortingStateLine.SS_Line10: // l = i
                    break;
                case SortingStateLine.SS_Line11: // while l < r and r <= j:
                    break;
                case SortingStateLine.SS_Line12: // if A[r] < A[l]:
                    break;
                case SortingStateLine.SS_Line13: // insert(A,r,l)
                    _algorithm.UndoInsert(r,l);
                    _requireWait = true;
                    break;
                case SortingStateLine.SS_Line14: // r += 1
                    break;
                case SortingStateLine.SS_Line15: // l += 1
                    break;
                case SortingStateLine.SS_None:
                    break;
            }
        }

        public override int GetSubsetStart()
        {
            return _variables["i"];
        }
        
        public override int GetSubsetEnd()
        {
            return _variables["j"];
        }
    }
}
