using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public class PathHandler : MonoBehaviour
{
    public enum CameraPosition
    {
        CP_Free = 0,
        CP_Front = 1,
        CP_Side = 2,
        CP_Top = 3
    }

    public float pathTime = 2f;

    public Transform frontPoint;
    public Transform sidePoint;
    public Transform topPoint;

    public Transform freeMinimumPoint;
    public Transform freeMaximumPoint;

    private CameraPosition _currentPosition = CameraPosition.CP_Free;
    private CPC_CameraPath _workingPath;
    private void Start()
    {
        _workingPath = GetComponent<CPC_CameraPath>();
    }

    public CameraPosition GetCurrentPosition()
    {
        return _currentPosition;
    }
    
    public void SwitchPosition(int posIdx)
    {
        // Debug.Log("Switch Pos: " + posIdx);
        SwitchPosition((CameraPosition)posIdx);
    }
    
    public void SwitchPosition(CameraPosition newPosition)
    {
        // Debug.Log("Switch Position: "+ newPosition);
        if(_currentPosition == newPosition) return;

        _workingPath.points.RemoveAll(point => true); // delete every point
        var offsetToOrigin = frontPoint.position.z - sidePoint.position.z;
        var prevHandleOffset = Vector3.zero;

        _workingPath.lookAtTarget = false;
        switch (_currentPosition)
        {
            case CameraPosition.CP_Free:
                Debug.Assert(Camera.main != null, "Camera.main != null");
                _workingPath.points.Add(new CPC_Point(Camera.main.transform.position, Camera.main.transform.rotation));
                _workingPath.lookAtTarget = true;
                break;
            case CameraPosition.CP_Front:
                _workingPath.points.Add(new CPC_Point(frontPoint.position, frontPoint.localRotation));
                prevHandleOffset.z = offsetToOrigin;
                break;
            case CameraPosition.CP_Side:
                _workingPath.points.Add(new CPC_Point(sidePoint.position, sidePoint.localRotation));
                prevHandleOffset.x = offsetToOrigin;
                break;
            case CameraPosition.CP_Top:
                _workingPath.points.Add(new CPC_Point(topPoint.position, topPoint.localRotation));
                prevHandleOffset.y = -offsetToOrigin;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        _workingPath.points[0].handlenext = _workingPath.points[0].handleprev = Vector3.zero;
        var correctionIdx = 1;
        
        switch (newPosition)
        {
            case CameraPosition.CP_Free:
                _workingPath.points.Add(new CPC_Point(freeMinimumPoint.position + (freeMaximumPoint.position - freeMinimumPoint.position) * 0.5f, Camera.main.transform.rotation));
                _workingPath.lookAtTarget = true;
                break;
            case CameraPosition.CP_Front:
                _workingPath.points.Add(new CPC_Point(frontPoint.position, frontPoint.localRotation));
                break;
            case CameraPosition.CP_Side:
                if (_currentPosition == CameraPosition.CP_Top)
                {
                    _workingPath.points.Add(new CPC_Point(topPoint.position, Quaternion.Euler(new Vector3(90, 90, 0))));
                    _workingPath.points[_workingPath.points.Count - 1].handlenext = _workingPath.points[_workingPath.points.Count - 1].handleprev = Vector3.zero;
                    correctionIdx = 2;
                }
                _workingPath.points.Add(new CPC_Point(sidePoint.position, sidePoint.localRotation));
                break;
            case CameraPosition.CP_Top:
                if (_currentPosition == CameraPosition.CP_Side)
                {
                    _workingPath.points.Add(new CPC_Point(topPoint.position,Quaternion.Euler(new Vector3(90, 90, 0))));
                    _workingPath.points[_workingPath.points.Count - 1].handlenext = _workingPath.points[_workingPath.points.Count - 1].handleprev = Vector3.zero;
                }
                _workingPath.points.Add(new CPC_Point(topPoint.position, topPoint.localRotation));
                _workingPath.lookAtTarget = false;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _workingPath.points[_workingPath.points.Count - 1].handlenext = _workingPath.points[_workingPath.points.Count - 1].handleprev = Vector3.zero;
        _workingPath.points[correctionIdx].handleprev = prevHandleOffset;
        _currentPosition = newPosition;

        Debug.Assert(Camera.main != null, "Camera.main != null");
        Debug.Assert(Camera.main.GetComponent<PC_ZoomMovement>(), "Camera must have a component PC_ZoomMovement");
        Camera.main.GetComponent<PC_ZoomMovement>().enabled = _currentPosition == CameraPosition.CP_Free;
        
        _workingPath.PlayPath(pathTime * _workingPath.points.Count);
    }
}
