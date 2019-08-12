using UnityEngine;

public class CoulombChargeBehaviour : MonoBehaviour, IResetObject, IGenerateE
{
    [Header("Design Parameters and Variables")]
    public GameObject particleBase;
    public GameObject particleFixingRing;
    public Material highlightMaterial;
    
    [Header("Particle Settings")]
    [Tooltip("Sets the charge value of the Charge. This should be a value between -10 and 10 micro Coulomb.")]
    [Range(-10f, 10f)] public float charge;
    public float radius = 0.7022421f;
    [Tooltip("Tells whether the charge moves or has a fixed position.")]
    public bool fixedPosition = false;
//
//    [Header("Boundaries")] 
//    public Transform minPosition;
//    public Transform maxPosition;

    private int _currentCharge = -1; // -1 = electron, 0 = neutron, 1 = proton -> needed to adapt the material during runtime
    private bool _inUse = false;

    private Vector3 _resetPosition;
    private Vector3 _updatePosition;
    private Rigidbody _rigidbody;
    private int _collided = 0;

    private CoulombLogic _coulombLogic;
    
    private static readonly float CoulombConstant = 9f * Mathf.Pow(10f, 9f); // 1f / (Mathf.PI * 8.8542e-12f);

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _currentCharge = charge < 0 ? -1 : charge > 0 ? 1 : 0;
        ChangeParticleType();
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        var obj  = GameObject.Find("CoulombLogic");
        if (obj)
            _coulombLogic = obj.GetComponent<CoulombLogic>();
        Debug.Assert(_coulombLogic != null);
    }

    private void ChangeParticleType()
    {
        var mat = particleBase.GetComponent<MeshRenderer>().materials;
        switch (_currentCharge)
        {
            case 0:
                mat[1] = mat[2] = mat[0];
                break;
            case -1:
                mat[2] = mat[0];
                mat[1] = highlightMaterial;
                break;
            case 1:
                mat[1] = mat[2] = highlightMaterial;
                break;
        }
        
        particleBase.GetComponent<MeshRenderer>().materials = mat;
    }
    
    public void SetCharge(float charge, Color chargeColor)
    {
        this.charge = charge;
        if (chargeColor.a > 0f)
        {
            particleBase.GetComponent<MeshRenderer>().materials[0].color = chargeColor;
        }

        _currentCharge = this.charge < 0 ? -1 : this.charge > 0 ? 1 : 0;
        ChangeParticleType();
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
        _resetPosition = transform.position;
    }

    public void ResetObject()
    {
        transform.position = _resetPosition;
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
        if (Mathf.Abs(charge) < 0.0001f) return Vector3.zero;
//
//        var Range = minPosition.position - maxPosition.position;
//        var inputPos = (position - minPosition.position);
//        inputPos.x /= Range.x;
//        inputPos.y /= Range.y;
//        inputPos.z /= Range.z;
//
//        var currentPos = (transform.position - minPosition.position);
//        currentPos.x /= Range.x;
//        currentPos.y /= Range.y;
//        currentPos.z /= Range.z;

        //        var distance = Vector3.Distance(currentPos, inputPos); //TODO - radius
//        var retVal = CoulombConstant * charge / distance) *dir;
//        return (CoulombConstant * charge / distance) * dir;

//        var Range = minPosition.position - maxPosition.position;
//        Range.x = Mathf.Abs(Range.x);
//        Range.y = Mathf.Abs(Range.y);
//        Range.z = Mathf.Abs(Range.z);
//
//        var inputPos = (position - minPosition.position);
//        inputPos.x = Range.x < 0.0001f? 0f : inputPos.x / Range.x;
//        inputPos.y = Range.y < 0.0001f? 0f : inputPos.y / Range.y;
//        inputPos.z = Range.z < 0.0001f? 0f : inputPos.z / Range.z;
//
//        var currentPos = (transform.position - minPosition.position);
//        currentPos.x = Range.x < 0.0001f? 0f : currentPos.x / Range.x;
//        currentPos.y = Range.y < 0.0001f? 0f : currentPos.y / Range.y;
//        currentPos.z = Range.z < 0.0001f? 0f : currentPos.z / Range.z;
        
        var distance = _coulombLogic.WorldToCalcSpace(Vector3.Distance(transform.position, position)); //TODO: radius???
        
        var dir = (position - transform.position).normalized;
        //TODO: consider 3d as well
        var potential = CoulombConstant * charge * Mathf.Pow(10f, -6f) / Mathf.Pow(distance, 2f);
        return potential * dir;

//        var dir = (position - transform.position).normalized;
//        var distance = Vector3.Distance(transform.position, position) - radius;
//        return (CoulombConstant * charge / distance) * dir;
    }

    public float getEFlux(Vector3 position)
    {
        throw new System.NotImplementedException();
    }

    public float getEPotential(Vector3 position)
    {
        if (Mathf.Abs(charge) < 0.0001f) return 0f;
        var distance = _coulombLogic.WorldToCalcSpace(Vector3.Distance(transform.position, position)); //TODO: radius???
        var potential = CoulombConstant * charge * Mathf.Pow(10f, -6f) / Mathf.Pow(distance, 2f);
        return potential;
    }
    
    public float getFieldStrength()
    {
        throw new System.NotImplementedException();
    }

}
