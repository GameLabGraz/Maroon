using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SortingLogic : MonoBehaviour
{
    public enum SortingAlgorithmType
    {
        SA_InsertionSort,
        SA_MergeSort,
        SA_HeapSort,
        SA_QuickSort,
        SA_SelectionSort,
        SA_BubbleSort,
        SA_GnomeSort,
        SA_RadixSort,
        SA_ShellSort
    }

    [SerializeField] private TextMeshProUGUI pseudoCodeText;
    [SerializeField] private TextMeshProUGUI swapsText;
    
    private SortingAlgorithmType _sortingAlgorithm;
    private SortingAlgorithm _algorithm;

    private SortingArray _sortingArray;
    
    private bool _waitForMove;

    private void Start()
    {
        _sortingArray = GetComponent<SortingArray>();
        _sortingArray.Size = 10; //TODO
        
        //TODO
        _sortingAlgorithm = SortingAlgorithmType.SA_QuickSort;
        SetAlgorithm();
    }

    private void Update()
    {
        if (SimulationController.Instance.SimulationRunning && !_waitForMove)
        {
            while (_waitForMove == false && SimulationController.Instance.SimulationRunning)
            {
                _waitForMove = true;
                _algorithm.ExecuteNextState();
            }
        }
    }

    #region GUI_Functions
    
    public void StepForward()
    {
        if (_waitForMove)
            return;
        _waitForMove = true;
        _algorithm.ExecuteNextState();
    }
    
    public void StepBackward()
    {
        if (_waitForMove)
            return;
        _waitForMove = true;
        _algorithm.ExecutePreviousState();
    }

    public void SetAlgorithmDropdown(int choice)
    {
        _sortingAlgorithm = (SortingAlgorithmType)choice;
        SetAlgorithm();
    }

    public void SetArraySize(float size)
    {
        _sortingArray.Size = (int)size;
        SetAlgorithm();
    }
    
    #endregion

    private void SetAlgorithm()
    {
        switch (_sortingAlgorithm)
        {
            case SortingAlgorithmType.SA_InsertionSort:
                _algorithm = new InsertionSort(this, _sortingArray.Size);
                break;
            case SortingAlgorithmType.SA_MergeSort:
                _algorithm = new MergeSort(this, _sortingArray.Size);
                break;
            case SortingAlgorithmType.SA_HeapSort:
                _algorithm = new HeapSort(this, _sortingArray.Size);
                break;
            case SortingAlgorithmType.SA_QuickSort:
                _algorithm = new QuickSort(this, _sortingArray.Size);
                break;
            case SortingAlgorithmType.SA_SelectionSort:
                _algorithm = new SelectionSort(this, _sortingArray.Size);
                break;
            case SortingAlgorithmType.SA_BubbleSort:
                //TODO _algorithm = new BubbleSort(this, arraySize);
                break;
            case SortingAlgorithmType.SA_GnomeSort:
                _algorithm = new GnomeSort(this, _sortingArray.Size);
                break;
            case SortingAlgorithmType.SA_RadixSort:
                //TODO _algorithm = new RadixSort(this, arraySize);
                break;
            case SortingAlgorithmType.SA_ShellSort:
                _algorithm = new ShellSort(this, _sortingArray.Size);
                break;
            default:
                //TODO: No algorithm selected
                break;
        }
        
        SetPseudocode(-1, null);
        SetSwapsComparisons(0,0);
        DisplayIndices(new Dictionary<string, int>());
    }

    public void MoveFinished()
    {
        _waitForMove = false;
    }

    public void SortingFinished()
    {
        SimulationController.Instance.StopSimulation();
        MarkCurrentSubset(0 , _sortingArray.Size - 1);
    }

    public void Insert(int fromIdx, int toIdx)
    {
        if (fromIdx < 0 || fromIdx >= _sortingArray.Size ||
            toIdx < 0 || toIdx >= _sortingArray.Size ||
            fromIdx == toIdx)
        {
            MoveFinished();
            return;
        }
        _sortingArray.Insert(fromIdx, toIdx);
    }
    
    public void Swap(int fromIdx, int toIdx)
    {
        if (fromIdx < 0 || fromIdx >= _sortingArray.Size ||
            toIdx < 0 || toIdx >= _sortingArray.Size ||
            fromIdx == toIdx)
        {
            MoveFinished();
            return;
        }
        _sortingArray.Swap(fromIdx, toIdx);
    }

    public bool CompareGreater(int idx1, int idx2)
    {
        if (idx1 < 0 || idx1 >= _sortingArray.Size ||
            idx2 < 0 || idx2 >= _sortingArray.Size ||
            idx1 == idx2)
        {
            MoveFinished();
            return false;
        }
        return _sortingArray.CompareGreater(idx1, idx2);
    }

    public void SetPseudocode(int highlightLine, Dictionary<string, int> extraVars)
    {
        string highlightedCode = "";
        for (int i = 0; i < _algorithm.Pseudocode.Count; ++i)
        {
            if (i == highlightLine)
            {
                highlightedCode += "<color=#810000>></color> " + _algorithm.Pseudocode[i] + "\n";
            }
            else
            {
                highlightedCode += "  " + _algorithm.Pseudocode[i] + "\n";
            }
        }

        if (extraVars != null)
        {
            foreach (var extraVar in extraVars)
            {
                if (extraVar.Value == -1)
                    break;
                if (extraVar.Key == "pInd")
                {
                    highlightedCode += "\n<style=\"sortingFunction\">" + "p" +
                                       "</style> : <style=\"sortingNumber\">" + _sortingArray.GetElementValue(extraVar.Value) +
                                       "</style>";
                }
                else
                {
                    highlightedCode += "\n<style=\"sortingFunction\">" + extraVar.Key +
                                       "</style> : <style=\"sortingNumber\">" + extraVar.Value +
                                       "</style>";
                }
            }
        }

        pseudoCodeText.text = highlightedCode;
    }

    public void MarkCurrentSubset(int from, int to)
    {
        if (from < 0 || from >= _sortingArray.Size ||
            to < 0 || to >= _sortingArray.Size)
        {
            return;
        }
        _sortingArray.MarkCurrentSubset(from, to);
    }

    public void SetSwapsComparisons(int swaps, int comparisons)
    {
        string text = "";
        text += "<b>Swaps:</b> <pos=50%>" + swaps + "\n";
        text += "<b>Comparisons:</b> <pos=50%>" + comparisons;
        swapsText.text = text;
    }
    
    public void DisplayIndices(Dictionary<string, int> indices)
    {
        _sortingArray.DisplayIndices(indices);
    }
}
