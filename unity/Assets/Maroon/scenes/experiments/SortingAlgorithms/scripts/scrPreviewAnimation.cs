using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrPreviewAnimation : MonoBehaviour
{
    private ArrayPlace[] _arrayPlaces;

    [SerializeField] private float blinkSpeed;
    [SerializeField] private float periodTime;
    
    // Start is called before the first frame update
    void Start()
    {
        _arrayPlaces = GetComponentsInChildren<ArrayPlace>();
        InvokeRepeating(nameof(StartBlinkSubroutine), 0, periodTime);
    }

    private void StartBlinkSubroutine()
    {
        StartCoroutine(HighlightPlacesSubroutine());
    }

    private IEnumerator HighlightPlacesSubroutine()
    {
        foreach (var place in _arrayPlaces)
        {
            place.HighlightForSeconds(blinkSpeed * 1.5f);
            yield return new WaitForSeconds(blinkSpeed);
        }
    }
    
}
