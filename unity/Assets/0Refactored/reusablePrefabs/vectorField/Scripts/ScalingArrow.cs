using System;
using UnityEngine;

public class ScalingArrow : MonoBehaviour
{
    public enum ScaleAxis
    {
        XAxis,
        YAxis,
        ZAxis
    }
    
    [Header("Child Objects")] 
    public GameObject arrowHead;
    public GameObject arrowBody;

    [Header("Settings")] 
    [Range(0f, 100f)]
    public float size = 0f;

    public ScaleAxis scaleAxis = ScaleAxis.YAxis;
    [Tooltip("The minimum scale of the arrowbody")]
    public float minimumScale = 100f;
    [Tooltip("The maximum scale of the arrowbody")]
    public float maximumScale = 200f;

    private float _currentSizeInPercent;
    private Vector3 _startTranslateBody;
    private Vector3 _startTranslateHead;
    private Vector3 _startScale;
    
    // Start is called before the first frame update
    void Start()
    {
        _startTranslateBody = arrowBody.transform.localPosition;
        _startTranslateHead = arrowHead.transform.localPosition;
        _startScale = arrowBody.transform.localScale;
        _currentSizeInPercent = size;
        AdaptScale();
    }

    // Update is called once per frame
    void Update()
    {
        if (Math.Abs(size - _currentSizeInPercent) > 0.0001)
        {
            _currentSizeInPercent = size;
            AdaptScale();
        }
    }

    private void AdaptScale()
    {
        var newScaleFactor = minimumScale + (maximumScale - minimumScale) * (_currentSizeInPercent / 100f);

        var newScale = _startScale;
        var newTranslateBody = _startTranslateBody;
        var newTranslateHead = _startTranslateHead;
        switch (scaleAxis)
        {
            case ScaleAxis.XAxis:
            {
                newScale.x = newScaleFactor;
                var translateIncrease = _startTranslateBody.x * (newScale.x / _startScale.x) - _startTranslateBody.x;
                newTranslateBody.x = _startTranslateBody.x + translateIncrease / 2f;
                newTranslateHead.x = _startTranslateHead.x - translateIncrease / 2f;
            }
                break;
            case ScaleAxis.YAxis:
            {
                newScale.y = newScaleFactor;
                var translateIncrease = _startTranslateBody.y * (newScale.y / _startScale.y) - _startTranslateBody.y;
                newTranslateBody.y = _startTranslateBody.y + translateIncrease / 2f;
                newTranslateHead.y = _startTranslateHead.y - translateIncrease / 2f;
            }
                break;
            case ScaleAxis.ZAxis:
            {
                newScale.z = newScaleFactor;
                var translateIncrease = _startTranslateBody.z * (newScale.z / _startScale.z) - _startTranslateBody.z;
                newTranslateBody.z = _startTranslateBody.z + translateIncrease / 2f;
                newTranslateHead.z = _startTranslateHead.z - translateIncrease / 2f;
            }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        arrowBody.transform.localScale = newScale;
        arrowBody.transform.localPosition = newTranslateBody;
        arrowHead.transform.localPosition = newTranslateHead;
    }
    
    
}
