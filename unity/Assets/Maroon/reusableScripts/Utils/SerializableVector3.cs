using UnityEngine;

namespace Maroon.Utils
{
    /// <summary>
    /// Serializable Vector3, to prevent self-referencing loop (.normalized.normalized.normalized...) when serializing Vector3 for JSON
    /// </summary>
    [System.Serializable]
    public class SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 Get()
        {
            return new Vector3(x, y, z);
        }

        public void Set(Vector3 newVector)
        {
            x = newVector.x;
            y = newVector.y;
            z = newVector.z;
        }

        public static implicit operator SerializableVector3(Vector3 vector)
        {
            return new SerializableVector3(vector);
        }

        public static implicit operator Vector3?(SerializableVector3 serializableVector3)
        {
            if (serializableVector3 == null)
                return null;
            return serializableVector3.Get();
        }

        public static implicit operator Vector3(SerializableVector3 serializableVector3)
        {
            if (serializableVector3 == null)
                return Vector3.zero;
            return serializableVector3.Get();
        }
    }
}