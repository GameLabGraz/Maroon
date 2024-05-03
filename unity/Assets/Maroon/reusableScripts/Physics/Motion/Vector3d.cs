using UnityEngine;

namespace Maroon.Physics.Motion
{
    public struct Vector3d
    {
        public double x;
        public double y;
        public double z;

        public Vector3d(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3d(float x, float y, float z)
        {
            this.x = (double)x;
            this.y = (double)y;
            this.z = (double)z;
        }

        public Vector3d(Vector3 v3)
        {
            this.x = (double)v3.x;
            this.y = (double)v3.y;
            this.z = (double)v3.z;
        }

        public static Vector3d operator +(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static Vector3d operator -(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3d operator *(Vector3d a, double d)
        {
            return new Vector3d(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3d operator *(double d, Vector3d a)
        {
            return new Vector3d(a.x * d, a.y * d, a.z * d);
        }

        public static explicit operator Vector3d(Vector3 v)
        {
            return new Vector3d(v);
        }

        public static explicit operator Vector3(Vector3d v)
        {
            return new Vector3((float)v.x, (float)v.y, (float)v.z);
        }
    }

}