using Maroon.Physics;
using UnityEngine;

public class CoulombChargeBehaviour : MonoBehaviour, IResetObject, IGenerateE
{
    [Header("Design Parameters and Variables")]
    [SerializeField]
    private GameObject particleBase;
    [SerializeField]
    private GameObject particleFixingRing;
    [SerializeField]
    private Material highlightMaterial;

    [Header("Particle Settings")]
    [Tooltip("Sets the charge value of the Charge. This should be a value between -10 and 10 micro Coulomb.")]

    [SerializeField]
    private QuantityFloat charge = 0.0f;
    public float Charge
    {
        get => charge;
        set => charge.Value = value;
    }

    [SerializeField]
    private float maxChargeValue = 5f;

    [SerializeField]
    private float minChargeValue = -5f;

    public float radius = 0.7022421f;
    [Tooltip("Tells whether the charge moves or has a fixed position.")]
    public bool fixedPosition = false;
    
    [Header("Movement Settings")]
    public bool pauseSimulationWhileMoving = true;
    public bool deleteIfOutsideBoundaries = true;

    private Vector3 _resetPosition;
    private Vector3 _updatePosition;
    private Rigidbody _rigidbody;
    private Renderer _particleBaseRenderer;
    private int _collided = 0;

    private CoulombLogic _coulombLogic;

    private static readonly float CoulombConstant = 9f * Mathf.Pow(10f, 9f); // 1f / (Mathf.PI * 8.8542e-12f);

    private void Awake()
    {
        _particleBaseRenderer = particleBase.GetComponent<MeshRenderer>();
        charge.onValueChanged.AddListener(OnChargeValueChangeHandler);
    }

    private void Start()
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

    private void Update()
    {
        //PC Only?
        _rigidbody.isKinematic = !SimulationController.Instance.SimulationRunning || fixedPosition;
    }

    private void OnChargeValueChangeHandler(float chargeValue)
    {
        var mat = _particleBaseRenderer.materials;
        if (Charge > 0)
        {
            mat[0].color = Color.Lerp(Teal.MinPositiveChargeColor, Teal.MaxPositiveChargeColor, chargeValue / maxChargeValue);
            mat[1] = mat[2] = highlightMaterial;
        }
        else if (chargeValue < 0)
        {
            mat[0].color = Color.Lerp(Teal.MinNegativeChargeColor, Teal.MaxNegativeChargeColor, chargeValue / minChargeValue);
            mat[2] = mat[0];
            mat[1] = highlightMaterial;
        }
        else
        {
            mat[0].color = Teal.NeutralChargeColor;
            mat[1] = mat[2] = mat[0];
        }
        _particleBaseRenderer.materials = mat;
    }

    public void SetFixedPosition(bool isPositionFixed)
    {
        fixedPosition = isPositionFixed;
        particleFixingRing.SetActive(fixedPosition);
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
        var potential = CoulombConstant * Charge * Mathf.Pow(10f, -6f) / Mathf.Pow(distance, 2f);
        return potential * dir;
    }

    public float getEFlux(Vector3 position)
    {
        throw new System.NotImplementedException();
    }

    public float getEPotential(Vector3 position)
    {
        if (Mathf.Abs(Charge) < 0.0001f) return 0f;
        var distance = _coulombLogic.WorldToCalcSpace(Vector3.Distance(transform.position, position)); //TODO: radius???
        var potential = CoulombConstant * Charge * Mathf.Pow(10f, -6f) / Mathf.Pow(distance, 2f);
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
        if (!deleteIfOutsideBoundaries) return;
        _coulombLogic.RemoveParticle(this, true);
        SimulationController.Instance.ResetSimulation();
    }
    
    public void MovementEndInsideBoundaries()
    {
        UpdateResetPosition();
        SimulationController.Instance.ResetSimulation();
    }
    
}
