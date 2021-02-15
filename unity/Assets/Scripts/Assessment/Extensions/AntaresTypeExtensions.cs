using System;
using UnityEngine;
using Antares.Evaluation;

public static class AntaresTypeExtensions
{
    public static Vector3 ToVector3(this Vector3D vector)
    {
        return new Vector3(vector.x, vector.y, vector.z);
    }

    public static Vector3D ToVector3D(this Vector3 vector)
    {
        return new Vector3D(vector.x, vector.y, vector.z);
    }

    public static object ToAntaresValue(this object obj)
    {
        if (obj is Vector3 vector) return vector.ToVector3D();

        return obj;
    }

    public static object ToUnityValue(this object obj)
    {
        switch (obj)
        {
            case Vector3D vector:
                return vector.ToVector3();
            case double value:
                return (float) value;
            case long value:
                return (int)value;
            default:
                return obj;
        }
    }
}
