using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SortingBucket : MonoBehaviour
{
    [SerializeField] private TextMeshPro indexText;

    [SerializeField] private GameObject highlight;

    public LinkedList<int> StoredElements = new LinkedList<int>();

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
    
    public void SetIndex(int i)
    {
        indexText.text = i.ToString();
    }
}
