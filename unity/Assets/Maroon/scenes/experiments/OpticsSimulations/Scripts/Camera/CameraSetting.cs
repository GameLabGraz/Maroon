using Maroon.Utils;
using UnityEngine;

namespace Maroon.Physics.Optics.Camera
{
    [System.Serializable]
    public struct CameraSetting
    {
        public SerializableVector3 Position;
        public SerializableQuaternion Rotation;
        public float FOV;

        public CameraSetting(Vector3 position, Quaternion rotation, float fov)
        {
            Position = new SerializableVector3(position);
            Rotation = new SerializableQuaternion(rotation);
            FOV = fov;
        }
    }
}