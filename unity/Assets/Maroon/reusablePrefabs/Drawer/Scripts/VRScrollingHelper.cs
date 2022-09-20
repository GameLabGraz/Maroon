using System.Collections;
using System.Collections.Generic;
using GameLabGraz.VRInteraction;
using UnityEngine;

public class VRScrollingHelper : MonoBehaviour
{
    [SerializeField] private VRLinearDrive scrollhandle = null;
    [SerializeField] private List<GameObject> scrollingOnlyObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> controls = new List<GameObject>();

    private const int MaxDisplayObjects = 4;
    private const float LocalZOffset = -0.12f;
    private int _currentPage = 0;
    private bool _firstUpdate = true;

    private int _activeElements = -1;
    
    // Start is called before the first frame update
    void FixedUpdate()
    {
        if (_firstUpdate) //need this otherwise not all StartFunctions of our controls are being called 
        {
            RecalculateScrolling(-1);
        }
    }

    protected void UpdateControlsToCurrentPage(int current, bool force = false)
    {
        if (_currentPage == current && !force) return;
        _currentPage = current;

        var isVisible = false;
        var visibleIdx = 0;
        var activeElementsCnt = GetControlsCount();
        
        for (var i = 0; i < controls.Count; ++i)
        {
            isVisible = _currentPage <= i && i < _currentPage + MaxDisplayObjects && i < activeElementsCnt;
            controls[i].SetActive(isVisible);

            if (isVisible)
            {
                controls[i].transform.localPosition = new Vector3(0f, 0f, LocalZOffset * visibleIdx);
                visibleIdx++;
            }
        }
        Debug.Assert(visibleIdx <= MaxDisplayObjects);
    }

    public void RecalculateScrolling(int activeElements)
    {
        Debug.Assert(scrollhandle != null);
        _activeElements = activeElements;

        var cntControls = GetControlsCount();
        if (cntControls <= MaxDisplayObjects)
        {
            //thats the maximum of current displayed controls
            scrollhandle.gameObject.SetActive(false); // not needed
            foreach(var obj in scrollingOnlyObjects)
                obj.SetActive(false);
        }
        else
        {
            scrollhandle.gameObject.SetActive(true);
            foreach(var obj in scrollingOnlyObjects)
                obj.SetActive(true);
            
            scrollhandle.SetMinMax(0, cntControls - MaxDisplayObjects);
            scrollhandle.stepSize = 1f;
            scrollhandle.useAsInteger = true;

            scrollhandle.onValueChangedInt.AddListener(x => { UpdateControlsToCurrentPage(x); });
        }

        _currentPage = 0;
        UpdateControlsToCurrentPage(_currentPage, true);
        _firstUpdate = false;
    }

    protected int GetControlsCount()
    {
        if (_activeElements < 0 || _activeElements > controls.Count)
        {
            return controls.Count;
        }

        return _activeElements;
    }
}
