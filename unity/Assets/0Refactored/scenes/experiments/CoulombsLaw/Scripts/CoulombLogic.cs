using System;
using System.Collections.Generic;
using Maroon.Physics.Electromagnetism;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

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
    public enum Axis {
        XAxis,
        YAxis,
        ZAxis
    }
    
    [Header("General Settings")] 
    public bool inVR = false;
    public int maxChargeCount = 10;
    public bool startIn2dMode = true;
    public Axis restrict2dModeMovement = Axis.ZAxis;

    public ParticleEvent onParticleAdded;
    public ParticleEvent onParticleRemoved;
    public UnityEvent onMaxChargesReached;
    public UnityEvent onUnderMaxChargesAgain;

    [Header("Calculation Settings")] 
    public Transform xOrigin2d;
    public Transform xAt1m2d;
    public Transform xOrigin3d;
    public Transform xAt1m3d;

    [Header("Axis Settings")] 
    public Axis calcSpaceXAxis = Axis.XAxis;
    public Axis calcSpaceYAxis = Axis.YAxis;
    public Axis calcSpaceZAxis = Axis.ZAxis;
    
    [Header("Maximum Ranges")]
    public float xMax2d = 1f;
    public float yMax2d = 1.35f;
    public float xMax3d = 1f;
    public float yMax3d = 1f;
    public float zMax3d = 1f;
    
    [Header("2D-3D Mode depending Settings")]
    public GameObject scene2D;
    public GameObject scene3D;

    public VectorField vectorField2d;
    public VectorField vectorField3d;

    public GameObject minBoundary2d;
    public GameObject maxBoundary2d;
    public GameObject minBoundary3d;
    public GameObject maxBoundary3d;
    
    public ModeChangeEvent onModeChange;
    public UnityEvent onModeChangeTo2d;
    public UnityEvent onModeChangeTo3d;

    private List<CoulombChargeBehaviour> _charges = new List<CoulombChargeBehaviour>();
    private HashSet<GameObject> _chargesGameObjects = new HashSet<GameObject>();

    private const float CoulombConstant = 1f / (Mathf.PI * 8.8542e-12f);
//    private const float CoulombConstant = 9f; // = 9 * 10^9 -> but we use the factor 0.001 beneath because we have constant * microCoulomb * microCoulomb (= 10^9 * 10^-6 * 10^-6 = 0.001)
//    private const float CoulombMultiplyFactor = 0.001f; // explanation above
    
    private bool _in3dMode = false;

    private float _worldToCalcSpaceFactor2d;
    private float _worldToCalcSpaceFactor2dLocal;
    private float _worldToCalcSpaceFactor3d;
    private float _worldToCalcSpaceFactor3dLocal;
    
    private bool _initialized = false;

    [Header("Debug Stuff")] 
    public float correctionFactor = 1f;


    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }
    
    private void Initialize(){
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
        
        if (SimulationController.Instance.SimulationRunning)
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

    public Vector3 CalcToWorldSpace(Vector3 distance, bool local = false)
    {
        return new Vector3(CalcToWorldSpace(distance.x, local), CalcToWorldSpace(distance.y, local),
            CalcToWorldSpace(distance.z, local));
    }

    public Vector3 CalcToWorldSpaceCoordinates(Vector3 coord, bool local = true)
    {
        if (local)
        {
            var origin = Vector3.zero;
            var at1m = Vector3.zero;
            if(IsIn2dMode())
            {
                var tmp = coord.y;
                coord.y = coord.z;
                coord.z = tmp;
                origin = xOrigin2d.transform.localPosition;
                at1m = xAt1m2d.transform.localPosition;
            }
            else
            {
                origin = xOrigin3d.transform.localPosition;
                at1m = xAt1m3d.transform.localPosition;
            }
            
              
            return new Vector3(
                origin.x + (at1m.x - origin.x) * coord.x,
                origin.y + (at1m.y - origin.y) * coord.y,
                origin.z + (at1m.z - origin.z) * coord.z);
        }

        throw new NotImplementedException();
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
            // Debug.Log("Charge as Vec: " + new Vector4(pos.x, pos.y, pos.z, charge.Charge));
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
                
                var r = WorldToCalcSpace(Vector3.Distance(currentParticle.transform.position, affectingParticle.transform.position)); // = distance
                r -= 2 * WorldToCalcSpace(0.71f); // - 2 * radius
                
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
                force /= Mathf.Pow(r, 2);
                
                sumForce += force * direction2;

                if(force > 0.0001f)
                    sumDirection += force * direction;
            }

            sumDirection = Vector3.Normalize(sumDirection)* Time.deltaTime;
            sumDirection = CalcToWorldSpace(sumDirection);

            if (Mathf.Abs(correctionFactor - 1f) > 0.0000000001f)
            {
                sumDirection.x *= correctionFactor;
                sumDirection.y *= correctionFactor;
                sumDirection.z *= correctionFactor;
            }
            
            currentParticle.CalculatedPosition(sumDirection + currentParticle.transform.position);
        }

        for (var i = 0; i < _charges.Count; ++i)
        {
            if (Mathf.Abs(_charges[i].Charge) < 0.0001f || _charges[i].fixedPosition)
                continue;
            _charges[i].UpdateCalculations();
        }
    }


    public GameObject CreateChargeAtCalcSpacePosition(GameObject prefab, Vector3 position, float chargeLoad, bool hasFixedPosition, bool deactivateCollisions = true)
    {
        Debug.Log("Create Charge with " + chargeLoad + " at " + position + " using " + (hasFixedPosition? "a" : "no") + " fixed position");
        var localPos = CalcToWorldSpaceCoordinates(position, true);
        var newCharge = CreateCharge(prefab, localPos, chargeLoad, hasFixedPosition, false, deactivateCollisions);
        newCharge.transform.localPosition = localPos;
        newCharge.GetComponent<CoulombChargeBehaviour>()?.UpdateResetPosition();

        var rb = newCharge.GetComponent<Rigidbody>();
        if (rb && !_in3dMode && !inVR)
        {
            rb.constraints = ( restrict2dModeMovement == Axis.XAxis? RigidbodyConstraints.FreezePositionX 
                                 : restrict2dModeMovement == Axis.YAxis? RigidbodyConstraints.FreezePositionY : RigidbodyConstraints.FreezePositionZ) 
                             | RigidbodyConstraints.FreezeRotation;
        }
        
        return newCharge;
    }
    
    public GameObject CreateCharge(GameObject prefab, Vector3 position, float chargeLoad, bool hasFixedPosition, bool positionInWorldCoord = true, bool deactivateCollisions = true)
    {
        var obj = Instantiate(prefab, _in3dMode ? vectorField3d.transform : vectorField2d.transform, true);
        Debug.Assert(obj != null);

        var chargeBehaviour = obj.GetComponent<CoulombChargeBehaviour>();
        Debug.Assert(chargeBehaviour != null);
        if(positionInWorldCoord) chargeBehaviour.SetPosition(position);
        else
        {
            if (_in3dMode)
                obj.transform.localPosition = xOrigin3d.localPosition + CalcToWorldSpace(position, true);
            else
            {
                var pos = xOrigin2d.position + CalcToWorldSpace(new Vector3(position.x, position.y));
                obj.transform.position = pos;
            } 
            chargeBehaviour.SetPosition(obj.transform.position);
        }
        chargeBehaviour.Charge = chargeLoad;
        chargeBehaviour.SetFixedPosition(hasFixedPosition);
        
        if(!inVR)
            chargeBehaviour.Init();

        var movement = obj.GetComponent<PC_DragHandler>();
        if (!movement) movement = obj.GetComponentInChildren<PC_DragHandler>();
        if (movement)
        {
            movement.SetBoundaries(_in3dMode ? minBoundary3d : minBoundary2d,
                _in3dMode ? maxBoundary3d : maxBoundary2d);
            movement.allowedXMovement = _in3dMode || restrict2dModeMovement != Axis.XAxis;
            movement.allowedYMovement = _in3dMode || restrict2dModeMovement != Axis.YAxis;
            movement.allowedZMovement = _in3dMode || restrict2dModeMovement != Axis.ZAxis;
            // movement.allowedXMovement = movement.allowedYMovement = true;
            // movement.allowedZMovement = _in3dMode;
        }
        
        var arrowMovement = obj.GetComponentInChildren<PC_ArrowMovement>();
        if (arrowMovement)
        {
            arrowMovement.SetBoundaries(_in3dMode ? minBoundary3d.transform : minBoundary2d.transform,
                _in3dMode ? maxBoundary3d.transform : maxBoundary2d.transform);
            arrowMovement.restrictYMovement = !_in3dMode;
        }

        var field = GameObject.FindGameObjectWithTag("Field").GetComponent<IField>(); //should be only one
        var fieldLine = obj.GetComponentInChildren<FieldLine>();
        if (field && fieldLine)
        {
            fieldLine.field = field;
        }
        obj.transform.localRotation = Quaternion.identity;
        obj.SetActive(true);

        AddParticle(chargeBehaviour, deactivateCollisions);
        // if(!_in3dMode)
            // chargeBehaviour.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        return obj;
    }
    

    public void AddParticle(CoulombChargeBehaviour coulombCharge, bool deactivateCollisions = true)
    {
        if(_charges.Count >= maxChargeCount) return;
        
        SimulationController.Instance.SimulationRunning = false;
        SimulationController.Instance.AddNewResetObjectAtBegin(coulombCharge);
        _charges.Add(coulombCharge);
        _chargesGameObjects.Add(coulombCharge.gameObject);
        coulombCharge.SetInUse(true);

        if (deactivateCollisions)
        {
            foreach (var collider in coulombCharge.gameObject.GetComponents<Collider>())
            {
                Physics.IgnoreCollision(collider, vectorField3d.gameObject.GetComponent<Collider>());
                Physics.IgnoreCollision(collider, vectorField2d.gameObject.GetComponent<Collider>());
            }

            foreach (var collider in coulombCharge.gameObject.GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(collider, vectorField3d.gameObject.GetComponent<Collider>());
                Physics.IgnoreCollision(collider, vectorField2d.gameObject.GetComponent<Collider>());
            }
        }

        SimulationController.Instance.ResetSimulation();
        
        if(_charges.Count == maxChargeCount) onMaxChargesReached.Invoke();
        
        onParticleAdded.Invoke(coulombCharge);
    }

    public bool ContainsParticle(CoulombChargeBehaviour coulombCharge)
    {
        return _charges.Contains(coulombCharge);
    }

    public void RemoveParticle(CoulombChargeBehaviour coulombCharge, bool destroy = false)
    {
        SimulationController.Instance.RemoveResetObject(coulombCharge);
        _charges.Remove(coulombCharge);
        _chargesGameObjects.Remove(coulombCharge.gameObject);
        coulombCharge.SetInUse(false);

        if (destroy)
        {
            coulombCharge.gameObject.SetActive(false);
            Destroy(coulombCharge.gameObject);
        }
        
        if(_charges.Count == maxChargeCount - 1) onUnderMaxChargesAgain.Invoke();

        onParticleRemoved.Invoke(coulombCharge);
    }

    public void RemoveAllParticles(bool destroy = false)
    {
        while (_charges.Count > 0)
        {
            RemoveParticle(_charges[0], destroy);
        }
    }


    public void OnSwitch3d2dMode(float newMode)
    {
        _in3dMode = !(newMode < 0.5);
        onModeChange.Invoke(_in3dMode);
        if(_in3dMode)
            onModeChangeTo3d.Invoke();
        else
            onModeChangeTo2d.Invoke();
        
        SimulationController.Instance.SimulationRunning = false;
        
        //remove all particles show new scene
        while(_charges.Count > 0)
            RemoveParticle(_charges[0], true);

        scene2D.SetActive(!_in3dMode);
        scene3D.SetActive(_in3dMode);

        if (!inVR && !SceneManager.GetActiveScene().name.Contains(".vr"))
        {
            var camTransform = Camera.main.transform;
            camTransform.position = _in3dMode ? new Vector3(0, 30f, -59.52f) : new Vector3(0, 4.4f, -59.52f);
            camTransform.rotation = _in3dMode ? new Quaternion(0.25f, 0f, 0f, 1f) : new Quaternion(0f, 0f, 0f, 0f);
        }

        vectorField3d.setVectorFieldVisible(_in3dMode);
        vectorField2d.setVectorFieldVisible(!_in3dMode);
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
