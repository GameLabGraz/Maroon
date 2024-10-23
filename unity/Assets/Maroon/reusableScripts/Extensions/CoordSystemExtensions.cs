using UnityEngine;

namespace Maroon.Extensions
{
    public static class CoordSystemExtensions
    {
        public static Vector3 SystemPosition(this GameObject gameObject)
        {
            return Maroon.GlobalEntities.CoordSystemHandler.Instance.GetSystemPosition(gameObject.transform.position);
        }
    }
}
