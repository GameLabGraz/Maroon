using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SortingArray : MonoBehaviour
{
    [SerializeField] private GameObject arrayPlace;
    
    [SerializeField] private Transform leftBorder;
    [SerializeField] private Transform rightBorder;

    private int _size;
    private Vector3 _placeOffset;
    private List<ArrayPlace> _elements = new List<ArrayPlace>();

    [SerializeField] private float timePerMove;

    private SortingLogic _sortingLogic;

    public int Size
    {
        get => _size;
        set
        {
            _size = value;
            CreateDetailArray(_size);
        }
    }

    private void Start()
    {
        Size = 10;
        _sortingLogic = GetComponent<SortingLogic>();
    }

    private void CreateDetailArray(int size)
    {
        float padding = (rightBorder.position.x - leftBorder.position.x) / size;
        _placeOffset = new Vector3(padding, 0, 0);
        Vector3 elementPosition = leftBorder.position + _placeOffset / 2;
        for(int i = 0; i < size; i++)
        {
            if (i >= _elements.Count)
            {
                var newArrayPlace = Instantiate(arrayPlace, leftBorder.parent, false) as GameObject;

                var arrayPlaceComponent = newArrayPlace.GetComponent<ArrayPlace>();
                arrayPlaceComponent.SetBaseNumber(i);
                _elements.Add(arrayPlaceComponent);
                
                arrayPlaceComponent.Value = Random.Range(1, 100);
            }
            _elements[i].transform.position = elementPosition;
            elementPosition += _placeOffset;
        }

        while (_elements.Count > size)
        {
            var lastElement = _elements[_elements.Count - 1];
            Destroy(lastElement.gameObject);
            _elements.Remove(lastElement);
        }
    }
    
    public void Insert(int fromIdx, int toIdx)
    {
        StartCoroutine(InsertCoroutine(fromIdx, toIdx));
    }

    private IEnumerator InsertCoroutine(int fromIdx, int toIdx)
    {
        _elements[fromIdx].FadeOutSeconds(timePerMove / 2);

        int i = toIdx;
        if (fromIdx > toIdx)
        {
            for (i = toIdx; i < fromIdx; i++)
            {
                _elements[i].MoveOutRight(timePerMove / 2, _placeOffset / 2);
            }
        }
        else
        {
            for (i = toIdx; i > fromIdx; i--)
            {
                _elements[i].MoveOutLeft(timePerMove / 2, _placeOffset / 2);
            }
        }
         
        yield return new WaitForSeconds(timePerMove / 2);

        int fromValue = _elements[fromIdx].Value;
        i = fromIdx;
        if (fromIdx > toIdx)
        {
             while (i > toIdx)
             {
                 _elements[i].Value = _elements[i-1].Value;
                 i--;
             }

             _elements[toIdx].Value = fromValue;
        }
        else
        {
            while (i < toIdx)
            {
                _elements[i].Value = _elements[i+1].Value;
                i++;
            }

            _elements[toIdx].Value = fromValue;
        }

        _elements[toIdx].FadeInSeconds(timePerMove / 2);
        
        if (fromIdx > toIdx)
        {
            for (i = toIdx + 1; i <= fromIdx; i++)
            {
                _elements[i].MoveInLeft(timePerMove / 2, _placeOffset / 2);
            }
        }
        else
        {
            for (i = toIdx - 1; i >= fromIdx; i--)
            {
                _elements[i].MoveInRight(timePerMove / 2, _placeOffset / 2);
            }
        }

        yield return new WaitForSeconds(timePerMove / 2);

        _sortingLogic.MoveFinished();
    }
    
    public void Swap(int fromIdx, int toIdx)
    {
        StartCoroutine(SwapCoroutine(fromIdx, toIdx));
    }

    private IEnumerator SwapCoroutine(int fromIdx, int toIdx)
    {
        _elements[fromIdx].FadeOutSeconds(timePerMove / 2);
        _elements[toIdx].FadeOutSeconds(timePerMove / 2);
        
        yield return new WaitForSeconds(timePerMove / 2);
        
        int fromValue = _elements[fromIdx].Value;
        _elements[fromIdx].Value = _elements[toIdx].Value;
        _elements[toIdx].Value = fromValue;
        
        _elements[fromIdx].FadeInSeconds(timePerMove / 2);
        _elements[toIdx].FadeInSeconds(timePerMove / 2);
        
        yield return new WaitForSeconds(timePerMove / 2);
        
        _sortingLogic.MoveFinished();
    }

    public bool CompareGreater(int idx1, int idx2)
    {
        StartCoroutine(WaitForMove());
        _elements[idx1].HighlightForSeconds(timePerMove);
        _elements[idx2].HighlightForSeconds(timePerMove);
        if (_elements[idx1].Value > _elements[idx2].Value)
        {
            return true;
        }

        return false;
    }

    public void MarkCurrentSubset(int from, int to)
    {
        for (int i = 0; i < _size; ++i)
        {
            if (i >= from && i <= to)
            {
                _elements[i].MarkAsSubset();
            }
            else
            {
                _elements[i].MarkAsNotSubset();
            }
        }
    }

    public void SetSwapsOperations(int swaps, int operations)
    {
        //TODO
    }
    
    public void DisplayIndices(Dictionary<string, int> indices)
    {
        for (int i = 0; i < Size; ++i)
        {
            List<string> matches = new List<string>();
            foreach (var pair in indices)
            {
                if (pair.Value == i)
                {
                    matches.Add(pair.Key);
                }
            }
            _elements[i].SetIndexText(matches);
        }
    }

    private IEnumerator WaitForMove()
    {
        yield return new WaitForSeconds(timePerMove);
        _sortingLogic.MoveFinished();
    }
}
