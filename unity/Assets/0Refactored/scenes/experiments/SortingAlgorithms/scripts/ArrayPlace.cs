using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArrayPlace : MonoBehaviour
{
    [SerializeField] private TextMeshPro baseText;
    [SerializeField] private TextMeshPro indexText;
    [SerializeField] private GameObject highlight;

    private SortingElement _element;

    private void Awake()
    {
        _element = GetComponentInChildren<SortingElement>();
    }

    public void SetBaseNumber(int index)
    {
        baseText.text = index.ToString();
    }

    public int Value
    {
        get => _element.Value;
        set => _element.Value = value;
    }

    public bool Hidden
    {
        get => _element.Hidden;
        set => _element.Hidden = value;
    }

    public void HighlightForSeconds(float seconds)
    {
        StartCoroutine(HighlightForSecondsCoroutine(seconds));
    }

    private IEnumerator HighlightForSecondsCoroutine(float seconds)
    {
        highlight.SetActive(true);
        yield return new WaitForSeconds(seconds);
        highlight.SetActive(false);
    }

    public void FadeOutSeconds(float seconds)
    {
        _element.FadeOutSeconds(seconds);
    }
    
    public void FadeInSeconds(float seconds)
    {
        _element.FadeInSeconds(seconds);
    }

    public void MoveOutRight(float seconds, Vector3 offset)
    {
        _element.MoveOutRight(seconds, offset);
    }
    
    public void MoveOutLeft(float seconds, Vector3 offset)
    {
        _element.MoveOutLeft(seconds, offset);
    }
    
    public void MoveInRight(float seconds, Vector3 offset)
    {
        _element.MoveInRight(seconds, offset);
    }
    
    public void MoveInLeft(float seconds, Vector3 offset)
    {
        _element.MoveInLeft(seconds, offset);
    }

    public void MarkAsSubset()
    {
        _element.SetDefaultColor();
    }
    
    public void MarkAsNotSubset()
    {
        _element.SetGrayColor();
    }

    public void SetIndexText(List<string> indices)
    {
        indexText.text = String.Join("\n", indices);
    }

    public void FinishActiveVisualizations()
    {
        StopAllCoroutines();
        highlight.SetActive(false);
        _element.FinishActiveVisualizations();
    }
    
    public void ResetVisualization()
    {
        FinishActiveVisualizations();
        _element.ResetVisualization();
    }
}
