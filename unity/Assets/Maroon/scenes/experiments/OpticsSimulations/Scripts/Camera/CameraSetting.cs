using Maroon.Utils;
using UnityEngine;

namespace Maroon.Physics.Optics.Camera
{
    [System.Serializable]
    public struct CameraSetting
    {
        public SerializableVector3 Position;
        public SerializableQuaternion RotationQuaternion;
        /// <summary>
        /// Rotation as Euler angles. If RotationQuaternion is set, this value will be overriden.
        /// </summary>
        public SerializableVector3 Rotation;
        public float FOV;

        public CameraSetting(Vector3 position, Quaternion rotationQuaternion, float fov)
        {
            Position = new SerializableVector3(position);
            RotationQuaternion = new SerializableQuaternion(rotationQuaternion);
            Rotation = new SerializableVector3(rotationQuaternion.eulerAngles);
            FOV = fov;
        }

        public CameraSetting(Vector3 position, Vector3 rotationEuler, float fov)
        {
            Position = new SerializableVector3(position);
            RotationQuaternion = new SerializableQuaternion(Quaternion.Euler(rotationEuler));
            Rotation = new SerializableVector3(rotationEuler);
            FOV = fov;
        }

        public void CheckRotations()
        {
            if (RotationQuaternion != null)
            {
                Rotation = new SerializableVector3(RotationQuaternion.Get().eulerAngles);
            }
            else if (Rotation != null)
            {
                RotationQuaternion = new SerializableQuaternion(Quaternion.Euler(Rotation.Get()));
            }
        }
    }
}