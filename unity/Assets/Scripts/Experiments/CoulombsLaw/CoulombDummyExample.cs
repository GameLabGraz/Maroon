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
    public CoulombLogic logic;
    [Range(1, 60)]
    public float timeInterval = 30;
    [Range(0, 60)] 
    public float startSimulationAfter = 5;

    public GameObject ChargePrefab;
    public int maxNumCharges = 4;

    // [Header("Axis Mapping")] 
    // public Mapping mapping;
    
    private float _currentTime = -1f;
    private bool _simulationStarted = false;

    // private List<GameObject> _charges = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        _currentTime -= Time.deltaTime;

        if (!_simulationStarted && _currentTime < timeInterval - startSimulationAfter)
        {
            SimulationController.Instance.StartSimulation();
            _simulationStarted = true;
        }
        
        if(_currentTime < 0)
            InitNewAnimation();
    }

    private void InitNewAnimation()
    {
        var numCharges = Random.Range(2f, maxNumCharges);
        SimulationController.Instance.StopSimulation();
        
        logic.RemoveAllParticles(true);

        for (var i = 0; i < Mathf.FloorToInt(numCharges); ++i)
        {
            // GameObject charge = null;
            // var chargeValue = Random.Range(-5f, 5f);
            // if (i < _charges.Count)
            // {
            //     charge = _charges[i];
            //     charge.SetActive(true);
            // }
            // else
            // {
            //     charge = logic.CreateCharge(ChargePrefab, Vector3.zero, chargeValue, false);
            //     _charges.Add(charge);
            // }
            //
            // charge.GetComponent<CoulombChargeBehaviour>().SetInUse(true);
            //
            // var calcPosition = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), 0f);
            // Debug.Log("Random Position: " + calcPosition);
            //
            // var realMapped = mapping.MapFromCalcToReal(calcPosition);
            // charge.transform.position = logic.xOrigin2d.position;
            // var localPos = charge.transform.localPosition;
            // localPos.x += realMapped.x * logic.GetWorldToCalcFactor(true);
            // localPos.y += realMapped.y * logic.GetWorldToCalcFactor(true);
            // localPos.z += realMapped.z * logic.GetWorldToCalcFactor(true);
            // charge.transform.localPosition = localPos;

            
            var chargeValue = Random.Range(-5f, 5f);
            var position = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), 0f);
            var newCharge = logic.CreateChargeAtCalcSpacePosition(ChargePrefab, position, chargeValue, false);
            newCharge.GetComponent<CoulombChargeBehaviour>().SetInUse(true);
        }

        _simulationStarted = false;
        _currentTime = timeInterval;
    }
}
