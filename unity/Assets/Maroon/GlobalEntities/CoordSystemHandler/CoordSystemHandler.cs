using UnityEngine;
using Maroon.Physics.CoordinateSystem;
using System.Collections.Generic;

namespace Maroon.GlobalEntities
{
    public class CoordSystemHandler: MonoBehaviour, GlobalEntity
    {
        private static CoordSystemHandler _instance = null;
        public static CoordSystemHandler Instance => CoordSystemHandler._instance;

        MonoBehaviour GlobalEntity.Instance => Instance;

        public bool IsCoordSystemAvailable => CoordSystem.Instance != null;
        public List<Unit> GetSubdivisionUnits() => CoordSystem.Instance != null ? CoordSystem.Instance.GetAxisSubDivisionUnits() : new List<Unit>() { Unit.m, Unit.m, Unit.m };
        public Vector3 GetSystemPosition(Vector3 position, Unit targetUnit = Unit.respective) => CoordSystem.Instance != null 
                                                                                        ? CoordSystem.Instance.GetPositionInAxisUnits(position, targetUnit) 
                                                                                        : position;

        public Vector3 GetWorldPosition(Vector3 localposition) => CoordSystem.Instance != null
                                                                                      ? CoordSystem.Instance.GetPositionInWorldSpace(localposition, CoordSystem.Instance.GetAxisSubDivisionUnits().ToArray())
                                                                                      : localposition;

        public float CalculateDistanceBetween(Vector3 positionOne, Vector3 positionTwo, Unit targetUnit = Unit.m) => Vector3.Distance(GetSystemPosition(positionOne, targetUnit), GetSystemPosition(positionTwo, targetUnit));

        private void Awake()
        {
            if (CoordSystemHandler._instance == null)
            {
                CoordSystemHandler._instance = this;
            }
            else if (CoordSystemHandler._instance != this)
            {
                DestroyImmediate(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
        }     
    }
}

