using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ModeChangeEvent : UnityEvent<bool>
{
}

public class CoulombLogic : MonoBehaviour, IResetWholeObject
{
    [Header("2D-3D Mode depending Settings")]
    public GameObject scene2D;
    public GameObject scene3D;
    
    public ModeChangeEvent onModeChange;
    
    private SimulationController _simController;
    private List<ParticleBehaviour> _particles;
    private HashSet<GameObject> _particlesGameObjects;

    private const float CoulombConstant = 9f; // = 9 * 10^9 -> but we use the factor 0.001 beneath because we have constant * microCoulomb * microCoulomb (= 10^9 * 10^-6 * 10^-6 = 0.001)
    private const float CoulombMultiplyFactor = 0.001f; // explanation above

    private VectorField _vectorField2d;
    private VectorField3d _vectorField3d;
    
    private bool _in3dMode = false;

    
    // Start is called before the first frame update
    void Start()
    {
        var simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            _simController = simControllerObject.GetComponent<SimulationController>();
        
        _vectorField3d = scene3D.GetComponentInChildren<VectorField3d>();
        _vectorField2d = scene2D.GetComponentInChildren<VectorField>();
        
        _particles = new List<ParticleBehaviour>();
        _particlesGameObjects = new HashSet<GameObject>();
        OnSwitch3d2dMode(_in3dMode? 1f : 0f);
    }

    private void FixedUpdate()
    {
        if (_simController.SimulationRunning)
        {
            RunSimulation();
        }
    }

    public List<ParticleBehaviour> GetParticles()
    {
        return _particles;
    }

    public HashSet<GameObject> GetChargeGameObjects()
    {
        return _particlesGameObjects;
    }


    private void RunSimulation()
    {
        for (var i = 0; i < _particles.Count; ++i)
        {
            var currentParticle = _particles[i];
            var sumDirection = Vector3.zero;
            if(Mathf.Abs(currentParticle.charge) < 0.0001f || currentParticle.fixedPosition)
                continue;

            var sumForce = Vector3.zero;
            for (var j = 0; j < _particles.Count; ++j)
            {
                if(i == j || Mathf.Abs(currentParticle.charge) < 0.0001f)
                    continue;
                
                var affectingParticle = _particles[j];
                Vector3 direction;
                Vector3 direction2;
                var r = Vector3.Distance(currentParticle.transform.position, affectingParticle.transform.position); // = distance
                r -= 2 * 0.71f; // - 2 * radius
//                if(r <= 1.5)
//                    continue;
                
                if ((currentParticle.charge < 0f) == (affectingParticle.charge < 0f))
                {
                    //both have the same charge (both pos resp. neg) -> abstoßend
                    direction = Vector3.Normalize(currentParticle.transform.position - affectingParticle.transform.position);
                    direction2 = (currentParticle.transform.position - affectingParticle.transform.position);
                }
                else
                {
                    direction = Vector3.Normalize(affectingParticle.transform.position - currentParticle.transform.position);
                    direction2 = (affectingParticle.transform.position - currentParticle.transform.position);
                }

                var force = CoulombConstant * CoulombMultiplyFactor * Mathf.Abs(currentParticle.charge) *
                            Mathf.Abs(affectingParticle.charge);
                force /= Mathf.Pow(r, 2);

                sumForce += force * direction2;
                
                if(force > 0.0001f)
                    sumDirection += force * direction;
            }

//            Debug.Log("Dir: " + sumForce);
            
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
        _simController.SimulationRunning = false;
        _simController.AddNewResetObjectAtBegin(particle);
        _particles.Add(particle);
        _particlesGameObjects.Add(particle.gameObject);
        //TODO: check if this works
        foreach (var collider in particle.gameObject.GetComponents<Collider>())
        {
            Physics.IgnoreCollision(collider, _vectorField3d.gameObject.GetComponent<Collider>());
            Physics.IgnoreCollision(collider, _vectorField2d.gameObject.GetComponent<Collider>());
        }
        foreach (var collider in particle.gameObject.GetComponentsInChildren<Collider>())
        {
            Physics.IgnoreCollision(collider, _vectorField3d.gameObject.GetComponent<Collider>());
            Physics.IgnoreCollision(collider, _vectorField2d.gameObject.GetComponent<Collider>());
        }
        
        _simController.ResetSimulation();
    }

    public void RemoveParticle(ParticleBehaviour particle, bool destroy = false)
    {
        _simController.RemoveResetObject(particle);
        _particles.Remove(particle);
        _particlesGameObjects.Remove(particle.gameObject);

        if (destroy)
        {
            particle.gameObject.SetActive(false);
            Destroy(particle.gameObject);
        }
    }


    public void OnSwitch3d2dMode(float newMode)
    {
        _in3dMode = !(newMode < 0.5);
        onModeChange.Invoke(_in3dMode);
        
        _simController.SimulationRunning = false;
        
        //remove all particles show new scene
        while(_particles.Count > 0)
            RemoveParticle(_particles[0], true);

        scene2D.SetActive(!_in3dMode);
        scene3D.SetActive(_in3dMode);

        Camera.main.transform.position = _in3dMode ? new Vector3(0, 30f, -59.52f) : new Vector3(0, 4.4f, -59.52f);
        Camera.main.transform.rotation = _in3dMode ? new Quaternion(0.25f, 0f, 0f, 1f) : new Quaternion(0f, 0f, 0f, 0f);

        _vectorField3d.setVectorFieldVisible(_in3dMode);
        _vectorField2d.setVectorFieldVisible(!_in3dMode);
    }

    public void ResetObject()
    {
    }

    public void ResetWholeObject()
    {        
        //remove all particles show new scene
        while(_particles.Count > 0)
            RemoveParticle(_particles[0], true);
    }
}
