using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CoordSystem : MonoBehaviour
{
    [SerializeField] private Transform origin;
    [SerializeField] private CoordSystemManager systemManager;

    public GameObject testCube;

    private static CoordSystem _instance;
    public static CoordSystem Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<CoordSystem>();
            return _instance;
        }
    }

    private void Start()
    {
        _ = testCube ?? throw new NullReferenceException();
        _ = origin ?? throw new NullReferenceException();
        _ = systemManager ?? throw new NullReferenceException();

        var t = GetPositionInLocalSpaceUnits(testCube.transform.position);
    }

    public Vector3 GetPositionInLocalSpaceUnits(Vector3 objectTransform)
    {
        var systemSpaceCoordinates = WorldCoordinatesToSystemSpace(objectTransform);
        var axisDictionary = systemManager.GetAxisDictionary();

        var xValue = axisDictionary[Axis.X].GetValueFromAxisPoint(systemSpaceCoordinates.x);
        var yValue = axisDictionary[Axis.Y].GetValueFromAxisPoint(systemSpaceCoordinates.y);
        var zValue = axisDictionary[Axis.Z].GetValueFromAxisPoint(systemSpaceCoordinates.z);

        return new Vector3(xValue, yValue, zValue);
    }

    public Vector3 WorldCoordinatesToSystemSpace(Vector3 objectTransform)
    {
        var axisLengths = systemManager.GetWorldLengthsOfDirection(true);
        var objectPosition = origin.InverseTransformDirection(objectTransform);
        
        var objectPositionRelativeToAxisLength = new Vector3(objectPosition.x / axisLengths.x, 
                                                             objectPosition.y / axisLengths.y,
                                                             objectPosition.z / axisLengths.z);

        Debug.Log($"POSITION OF TEST CUBE: {objectPositionRelativeToAxisLength}");
        return objectPositionRelativeToAxisLength;
    }

    public Vector3 GetPositionInWorldSpace(Vector3 localSpacePosition)
    {
        
        return Vector3.back;
    }

    
    private float CalculatePointOnAxisFromValue()
    {
        return 0.0f;
    }

    private float CalculateValueFromAxisPoint(float axisPoint, Axis axis)
    {
        var axisWorldLengths = systemManager.GetWorldLengthsOfDirection(true);
        var value = 0;

        return 0f;
    }

    /*
    public float WorldToCalcSpace(float distanceWorldSpace, bool local = false)
    {
        if (IsIn2dMode())
            return distanceWorldSpace / (local ? _worldToCalcSpaceFactor2dLocal : _worldToCalcSpaceFactor2d);
        return distanceWorldSpace / (local ? _worldToCalcSpaceFactor3dLocal : _worldToCalcSpaceFactor3d);
    }

    public Vector3 WorldToCalcSpace(Transform position)
    {
        if (IsIn2dMode())
        {
            var originPos = xOrigin2d.position;
            var calculationPos = position.position;
            return new Vector3(WorldToCalcSpace(Mathf.Abs(originPos.x - calculationPos.x)),
                WorldToCalcSpace(Mathf.Abs(originPos.y - calculationPos.y)), 0f);
        }
        else
        {
            //3d mode
            var originPos = xOrigin3d.localPosition;
            var calculatePos = xOrigin3d.parent.InverseTransformPoint(position.position);

            return new Vector3(WorldToCalcSpace(Mathf.Abs(originPos.x - calculatePos.x), true),
                WorldToCalcSpace(Mathf.Abs(originPos.y - calculatePos.y), true),
                WorldToCalcSpace(Mathf.Abs(originPos.z - calculatePos.z), true));
        }
    }

    public Vector3 CalcToWorldSpace(Vector3 distance, bool local = false)
    {
        return new Vector3(CalcToWorldSpace(distance.x, local), CalcToWorldSpace(distance.y, local),
            CalcToWorldSpace(distance.z, local));
    }

    public Vector3 CalcToWorldSpaceCoordinates(Vector3 coord, bool local = true)
    {
        if (local)
        {
            var origin = Vector3.zero;
            var at1m = Vector3.zero;
            if (IsIn2dMode())
            {
                var tmp = coord.y;
                coord.y = coord.z;
                coord.z = tmp;
                origin = xOrigin2d.transform.localPosition;
                at1m = xAt1m2d.transform.localPosition;
            }
            else
            {
                origin = xOrigin3d.transform.localPosition;
                at1m = xAt1m3d.transform.localPosition;
            }


            return new Vector3(
                origin.x + (at1m.x - origin.x) * coord.x,
                origin.y + (at1m.y - origin.y) * coord.y,
                origin.z + (at1m.z - origin.z) * coord.z);
        }

        throw new NotImplementedException();
    }

    public float CalcToWorldSpace(float distanceCalcSpace, bool local = false)
    {
        if (IsIn2dMode())
            return distanceCalcSpace * (local ? _worldToCalcSpaceFactor2dLocal : _worldToCalcSpaceFactor2d);
        return distanceCalcSpace * (local ? _worldToCalcSpaceFactor3dLocal : _worldToCalcSpaceFactor3d);
    }*/
}
