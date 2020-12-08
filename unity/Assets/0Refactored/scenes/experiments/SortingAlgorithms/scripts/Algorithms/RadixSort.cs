using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadixSort : SortingAlgorithm
{
    public override List<string> Pseudocode
    {
        get => new List<string>()
        {
            "<style=\"sortingTitle\">Radix Sort:</style>",
            "key = <style=\"sortingFunction\">max</style>(A)",
            "exp = <style=\"sortingNumber\">1</style>",
            "<style=\"sortingKeyword\">while</style> key / exp > <style=\"sortingNumber\">1</style>:",
            "    <style=\"sortingFunction\">countingSort</style>(exp)",
            "    exp = exp * <style=\"sortingNumber\">10</style>",
            "",
            "<style=\"sortingFunction\">countingSort</style>(exp):",
            "    <style=\"sortingKeyword\">for</style> i = <style=\"sortingNumber\">0</style> .. <style=\"sortingKeyword\">len</style>(A)-<style=\"sortingNumber\">1</style>:",
            "        b = (A[i]//exp)%<style=\"sortingNumber\">10</style>",
            "        <style=\"sortingFunction\">moveToBucket</style>(i,b)",
            "    i = <style=\"sortingNumber\">0</style>",
            "    <style=\"sortingKeyword\">for</style> b = <style=\"sortingNumber\">0</style> .. <style=\"sortingNumber\">10</style>:",
            "        <style=\"sortingKeyword\">if</style> bucket[b] <style=\"sortingKeyword\">not</style> empty:",
            "            <style=\"sortingFunction\">moveFromBucket</style>(i,b)",
            "            i = i + <style=\"sortingNumber\">1</style>"
        };
    }
    
    public RadixSort(SortingLogic logic, int n, bool battleMode) : base(logic, battleMode)
    {
        _executedStates.AddFirst(new RadixSortingState(this, n));
    }

    private class RadixSortingState : SortingState
    {
        public RadixSortingState(RadixSortingState old) : base(old) {}

        public RadixSortingState(RadixSort algorithm, int n): base(algorithm)
        {
            _variables.Add("n", n);
            _variables.Add("exp", -1);
            _variables.Add("key", -1);
            _variables.Add("i", -1);
            _variables.Add("b", -1);
        }

        public override SortingState Next()
        {
            RadixSortingState next = new RadixSortingState(this);
            return InitializeNext(next);
        }
        
        public override SortingState Copy()
        {
            RadixSortingState copy = new RadixSortingState(this);
            return copy;
        }

        public override void Execute()
        {
            int n = _variables["n"];
            int exp = _variables["exp"];
            int key = _variables["key"];
            int i = _variables["i"];
            int b = _variables["b"];
            switch (_line)
            {
                case SortingStateLine.SS_Line1: // key = max(A)
                    key = _algorithm.GetMaxValue();
                    _nextLine = SortingStateLine.SS_Line2;
                    break;
                case SortingStateLine.SS_Line2: // exp = 1
                    exp = 1;
                    _nextLine = SortingStateLine.SS_Line3;
                    break;
                case SortingStateLine.SS_Line3: // while key / exp > 1
                    if (key / exp > 1)
                    {
                        _nextLine = SortingStateLine.SS_Line4;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_None;
                    }
                    break;
                case SortingStateLine.SS_Line4: // countingSort(exp)
                    EnterSubroutineWithExitLine(SortingStateLine.SS_Line5);
                    _nextLine = SortingStateLine.SS_Line7;
                    break;
                case SortingStateLine.SS_Line5: // exp = exp * 10
                    exp *= 10;
                    _nextLine = SortingStateLine.SS_Line3;
                    break;
                case SortingStateLine.SS_Line6: // 
                    break;
                case SortingStateLine.SS_Line7: // countingSort(exp):
                    _nextLine = SortingStateLine.SS_Line8;
                    _nextValues = new Dictionary<string, int>(_variables);
                    _nextValues["i"] = -1;
                    break;
                case SortingStateLine.SS_Line8: // for i = 0 .. len(A)-1
                    i++;
                    
                    if (i < n)
                    {
                        _nextLine = SortingStateLine.SS_Line9;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_Line11;
                    }
                    break;
                case SortingStateLine.SS_Line9: // b = (A[i]//exp)%10
                    b = _algorithm.GetBucketNumber(i, exp);
                    _nextLine = SortingStateLine.SS_Line10;
                    break;
                case SortingStateLine.SS_Line10: // moveToBucket(i,b)
                    _algorithm.MoveToBucket(i, b);
                    _nextLine = SortingStateLine.SS_Line8;
                    break;
                case SortingStateLine.SS_Line11: // i = 0
                    i = 0;
                    _nextValues = new Dictionary<string, int>(_variables);
                    _nextValues["b"] = -1;
                    _nextValues["i"] = 0;
                    _nextLine = SortingStateLine.SS_Line12;
                    break;
                case SortingStateLine.SS_Line12: // for b = 0 .. 10
                    b++;

                    if (b < 10)
                    {
                        _nextLine = SortingStateLine.SS_Line13;
                    }
                    else
                    {
                        LeaveSubroutine();
                    }
                    break;
                case SortingStateLine.SS_Line13: // if bucket[b] not empty:
                    if (_algorithm.BucketEmpty(b))
                    {
                        _nextLine = SortingStateLine.SS_Line12;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_Line14;
                    }
                    break;
                case SortingStateLine.SS_Line14: // moveFromBucket(i,b)
                    _algorithm.MoveFromBucket(i, b);
                    _nextLine = SortingStateLine.SS_Line15;
                    break;
                case SortingStateLine.SS_Line15: // i = i + 1
                    i++;
                    _nextLine = SortingStateLine.SS_Line13;
                    break;
                case SortingStateLine.SS_None:
                    break;
            }

            _variables["n"] = n;
            _variables["exp"] = exp;
            _variables["key"] = key;
            _variables["i"] = i;
            _variables["b"] = b;
        }
        
        public override void Undo()
        {
            int exp = _variables["exp"];
            int i = _variables["i"];
            int b = _variables["b"];
            switch (_line)
            {
                case SortingStateLine.SS_Line1: // key = max(A)
                    _algorithm.UndoGetMaxValue();
                    break;
                case SortingStateLine.SS_Line2: // exp = 1
                    break;
                case SortingStateLine.SS_Line3: // while key / exp > 1
                    break;
                case SortingStateLine.SS_Line4: // countingSort(exp)
                    break;
                case SortingStateLine.SS_Line5: // exp = exp * 10
                    break;
                case SortingStateLine.SS_Line6: // 
                    break;
                case SortingStateLine.SS_Line7: // countingSort(exp):
                    break;
                case SortingStateLine.SS_Line8: // for i = 0 .. len(A)-1
                    break;
                case SortingStateLine.SS_Line9: // b = (A[i]//exp)%10
                    _algorithm.UndoGetBucketNumber(i, exp);
                    break;
                case SortingStateLine.SS_Line10: // moveToBucket(i,b)
                    _algorithm.UndoMoveToBucket(i, b);
                    break;
                case SortingStateLine.SS_Line11: // i = 0
                    break;
                case SortingStateLine.SS_Line12: // for b = 0 .. 10
                    break;
                case SortingStateLine.SS_Line13: // if bucket[b] not empty:
                    break;
                case SortingStateLine.SS_Line14: // moveFromBucket(i,b)
                    _algorithm.UndoMoveFromBucket(i, b);
                    break;
                case SortingStateLine.SS_Line15: // i = i + 1
                    break;
                case SortingStateLine.SS_None:
                    break;
            }
        }
        
        public override Dictionary<string, int> GetIndexVariables()
        {
            var indexVariables = new Dictionary<string, int>(_variables);
            indexVariables.Remove("n");
            indexVariables.Remove("exp");
            indexVariables.Remove("key");
            indexVariables.Remove("b");
            return indexVariables;
        }

        public override Dictionary<string, int> GetExtraVariables()
        {
            var extraVariables = new Dictionary<string, int>();
            extraVariables.Add("key", _variables["key"]);
            extraVariables.Add("exp", _variables["exp"]);
            extraVariables.Add("b", _variables["b"]);
            return extraVariables;
        }
    }
}
