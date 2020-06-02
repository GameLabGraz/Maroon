using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
// public class Mapping
// {
//     public enum Axis
//     {
//         XAxis,
//         YAxis,
//         ZAxis
//     }
//
//     public Axis calcXAxisToRealAxis = Axis.XAxis;
//     public Axis calcYAxisToRealAxis = Axis.YAxis;
//     public Axis calcZAxisToRealAxis = Axis.ZAxis;
//
//     private float GetRealAxis(Vector3 vec, Axis axis)
//     {
//         if (calcXAxisToRealAxis == axis)
//             return vec.x;
//         if (calcYAxisToRealAxis == axis)
//             return vec.y;
//         if (calcZAxisToRealAxis == axis)
//             return vec.z;
//         throw new ArgumentOutOfRangeException();
//     }
//
//     private float GetCalcAxis(Vector3 vec, int axisIdentify)
//     {
//         switch (axisIdentify == 0? calcXAxisToRealAxis : axisIdentify == 1? calcYAxisToRealAxis : calcZAxisToRealAxis)
//         {
//             case Axis.XAxis:
//                 return vec.x;
//             case Axis.YAxis:
//                 return vec.y;
//             case Axis.ZAxis:
//                 return vec.z;
//         }
//         throw new ArgumentOutOfRangeException();
//     }
//     
//     public Vector3 MapFromCalcToReal(Vector3 calc)
//     {
//         var real = Vector3.zero;
//         real.x = GetRealAxis(calc, Axis.XAxis);
//         real.y = GetRealAxis(calc, Axis.YAxis);
//         real.z = GetRealAxis(calc, Axis.ZAxis);
//         return real;
//     }
//
//     public Vector3 MapFromRealToCalc(Vector3 real)
//     {
//         var calc = Vector3.zero;
//         calc.x = GetCalcAxis(real, 0);
//         calc.y = GetCalcAxis(real, 1);
//         calc.z = GetCalcAxis(real, 2);
//         return calc;
//     }
//
// }

public class CoulombDummyExample : MonoBehaviour
{
    [Serializable]
    public struct ChargeStruct
    {
        public Vector2 position;
        [Range(-5f, 5f)] public float charge;
    }
    
    public CoulombLogic logic;
    public GameObject ChargePrefab;
    public List<ChargeStruct> charges = new List<ChargeStruct>();
    
    private bool _initialized = false;

    void Update()
    {
        if(!_initialized)
            InitNewAnimation();
    }

    private void InitNewAnimation()
    {
        SimulationController.Instance.StopSimulation();
        logic.RemoveAllParticles(true);

        foreach (var chargeTemplate in charges)
        {
            var position = new Vector3(chargeTemplate.position.x, chargeTemplate.position.y, 0f);
            var newCharge = logic.CreateChargeAtCalcSpacePosition(ChargePrefab, position, chargeTemplate.charge, false);
            newCharge.GetComponent<CoulombChargeBehaviour>().SetInUse(true);
        }

        _initialized = true;
    }
}
