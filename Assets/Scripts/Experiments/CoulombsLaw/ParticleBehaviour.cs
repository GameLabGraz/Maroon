using UnityEngine;

public class ParticleBehaviour : MonoBehaviour, IResetObject, IGenerateE
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
    
    private int _currentCharge = -1; // -1 = electron, 0 = neutron, 1 = proton -> needed to adapt the material during runtime
    private bool _inUse = false;

    private Vector3 _resetPosition;
    private Vector3 _updatePosition;
    private Rigidbody _rigidbody;
    private int _collided = 0;
    
    private const float CoulombConstant = 1f / (Mathf.PI * 8.8542e-12f);
//    private const float CoulombConstant = 9f; // = 9 * 10^9 -> but we use the factor 0.001 beneath because we have constant * microCoulomb * microCoulomb (= 10^9 * 10^-6 * 10^-6 = 0.001)
//    private const float CoulombMultiplyFactor = 0.001f; // explanation above

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _currentCharge = charge < 0 ? -1 : charge > 0 ? 1 : 0;
        ChangeParticleType();
        GetComponent<Rigidbody>().velocity = Vector3.zero;
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

    //TODO: remove
//    public Vector3 getB(Vector3 position)
//    {
//        if (Mathf.Abs(charge) < 0.0001f)
//        {
//            return Vector3.zero;
//        }
//
//        var transPos = transform.position;
//        var distance = Mathf.Pow(Vector3.Distance(position, transPos), 2);
//        var dir = (transPos - position).normalized;
//
//        return (CoulombConstant  * charge / distance) * dir;
//    }

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
        
        var dir = (position - transform.position).normalized;
        var distance = Vector3.Distance(transform.position, position) - radius;
        return (CoulombConstant * charge / distance) * dir;
    }

    public float getEFlux(Vector3 position)
    {
        throw new System.NotImplementedException();
    }

    public float getEPotential(Vector3 position)
    {
        throw new System.NotImplementedException();
    }

    public float getFieldStrength()
    {
        throw new System.NotImplementedException();
    }

}
