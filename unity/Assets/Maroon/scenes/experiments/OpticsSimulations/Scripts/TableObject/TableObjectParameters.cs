using Maroon.Utils;
using UnityEngine;

namespace Maroon.Physics.Optics.TableObject
{
    [System.Serializable]
    public class TableObjectParameters
    {
        /// <summary>
        /// The position of the TableObject.
        /// </summary>
        public SerializableVector3 Position;
        /// <summary>
        /// The rotation of the TableObject in the form of Euler angles. Will overriden by RotationQuaternion if that is is set.
        /// </summary>
        public SerializableVector3? Rotation;
        /// <summary>
        /// The rotation of the TableObject in the form of a Quaternion. Will override Rotation if this variable set.
        /// </summary>
        public SerializableQuaternion? RotationQuaternion;

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