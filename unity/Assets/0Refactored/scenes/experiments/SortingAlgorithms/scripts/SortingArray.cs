using System;
using System.Collections;
using System.Collections.Generic;
using GEAR.Localization;
using PlatformControls.PC;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class SortingArray : SortingVisualization, IResetObject
{
    [SerializeField] private GameObject arrayPlace;
    [SerializeField] private GameObject bucketPrefab;

    [SerializeField] private GameObject bucketsObject;
    
    [SerializeField] private Transform leftBorder;
    [SerializeField] private Transform rightBorder;
    
    [SerializeField] private TextMeshProUGUI pseudoCodeText;
    [SerializeField] private TextMeshProUGUI swapsText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [SerializeField] private PC_Slider sizeSlider;

    private Vector3 _placeOffset;
    private List<ArrayPlace> _elements = new List<ArrayPlace>();
    private List<SortingBucket> _buckets = new List<SortingBucket>();

    public override int Size
    {
        get => _size;
        set
        {
            _size = value;
            CreateDetailArray(_size);
            _sortingLogic.ResetAlgorithm();
        }
    }
    
    public void SetArraySize(float size)
    {
        Size = (int)size;
    }

    public override void Init(int size)
    {
        //ignore size!
        base.Init(size);
        CreateBuckets();
        HideBuckets();
        Size = (int)sizeSlider.value;
        _sortingLogic.Init();
    }

    public override void SortingFinished()
    {
        SimulationController.Instance.StopSimulation();
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

    private void CreateBuckets()
    {
        float padding = (rightBorder.position.x - leftBorder.position.x) / 10;
        Vector3 placeOffset = new Vector3(padding, 0, 0);
        Vector3 elementPosition = leftBorder.position + placeOffset / 2 - new Vector3(0, 0, 1);
        for(int i = 0; i < 10; i++)
        {
            var newBucket = Instantiate(bucketPrefab, bucketsObject.transform, false) as GameObject;
            var bucketComponent = newBucket.GetComponent<SortingBucket>();
            _buckets.Add(bucketComponent);
            bucketComponent.SetIndex(i);
            
            newBucket.transform.position = elementPosition;
            elementPosition += placeOffset;
        }
    }

    public override void HideBuckets()
    {
        bucketsObject.SetActive(false);
    }
    
    public override void ShowBuckets()
    {
        bucketsObject.SetActive(true);
    }

    public override void Insert(int fromIdx, int toIdx)
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
    
    public override void Swap(int fromIdx, int toIdx)
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

    public override bool CompareGreater(int idx1, int idx2)
    {
        StartCoroutine(WaitForMove());
        _elements[idx1].HighlightForSeconds(timePerMove);
        _elements[idx2].HighlightForSeconds(timePerMove);

        return _elements[idx1].Value > _elements[idx2].Value;
    }
    
    public override int GetMaxValue()
    {
        StartCoroutine(WaitForMove());
        int max = 0;
        int maxIndex = 0;
        foreach (var element in _elements)
        {
            if (element.Value > max)
            {
                max = element.Value;
                maxIndex = _elements.IndexOf(element);
            }
        }
        _elements[maxIndex].HighlightForSeconds(timePerMove);

        return max;
    }
    
    public override int GetBucketNumber(int ind, int exp)
    {
        StartCoroutine(WaitForMove());
        _elements[ind].HighlightForSeconds(timePerMove);

        int b = (_elements[ind].Value / exp) % 10;
        _buckets[b].HighlightForSeconds(timePerMove);
        
        return b;
    }
    
    public override void MoveToBucket(int from, int bucket)
    {
        StartCoroutine(WaitForMove());
        _elements[from].FadeOutSeconds(timePerMove);
        _buckets[bucket].HighlightForSeconds(timePerMove);
        
        _buckets[bucket].StoredElements.AddLast(_elements[from].Value);
    }
    
    public override void UndoMoveToBucket(int from, int bucket)
    {
        StartCoroutine(WaitForMove());

        _elements[from].Value = _buckets[bucket].StoredElements.Last.Value;
        _buckets[bucket].StoredElements.RemoveLast();
        
        _elements[from].FadeInSeconds(timePerMove);
        _buckets[bucket].HighlightForSeconds(timePerMove);
    }
    
    public override void MoveFromBucket(int to, int bucket)
    {
        StartCoroutine(WaitForMove());

        _elements[to].Value = _buckets[bucket].StoredElements.First.Value;
        _buckets[bucket].StoredElements.RemoveFirst();
        
        _elements[to].FadeInSeconds(timePerMove);
        _buckets[bucket].HighlightForSeconds(timePerMove);
    }
    
    public override void UndoMoveFromBucket(int to, int bucket)
    {
        StartCoroutine(WaitForMove());
        
        _elements[to].FadeOutSeconds(timePerMove);
        _buckets[bucket].HighlightForSeconds(timePerMove);

        _buckets[bucket].StoredElements.AddFirst(_elements[to].Value);
    }

    public override bool BucketEmpty(int bucket)
    {
        return _buckets[bucket].StoredElements.Count == 0;
    }

    public override void MarkCurrentSubset(int from, int to)
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
    
    public override void SetSwapsComparisons(int swaps, int comparisons)
    {
        string text = "";
        text += "<b>Swaps:</b> <pos=50%>" + swaps + "\n";
        text += "<b>Comparisons:</b> <pos=50%>" + comparisons;
        swapsText.text = text;
    }

    public override void DisplayPseudocode(List<string> pseudocode, int highlightLine, Dictionary<string, int> extraVars)
    {
        string highlightedCode = "";
        for (int i = 0; i < pseudocode.Count; ++i)
        {
            if (i == highlightLine)
            {
                highlightedCode += "<color=#810000>></color> " + pseudocode[i] + "\n";
            }
            else
            {
                highlightedCode += "  " + pseudocode[i] + "\n";
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
                                       "</style> : <style=\"sortingNumber\">" + GetElementValue(extraVar.Value) +
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
    
    public override void SetDescription(string key)
    {
        descriptionText.text = LanguageManager.Instance.GetString(key);
    }
    
    public override void DisplayIndices(Dictionary<string, int> indices)
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

    private int GetElementValue(int index)
    {
        return _elements[index].Value;
    }
    
    public void ResetObject()
    {
        foreach (var element in _elements)
        {
            element.Value = Random.Range(1, 100);
        }
        _sortingLogic.ResetAlgorithm();
    }
}
