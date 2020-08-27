using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class PositionReset : MonoBehaviour, IResetWholeObject
{

    private Vector3 _startLocalPosition;
    private float _timeToReset = -1f;
    private float _currentTime = 0f;
    private Vector3 _resetStartPos; 
    private bool _isResetting = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _startLocalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isResetting) return;
        _currentTime += Time.deltaTime / _timeToReset;
        transform.localPosition = Vector3.Lerp(_resetStartPos, _startLocalPosition, _currentTime);

        if (_currentTime >= 1f)
        {
            _isResetting = false;
        }
        
    }

    public void ResetObject()
    {
    }

    public void ResetWholeObject()
    {
        transform.localPosition = _startLocalPosition;
    }

    public void ResetPosition(float time)
    {
        _timeToReset = time;
        _currentTime = 0f;
        _isResetting = true;
        _resetStartPos = transform.localPosition;
    }
}
