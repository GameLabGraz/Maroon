using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoulombDummyExample : MonoBehaviour
{
    public CoulombLogic logic;
    [Range(1, 60)]
    public float timeInterval = 30;
    [Range(0, 60)] 
    public float startSimulationAfter = 5;

    public GameObject ChargePrefab;
    public int maxNumCharges = 4;
    
    private float _currentTime = -1f;
    private bool _simulationStarted = false;

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
        var numCharges = Random.Range(2, maxNumCharges);
        SimulationController.Instance.StopSimulation();
        
        logic.RemoveAllParticles(true);

        for (var i = 0; i < numCharges; ++i)
        {
            var chargeValue = Random.Range(-5f, 5f);
            var position = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), 0f);
            
            var newCharge = logic.CreateChargeAtCalcSpacePosition(ChargePrefab, position, chargeValue, false);
            newCharge.GetComponent<CoulombChargeBehaviour>().SetInUse(true);
            
            // logic.CreateCharge(ChargePrefab, position, chargeValue, false, false);
        }

        _simulationStarted = false;
        _currentTime = timeInterval;
    }
}
