using System.Collections;
using System.Collections.Generic;
using GameLabGraz.VRInteraction;
using UnityEngine;

public class VRScrollingHelper : MonoBehaviour
{
    [SerializeField] private VRLinearDrive scrollhandle = null;
    [SerializeField] private List<GameObject> controls = new List<GameObject>();

    private const int MaxDisplayObjects = 4;
    private const float LocalZOffset = -0.12f;
    private int _currentPage = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(scrollhandle != null);

        if (controls.Count <= MaxDisplayObjects)
        {
            //thats the maximum of current displayed controls
            scrollhandle.gameObject.SetActive(false); // not needed
        }
        else
        {
            scrollhandle.maximum = controls.Count - MaxDisplayObjects;
            scrollhandle.stepSize = 1f;
            scrollhandle.useAsInteger = true;

            scrollhandle.onValueChangedInt.AddListener(x =>
            {
                UpdateControlsToCurrentPage(x);
            });
        }
        
        _currentPage = 0;
        UpdateControlsToCurrentPage(_currentPage, true);
    }

    protected void UpdateControlsToCurrentPage(int current, bool force = false)
    {
        if (_currentPage == current && !force) return;
        _currentPage = current;

        var isVisible = false;
        var visibleIdx = 0;
        for (var i = 0; i < controls.Count; ++i)
        {
            isVisible = _currentPage <= i && i < _currentPage + MaxDisplayObjects;
            controls[i].SetActive(isVisible);

            if (isVisible)
            {
                controls[i].transform.localPosition = new Vector3(0f, 0f, LocalZOffset * visibleIdx);
                visibleIdx++;
            }
        }
        Debug.Assert(visibleIdx <= MaxDisplayObjects);
    }
}
