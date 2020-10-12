using System;
using Maroon.Physics;
using UnityEngine;

public class CoulombChargeBehaviour : MonoBehaviour, IResetObject, IGenerateE, IDeleteObject
{
    public static readonly Color MaxPositiveChargeColor = Color.red;
    public static readonly Color MinPositiveChargeColor = new Color(1f, 0.72f, 0.72f);
    public static readonly Color MaxNegativeChargeColor = Color.blue;
    public static readonly Color MinNegativeChargeColor = new Color(0.72f, 0.72f, 1f);
    public static readonly Color NeutralChargeColor = Color.green;


    [Header("Design Parameters and Variables")]
    [SerializeField]
    private GameObject particleBase;
    [SerializeField]
    private GameObject particleFixingRing;
    [SerializeField]
    private Material highlightMaterial;

    [Header("Assessment System")]
    [SerializeField]
    [Tooltip("Sets the charge value of the Charge. This should be a value between -10 and 10 micro Coulomb.")]
    public QuantityFloat charge = 0.0f;
    public QuantityString chargeUnit = "nC";
    public QuantityBool isVisible = true;

    public float Charge
    {
        get => charge;
        set
        {
            charge.Value = value;
            chargeUnit.Value = "nC";
        }
    }

    public float ChargeUnit = Mathf.Pow(10, -9);

    [Header("Particle Settings")]
    [SerializeField]
    private float maxChargeValue = 5f;

    [SerializeField]
    private float minChargeValue = -5f;

    public float radius = 0.7022421f;
    [Tooltip("Tells whether the charge moves or has a fixed position.")]
    public QuantityBool fixedPosition = false;
    
    [Header("Movement Settings")]
    public bool pauseSimulationWhileMoving = true;
    public bool deleteIfOutsideBoundaries = true;

    private Vector3 _resetPosition;
    private Vector3 _updatePosition;
    private Rigidbody _rigidbody;
    private Renderer _particleBaseRenderer;
    private int _collided = 0;

    private CoulombLogic _coulombLogic;
    private const float CoulombConstant = 1f / (4 * Mathf.PI * 8.8542e-12f);

    //VR Specific
    public bool _inUse = false;
    public int inUseLayer = -1;
    private int _layer;
    //VR Specific End

    public RigidbodyConstraints inUseFreezing = RigidbodyConstraints.FreezeRotation;

    public bool Enabled
    {
        get => enabled;
        set => enabled = value;
    }

    private void Awake()
    {
        _particleBaseRenderer = particleBase.GetComponent<MeshRenderer>();
        charge.onValueChanged.AddListener(OnChargeValueChangeHandler);
        fixedPosition.onValueChanged.AddListener((value) => particleFixingRing.SetActive(value));
        isVisible.onValueChanged.AddListener(ChangeVisibility);
        _layer = gameObject.layer;

        Init();
    }
    
    public void Init()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.velocity = Vector3.zero;

        var obj  = GameObject.Find("CoulombLogic");
        if (obj)
            _coulombLogic = obj.GetComponent<CoulombLogic>();
        
        Debug.Assert(_coulombLogic != null);

        if (_coulombLogic.IsIn2dMode())
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezePositionZ;
        }
    }

    private void GetCoulombLogic()
    {
        var obj = GameObject.Find("CoulombLogic");
        if (obj) _coulombLogic = obj.GetComponent<CoulombLogic>();
    }

    private void Update()
    {
        //PC Only?
        if(_inUse)
            _rigidbody.isKinematic = !SimulationController.Instance.SimulationRunning || fixedPosition;
    }
    
    public void SetInUse(bool inUse)
    {
        _inUse = inUse;
        //TODO: check if needed in PC version? or if it creates a bug there
        _rigidbody.useGravity = !inUse;
        // Debug.Log("Set In USE: " + inUse);

        // _rigidbody.constraints = inUse?     RigidbodyConstraints.FreezeRotation | inUseFreezing : RigidbodyConstraints.None;
        _rigidbody.constraints = inUse? RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY |
                  RigidbodyConstraints.FreezeRotationZ
                : RigidbodyConstraints.None;

        if (inUseLayer != -1)
        {
            if (inUse)
                gameObject.layer = inUseLayer;
            else
                gameObject.layer = _layer;
        }
        
        if(!_inUse)
            _rigidbody.isKinematic = false;
    }

    public bool IsInUse()
    {
        return _inUse;
    }

    private void OnChargeValueChangeHandler(float chargeValue)
    {
        var mat = _particleBaseRenderer.materials;
        if (Charge > 0)
        {
            mat[0].color = Color.Lerp(MinPositiveChargeColor, MaxPositiveChargeColor, chargeValue / maxChargeValue);
            mat[1] = mat[2] = highlightMaterial;
        }
        else if (chargeValue < 0)
        {
            mat[0].color = Color.Lerp(MinNegativeChargeColor, MaxNegativeChargeColor, chargeValue / minChargeValue);
            mat[2] = mat[0];
            mat[1] = highlightMaterial;
        }
        else
        {
            mat[0].color = NeutralChargeColor;
            mat[1] = mat[2] = mat[0];
        }
        _particleBaseRenderer.materials = mat;
    }

    public void SetFixedPosition(bool isPositionFixed)
    {
        fixedPosition.Value = isPositionFixed;
    }

    public void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
        UpdateResetPosition();
    }

    public void UpdateResetPosition()
    {
        _resetPosition = transform.localPosition;
    }

    public void ResetObject()
    {
        transform.localPosition = _resetPosition;
    }
    
    
    public void CalculatedPosition(Vector3 calculatedPosition)
    {
        _updatePosition = calculatedPosition;
    }

    public void UpdateCalculations()
    {
        _rigidbody.velocity = Vector3.zero;
        if(_collided < 3)
            transform.position = _updatePosition;
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Particle"))
            _collided++;

    }

    private void OnCollisionExit(Collision other)
    {        
        if(other.gameObject.CompareTag("Particle"))
            _collided--;
    }

    public Vector3 getE(Vector3 position)
    {
        if (Mathf.Abs(Charge) < 0.0001f) return Vector3.zero;
        var distance = _coulombLogic.WorldToCalcSpace(Vector3.Distance(transform.position, position)); //TODO: radius???
        var dir = (position - transform.position).normalized;
        var potential = CoulombConstant * Charge * ChargeUnit / Mathf.Pow(distance, 2f);
        return potential * dir;
    }

    public float getEFlux(Vector3 position)
    {
        throw new NotImplementedException();
    }

    public float getEPotential(Vector3 position)
    {
        if (Mathf.Abs(Charge) < 0.0001f) return 0f;
        var distance = _coulombLogic.WorldToCalcSpace(Vector3.Distance(transform.position, position)); //TODO: radius???
        var potential = CoulombConstant * Charge * ChargeUnit / distance;
        return potential;
    }
    
    public float getFieldStrength()
    {
        return Charge * 3f; //as our field strength is 3 -> a 1 Coulomb Charge would feel 3, a 2 Coulomb Charge 6 and a -1 Charge -3
    }

    public void MovementStart()
    {
        if (pauseSimulationWhileMoving) SimulationController.Instance.SimulationRunning = false;
    }

    public void MovementEndOutsideBoundaries()
    {
        Debug.Log("End outside Boundary");
        if (!deleteIfOutsideBoundaries) return;
        _coulombLogic.RemoveParticle(this, true);
        SimulationController.Instance.ResetSimulation();
    }
    
    public void MovementEndInsideBoundaries()
    {
        UpdateResetPosition();
        SimulationController.Instance.ResetSimulation();
    }
    
    public void SetCharge(float charge, Color chargeColor)
    {
        this.charge = charge;
        if (chargeColor.a > 0f)
        {
            particleBase.GetComponent<MeshRenderer>().materials[0].color = chargeColor;
        }
        
        OnChargeValueChangeHandler(charge);
    }

    public void OnDeleteObject()
    {
        _coulombLogic.RemoveParticle(this, true);
    }

    public IQuantity GetCharge()
    {
        return charge;
    }

    public IQuantity GetFixedPosition()
    {
        return fixedPosition;
    }

    protected void ChangeVisibility(bool setVisible)
    {
        foreach (var r in GetComponents<Renderer>()) r.enabled = setVisible;
        foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = setVisible;
        
        foreach (var c in GetComponents<Collider>()) c.enabled = setVisible;
        foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = setVisible;
    }
}
