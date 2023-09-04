using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackToOrigin : MonoBehaviour
{

    private Vector3 _startLocalPosition;
    private float _timeToReset = -1f;
    private float _currentTime = 0f;
    private Vector3 _resetStartPos;
    private bool _isResetting = false;
    // Start is called before the first frame update
    void Start()
    {
        _startLocalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isResetting) return;

        _currentTime += Time.deltaTime / _timeToReset;
        transform.position = Vector3.Lerp(_resetStartPos, _startLocalPosition, _currentTime);

        if (_currentTime >= 1f)
        {
            _isResetting = false;
        }

        

    }
    public void resetPosition(float time)
    {
        _timeToReset = time;
        _currentTime = 0f;
        _isResetting = true;
        _resetStartPos = transform.position;
    }
}
