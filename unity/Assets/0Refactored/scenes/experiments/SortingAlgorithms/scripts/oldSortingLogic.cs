using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class oldSortingLogic : MonoBehaviour
{
    public enum SortingAlgorithmType
    {
        SA_None,
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
    
    [Header("Sorting Machine Settings")] 
    public SortingMachine sortingMachine;

    public SortingAlgorithmType sortingAlgorithm;
    private SortingAlgorithm _algorithm;
    private bool _currentlySorting;
    private bool _waitForMachine;

    [Header("Display")] 
    public TextMeshPro displayText;
    
    [Header("Array Settings")] 
    public oldArrayPlace referencePlace;
    [Range(0,10)]
    public int arraySize = 10;

    [Header("Debugging Variables")]
    [Range(0, 9)] public int moveFrom = 0;
    [Range(0, 9)] public int moveTo = 0;
    public bool move = false;
    public bool swap = false;
    
    private List<oldArrayPlace> _arrayPlaces = new List<oldArrayPlace>();

    public List<oldArrayPlace> ArrayPlaces
    {
        get => _arrayPlaces;
    }
    
    
    private int currentSize = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        if (referencePlace != null)
            _arrayPlaces.Add(referencePlace);
        CreateArray(arraySize);
        
        SetAlgorithm();
        
        _currentlySorting = true;
        setPseudocode(-1);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentSize != arraySize)
            CreateArray(arraySize);

        if (!_waitForMachine && _currentlySorting)
        {
            _waitForMachine = true;
            _algorithm.ExecuteNextState();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            _currentlySorting = !_currentlySorting;
        }
        
        //TODO: Just for debugging, should be done by controller
        if (!_waitForMachine && Input.GetKeyDown(KeyCode.RightArrow))
        {
            _waitForMachine = true;
            _algorithm.ExecuteNextState();
        }
        if (!_waitForMachine && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _waitForMachine = true;
            _algorithm.ExecutePreviousState();
        }
        
        if (move) {
            Insert(moveFrom, moveTo);
        }

        if (swap) {
            Swap(moveFrom, moveTo);
        }
    }

    private void SetAlgorithm()
    {
        switch (sortingAlgorithm)
        {
            /*case SortingAlgorithmType.SA_InsertionSort:
                _algorithm = new InsertionSort(this, arraySize);
                break;
            case SortingAlgorithmType.SA_MergeSort:
                _algorithm = new MergeSort(this, arraySize);
                break;
            case SortingAlgorithmType.SA_HeapSort:
                _algorithm = new HeapSort(this, arraySize);
                break;
            case SortingAlgorithmType.SA_QuickSort:
                _algorithm = new QuickSort(this, arraySize);
                break;
            case SortingAlgorithmType.SA_SelectionSort:
                _algorithm = new SelectionSort(this, arraySize);
                break;
            case SortingAlgorithmType.SA_BubbleSort:
                //TODO _algorithm = new BubbleSort(this, arraySize);
                break;
            case SortingAlgorithmType.SA_GnomeSort:
                _algorithm = new GnomeSort(this, arraySize);
                break;
            case SortingAlgorithmType.SA_RadixSort:
                //TODO _algorithm = new RadixSort(this, arraySize);
                break;
            case SortingAlgorithmType.SA_ShellSort:
                _algorithm = new ShellSort(this, arraySize);
                break;
            default:
                //TODO: No algorithm selected
                break;*/
        }
    }

    public void CreateArray(int newSize)
    {
        // + is going to left, - is going to right -> z-axis!
        var currentPos = Vector3.zero;
        var neededWidth = newSize * referencePlace.width;
        currentPos.z += (neededWidth / 2f) - referencePlace.width / 2f;
        
        for (var i = 0; i < _arrayPlaces.Count; ++i)
        {
            var isActive = i < newSize;
            _arrayPlaces[i].gameObject.SetActive(isActive);
            if (!isActive) continue;
            
            //Set Position
            _arrayPlaces[i].gameObject.transform.localPosition = currentPos;
            _arrayPlaces[i].Index = i;
            currentPos.z -= referencePlace.width;
        }

        while (_arrayPlaces.Count < newSize)
        {
            var newPlace = Instantiate(referencePlace.gameObject, referencePlace.transform.parent);
            
            newPlace.SetActive(true);
            newPlace.transform.localPosition = currentPos;
            var place = newPlace.GetComponent<oldArrayPlace>();
            place.Index = _arrayPlaces.Count;
            currentPos.z -= referencePlace.width;
            
            _arrayPlaces.Add(place);
        }

        currentSize = arraySize;
    }

    public void Insert(int fromIdx, int toIdx)
    {
        if (fromIdx < 0 || fromIdx >= _arrayPlaces.Count || !_arrayPlaces[fromIdx].isActiveAndEnabled ||
            toIdx < 0 || toIdx >= _arrayPlaces.Count || !_arrayPlaces[toIdx].isActiveAndEnabled ||
            fromIdx == toIdx)
        {
            _waitForMachine = false;
            return;
        }
        
        sortingMachine.Insert(fromIdx, toIdx);
        move = false;
    }

    public void Swap(int idx1, int idx2)
    {
        if (idx1 < 0 || idx1 >= _arrayPlaces.Count || !_arrayPlaces[idx1].isActiveAndEnabled ||
            idx2 < 0 || idx2 >= _arrayPlaces.Count || !_arrayPlaces[idx2].isActiveAndEnabled ||
            idx1 == idx2)
        {
            _waitForMachine = false;
            return;
        }

        sortingMachine.Swap(idx1, idx2);
        swap = false;
    }
    
    public bool CompareGreater(int idx1, int idx2)
    {
        if (idx1 < 0 || idx1 >= _arrayPlaces.Count || !_arrayPlaces[idx1].isActiveAndEnabled ||
            idx2 < 0 || idx2 >= _arrayPlaces.Count || !_arrayPlaces[idx2].isActiveAndEnabled ||
            idx1 == idx2)
        {
            _waitForMachine = false;
            return false;
        }

        sortingMachine.Compare(idx1, idx2);
        return _arrayPlaces[idx1].GetSortElementValue() > _arrayPlaces[idx2].GetSortElementValue();
    }
    
    public void MoveFinished()
    {
        _waitForMachine = false;
    }

    public void sortingFinished()
    {
        _currentlySorting = false;
        markCurrentSubset(-1,-1);
    }

    public void RearrangeArrayElements(float speed)
    {
        for (var i = 0; i < ArrayPlaces.Count - 1; ++i)
        {
            if (ArrayPlaces[i].sortElement != null) continue;
            
            ArrayPlaces[i].SetSortElement(_arrayPlaces[i+1].sortElement, speed);
            _arrayPlaces[i + 1].sortElement = null;
        }
    }

    public void MakePlaceInArray(int index, float speed)
    {
        for (var i = ArrayPlaces.Count - 1; i > index; --i)
        {
            if(ArrayPlaces[i].sortElement != null) continue;
            
            ArrayPlaces[i].SetSortElement(_arrayPlaces[i-1].sortElement, speed);
            ArrayPlaces[i - 1].sortElement = null;
        }
    }
    
    public void setSwapsOperations(int swaps, int operations)
    {
        sortingMachine.setSwapsOperations(swaps, operations);
    }

    public void setPseudocode(int highlightLine)
    {
        //TODO: Maybe we can find a nicer highlight?
        string highlightedCode = "";
        for (int i = 0; i < _algorithm.pseudocode.Count; ++i)
        {
            if (i == highlightLine)
            {
                highlightedCode += "<mark=#a8eb0055>" + _algorithm.pseudocode[i] + "\n" + "</mark>";
            }
            else
            {
                highlightedCode += _algorithm.pseudocode[i] + "\n";
            }
        }
        displayText.text = highlightedCode;
    }

    public void markCurrentSubset(int from, int to) // exclude to
    {
        for (int i = 0; i < ArrayPlaces.Count; ++i)
        {
            if (i >= from && i <= to)
            {
                ArrayPlaces[i].sortElement.GetComponent<oldSortingElement>().markActiveColor();
            }
            else
            {
                ArrayPlaces[i].sortElement.GetComponent<oldSortingElement>().resetToDefaultColor();
            }
        }
    }

    public void markPivot(int pivot)
    {
        if (pivot != -1)
        {
            ArrayPlaces[pivot].sortElement.GetComponent<oldSortingElement>().SetPivotColor();
        }
    }

    public void displayIndices(Dictionary<string, int> indices)
    {
        for (int i = 0; i < _arrayPlaces.Count; ++i)
        {
            List<string> matches = new List<string>();
            foreach (var pair in indices)
            {
                if (pair.Value == i)
                {
                    matches.Add("<b><color=#006666>" + pair.Key + "</color></b>");
                }
            }
            _arrayPlaces[i].UpdateIndex(string.Join(",", matches));
        }
    }
}
