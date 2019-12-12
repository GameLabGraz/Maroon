using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ModeChangeEvent : UnityEvent<bool>
{
}

[System.Serializable]
public class ParticleEvent : UnityEvent<CoulombChargeBehaviour>
{
}


public class CoulombLogic : MonoBehaviour, IResetWholeObject
{
    [Header("General Settings")] 
    public int maxChargeCount = 10;
    public bool startIn2dMode = true;

    public ParticleEvent onParticleAdded;
    public ParticleEvent onParticleRemoved;
    public UnityEvent onMaxChargesReached;
    public UnityEvent onUnderMaxChargesAgain;

    [Header("Calculation Settings")] 
    public Transform xOrigin2d;
    public Transform xAt1m2d;
    public Transform xOrigin3d;
    public Transform xAt1m3d;
    
    [Header("Maximum Ranges")]
    public float xMax2d = 1f;
    public float yMax2d = 1.35f;
    public float xMax3d = 1f;
    public float yMax3d = 1f;
    public float zMax3d = 1f;
    
    [Header("2D-3D Mode depending Settings")]
    public GameObject scene2D;
    public GameObject scene3D;

    public GameObject minBoundary2d;
    public GameObject maxBoundary2d;
    public GameObject minBoundary3d;
    public GameObject maxBoundary3d;
    
    public ModeChangeEvent onModeChange;

    private SimulationController _simController;
    private List<CoulombChargeBehaviour> _charges;
    private HashSet<GameObject> _chargesGameObjects;

    private const float CoulombConstant = 1f / (Mathf.PI * 8.8542e-12f);
//    private const float CoulombConstant = 9f; // = 9 * 10^9 -> but we use the factor 0.001 beneath because we have constant * microCoulomb * microCoulomb (= 10^9 * 10^-6 * 10^-6 = 0.001)
//    private const float CoulombMultiplyFactor = 0.001f; // explanation above

    private VectorField _vectorField2d;
    private VectorField3d _vectorField3d;
    
    private bool _in3dMode = false;

    private float _worldToCalcSpaceFactor2d;
    private float _worldToCalcSpaceFactor2dLocal;
    private float _worldToCalcSpaceFactor3d;
    private float _worldToCalcSpaceFactor3dLocal;
    
    private bool _initialized = false;
    
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }
    
    private void Initialize(){
        var simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            _simController = simControllerObject.GetComponent<SimulationController>();
        
        _vectorField3d = scene3D.GetComponentInChildren<VectorField3d>();
        _vectorField2d = scene2D.GetComponentInChildren<VectorField>();
        
        _charges = new List<CoulombChargeBehaviour>();
        _chargesGameObjects = new HashSet<GameObject>();
        OnSwitch3d2dMode(startIn2dMode? 0f : 1f);
    
        _worldToCalcSpaceFactor2d = Mathf.Abs(xAt1m2d.position.x - xOrigin2d.position.x);
        _worldToCalcSpaceFactor2dLocal = Mathf.Abs(xAt1m2d.localPosition.x - xOrigin2d.localPosition.x);
        _worldToCalcSpaceFactor3d = Mathf.Abs(xAt1m3d.position.x - xOrigin3d.position.x);
        _worldToCalcSpaceFactor3dLocal = Mathf.Abs(xAt1m3d.localPosition.x - xOrigin3d.localPosition.x);
        _initialized = true;
    }

    private void FixedUpdate()
    {
        if (!_initialized)
            Initialize();
        
        if (_simController.SimulationRunning)
        {
            RunSimulation();
        }
    }

    public float WorldToCalcSpace(float distanceWorldSpace, bool local = false)
    {
        if (IsIn2dMode())
            return distanceWorldSpace / (local? _worldToCalcSpaceFactor2dLocal : _worldToCalcSpaceFactor2d);
        return distanceWorldSpace / (local? _worldToCalcSpaceFactor3dLocal : _worldToCalcSpaceFactor3d);
    }

    public float CalcToWorldSpace(float distanceCalcSpace, bool local = false)
    {
        if (IsIn2dMode())
            return distanceCalcSpace * (local? _worldToCalcSpaceFactor2dLocal : _worldToCalcSpaceFactor2d);
        return distanceCalcSpace * (local? _worldToCalcSpaceFactor3dLocal : _worldToCalcSpaceFactor3d);
    }
    
    public List<CoulombChargeBehaviour> GetCharges()
    {
        return _charges;
    }
    
    public List<Vector4> GetChargesAsVector()
    {
        var list = new List<Vector4>();
        foreach (var charge in _charges)
        {
            var pos = charge.transform.position;
            list.Add(new Vector4(pos.x, pos.y, pos.z, charge.Charge));
            Debug.Log("Charge as Vec: " + new Vector4(pos.x, pos.y, pos.z, charge.Charge));
        }

        return list;
    }

    public bool IsIn3dMode() { return _in3dMode; }
    public bool IsIn2dMode() { return !_in3dMode; }
    public int GetMaxChargesCount() { return maxChargeCount; }
    
    public HashSet<GameObject> GetChargeGameObjects()
    {
        return _chargesGameObjects;
    }


    private void RunSimulation()
    {
        for (var i = 0; i < _charges.Count; ++i)
        {
            var currentParticle = _charges[i];
            var sumDirection = Vector3.zero;
            if(Mathf.Abs(currentParticle.Charge) < 0.0001f || currentParticle.fixedPosition)
                continue;

            var sumForce = Vector3.zero;
            for (var j = 0; j < _charges.Count; ++j)
            {
                if(i == j || Mathf.Abs(currentParticle.Charge) < 0.0001f)
                    continue;
                
                var affectingParticle = _charges[j];
                Vector3 direction;
                Vector3 direction2;
                var r = Vector3.Distance(currentParticle.transform.position, affectingParticle.transform.position); // = distance
                r -= 2 * 0.71f; // - 2 * radius
//                if(r <= 1.5)
//                    continue;
                
                if ((currentParticle.Charge < 0f) == (affectingParticle.Charge < 0f))
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

                var force = CoulombConstant * Mathf.Abs(currentParticle.Charge) * Mathf.Abs(affectingParticle.Charge);
//                var force = CoulombConstant * CoulombMultiplyFactor * Mathf.Abs(currentParticle.charge) * Mathf.Abs(affectingParticle.charge);
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

        for (var i = 0; i < _charges.Count; ++i)
        {
            if (Mathf.Abs(_charges[i].Charge) < 0.0001f || _charges[i].fixedPosition)
                continue;
            _charges[i].UpdateCalculations();
        }
    }


    public void AddParticle(CoulombChargeBehaviour coulombCharge)
    {
        if(_charges.Count >= maxChargeCount) return;
        
        _simController.SimulationRunning = false;
        _simController.AddNewResetObjectAtBegin(coulombCharge);
        _charges.Add(coulombCharge);
        _chargesGameObjects.Add(coulombCharge.gameObject);

        foreach (var collider in coulombCharge.gameObject.GetComponents<Collider>())
        {
            Physics.IgnoreCollision(collider, _vectorField3d.gameObject.GetComponent<Collider>());
            Physics.IgnoreCollision(collider, _vectorField2d.gameObject.GetComponent<Collider>());
        }
        foreach (var collider in coulombCharge.gameObject.GetComponentsInChildren<Collider>())
        {
            Physics.IgnoreCollision(collider, _vectorField3d.gameObject.GetComponent<Collider>());
            Physics.IgnoreCollision(collider, _vectorField2d.gameObject.GetComponent<Collider>());
        }
        
        _simController.ResetSimulation();
        
        if(_charges.Count == maxChargeCount) onMaxChargesReached.Invoke();
        
        onParticleAdded.Invoke(coulombCharge);
    }

    public void RemoveParticle(CoulombChargeBehaviour coulombCharge, bool destroy = false)
    {
        _simController.RemoveResetObject(coulombCharge);
        _charges.Remove(coulombCharge);
        _chargesGameObjects.Remove(coulombCharge.gameObject);

        if (destroy)
        {
            coulombCharge.gameObject.SetActive(false);
            Destroy(coulombCharge.gameObject);
        }
        
        if(_charges.Count == maxChargeCount - 1) onUnderMaxChargesAgain.Invoke();

        onParticleRemoved.Invoke(coulombCharge);
    }


    public void OnSwitch3d2dMode(float newMode)
    {
        _in3dMode = !(newMode < 0.5);
        onModeChange.Invoke(_in3dMode);
        
        _simController.SimulationRunning = false;
        
        //remove all particles show new scene
        while(_charges.Count > 0)
            RemoveParticle(_charges[0], true);

        scene2D.SetActive(!_in3dMode);
        scene3D.SetActive(_in3dMode);

        Debug.Log("Switch Camera");
        
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
        while(_charges.Count > 0)
            RemoveParticle(_charges[0], true);
    }
    
    public Vector3 GetMinimumPos()
    {
        return IsIn2dMode() ? minBoundary2d.transform.position : minBoundary3d.transform.position;
    }

    public Vector3 GetMaximumPos()
    {
        return IsIn2dMode() ? maxBoundary2d.transform.position : maxBoundary3d.transform.position;
    }
}
