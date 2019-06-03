using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;

public class ParticleBehaviour : MonoBehaviour, IResetObject, IGenerateB
{
    [Header("Design Parameters and Variables")]
    public GameObject particleBase;
    public GameObject particleFixingRing;
    public Material highlightMaterial;
    
    [Header("Particle Settings")]
    [Range(-10f, 10f)] public float charge;
    public bool fixedPosition = false;
    
    private const float Tolerance = 0.001f;
    private int _currentCharge = -1; // -1 = electron, 0 = neutron, 1 = proton -> needed to adapt the material during runtime
    private bool _inUse = false;

    private Vector3 _resetPosition;
    private Vector3 _updatePosition;

    private const float CoulombConstant = 9f; // = 9 * 10^9 -> but we use the factor 0.001 beneath because we have constant * microCoulomb * microCoulomb (= 10^9 * 10^-6 * 10^-6 = 0.001)
    private const float CoulombMultiplyFactor = 0.001f; // explanation above

    // Start is called before the first frame update
    void Start()
    {
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
        _resetPosition = newPosition;
        transform.position = newPosition;
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
        transform.position = _updatePosition;
    }

    public Vector3 getB(Vector3 position)
    {
        if (Mathf.Abs(charge) < 0.0001f)
        {
            return Vector3.zero;
        }

        var transPos = transform.position;
        var distance = Mathf.Pow(Vector3.Distance(position, transPos), 2);
        var dir = (transPos - position).normalized;

        return (CoulombConstant  * charge / distance) * dir;
    }

    public float getFieldStrength()
    {
        Debug.Log("GET FIELD STRENGHT????");
        return 0f;
    }

}
