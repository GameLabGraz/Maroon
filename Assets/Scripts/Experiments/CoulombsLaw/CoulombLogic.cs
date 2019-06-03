using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoulombLogic : MonoBehaviour
{
    private SimulationController simController;

    private List<ParticleBehaviour> _particles;

    private const float CoulombConstant = 9f; // = 9 * 10^9 -> but we use the factor 0.001 beneath because we have constant * microCoulomb * microCoulomb (= 10^9 * 10^-6 * 10^-6 = 0.001)
    private const float CoulombMultiplyFactor = 0.001f; // explanation above
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();
        
        _particles = new List<ParticleBehaviour>();
    }

    private void FixedUpdate()
    {
        if (simController.SimulationRunning)
        {
            RunSimulation();
        }
    }

    public List<ParticleBehaviour> GetParticles()
    {
        return _particles;
    }


    private void RunSimulation()
    {
        
        Debug.Log("Run Simulation");
        for (var i = 0; i < _particles.Count; ++i)
        {
            var currentParticle = _particles[i];
            var sumDirection = Vector3.zero;
            if(Mathf.Abs(currentParticle.charge) < 0.0001f || currentParticle.fixedPosition)
                continue;

            for (var j = 0; j < _particles.Count; ++j)
            {
                if(i == j || Mathf.Abs(currentParticle.charge) < 0.0001f)
                    continue;
                
                var affectingParticle = _particles[j];
                Vector3 direction;
                var r = Vector3.Distance(currentParticle.transform.position, affectingParticle.transform.position); // = distance
                
                if(r <= 1.5)
                    continue;
                
                if ((currentParticle.charge < 0f) == (affectingParticle.charge < 0f))
                {
                    //both have the same charge (both pos resp. neg) -> abstoßend
                    direction = Vector3.Normalize(currentParticle.transform.position - affectingParticle.transform.position);
                }
                else
                {
                    direction = Vector3.Normalize(affectingParticle.transform.position - currentParticle.transform.position);
                }

                var force = CoulombConstant * CoulombMultiplyFactor * Mathf.Abs(currentParticle.charge) *
                            Mathf.Abs(affectingParticle.charge);
                force /= Mathf.Pow(r, 2);
                
                if(force > 0.0001f)
                    sumDirection += force * direction;
            }

            sumDirection = Vector3.Normalize(sumDirection)* Time.deltaTime;
            currentParticle.CalculatedPosition(sumDirection + currentParticle.transform.position);
//
//            if (Mathf.Abs(sumDirection.x) < 0.0001f && Mathf.Abs(sumDirection.y) < 0.0001f &&
//                Mathf.Abs(sumDirection.z) < 0.0001f)
//            {
//                currentParticle.transform.GetComponent<Rigidbody>().isKinematic = true;
//            }
//            else {
//                currentParticle.transform.GetComponent<Rigidbody>().isKinematic = false;
//            }
            
//            Debug.Log("Particle " + i + ": charge: " + currentParticle.charge + " - force: " + sumDirection);
            
        }

        for (var i = 0; i < _particles.Count; ++i)
        {
            if (Mathf.Abs(_particles[i].charge) < 0.0001f || _particles[i].fixedPosition)
                continue;
            _particles[i].UpdateCalculations();
        }
    }


    public void AddParticle(ParticleBehaviour particle)
    {
        simController.SimulationRunning = false;
        simController.AddNewResetObjectAtBegin(particle);
        _particles.Add(particle);
        
        simController.ResetSimulation();
    }

    public void RemoveParticle(ParticleBehaviour particle, bool destroy = false)
    {
        simController.RemoveResetObject(particle);
        _particles.Remove(particle);
        
        if(destroy)
            Destroy(particle.gameObject);
    }
}
