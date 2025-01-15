using UnityEngine;

namespace Maroon.Utils
{
    /// <summary>
    /// Serializable Quaternion, to prevent self-referencing loop when serializing Quaternion for JSON
    /// </summary>
    [System.Serializable]
    public class SerializableQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public SerializableQuaternion(Quaternion quaternion)
        {
            x = quaternion.x;
            y = quaternion.y;
            z = quaternion.z;
            w = quaternion.w;
        }

        public Quaternion Get()
        {
            return new Quaternion(x, y, z, w);
        }

        public void Set(Quaternion newQuaternion)
        {
            x = newQuaternion.x;
            y = newQuaternion.y;
            z = newQuaternion.z;
            w = newQuaternion.w;
        }

        public static implicit operator SerializableQuaternion(Quaternion quaternion)
        {
            return new SerializableQuaternion(quaternion);
        }


        public static implicit operator Quaternion(SerializableQuaternion serializableQuaternion)
        {
            return serializableQuaternion.Get();
        }
    }
}