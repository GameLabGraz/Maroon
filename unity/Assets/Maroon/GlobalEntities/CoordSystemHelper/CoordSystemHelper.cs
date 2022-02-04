using UnityEngine;
using Maroon.Physics.CoordinateSystem;
using System.Collections.Generic;

namespace Maroon.GlobalEntities
{
    public class CoordSystemHelper : MonoBehaviour, GlobalEntity
    {
        private static CoordSystemHelper _instance = null;
        public static CoordSystemHelper Instance => CoordSystemHelper._instance;

        MonoBehaviour GlobalEntity.Instance => Instance;

        public bool IsCoordSystemAvailable => CoordSystem.Instance != null;
        public List<Unit> GetSubdivisionUnits() => CoordSystem.Instance != null ? CoordSystem.Instance.GetAxisSubDivisionUnits() : new List<Unit>() { Unit.m, Unit.m, Unit.m };
        public Vector3 GetSystemPosition(Vector3 position, Unit targetUnit = Unit.respective) => CoordSystem.Instance != null 
                                                                                        ? CoordSystem.Instance.GetPositionInAxisUnits(position, targetUnit) 
                                                                                        : position;
        private void Awake()
        {
            if (CoordSystemHelper._instance == null)
            {
                CoordSystemHelper._instance = this;
            }
            else if (CoordSystemHelper._instance != this)
            {
                DestroyImmediate(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
        }
 
        
        public float GetSystemDistance(Vector3 startPosition, Vector3 endPosition, Unit targetUnit = Unit.m)
        {
            if (CoordSystem.Instance != null)
            {
                var localStartPos = GetSystemPosition(startPosition, targetUnit);
                var localEndPos = GetSystemPosition(endPosition, targetUnit);
                return Vector3.Distance(localStartPos,localEndPos);
            }
            else
            {
                return Vector3.Distance(startPosition, endPosition);
            }
        }

        public Vector3 CalculateNewWorldPosition(Vector3 oldPosition,float value, Vector3 axis)
        {
            var newPosition = oldPosition;
            var tempVector = new Vector3(value * axis.x, value * axis.y, value * axis.z);
           
            if (CoordSystem.Instance == null)
            {
                if (axis.x > 0.1)
                    newPosition.x = tempVector.x;
                if (axis.y > 0.1)
                    newPosition.y = tempVector.y;
                if (axis.z > 0.1)
                    newPosition.z = tempVector.z;

                return newPosition;
            }         
             
            var valueChange = CoordSystem.Instance.GetPositionInWorldSpace(tempVector, CoordSystem.Instance.GetAxisSubDivisionUnits().ToArray());
          
            if (axis.x > 0.1)
                newPosition.x = valueChange.x;
            if(axis.y > 0.1)
                newPosition.y = valueChange.y;
            if(axis.z > 0.1)
                newPosition.z = valueChange.z;

            return newPosition;
        }
    }
}

