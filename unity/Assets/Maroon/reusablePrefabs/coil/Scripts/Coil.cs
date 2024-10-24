//-----------------------------------------------------------------------------
// Coil.cs
//
// Class for a live coil.
// Is also a electromagnetic object.  
//-----------------------------------------------------------------------------
//

using UnityEngine;

/// <summary>
/// Class for a live coil.
/// Is also a electromagnetic object. 
/// </summary>
public class Coil : EMObject, IResetObject
{
    /// <summary>
    /// The magnetic field
    /// </summary>
    [SerializeField] private BField field;

    /// <summary>
    /// The coil diameter
    /// </summary>
    [SerializeField] private float diameter = 0.33f;
    public float Diameter
    {
        get { return diameter; }
        set { 
            diameter = value;
            Radius = diameter / 2f;
        }
    }

    /// <summary>
    /// The coil radius
    /// </summary>
    public float Radius { 
        get; 
        private set; 
    }

    /// <summary>
    /// The radius of the cross section of the wire that makes up the coil
    /// </summary>
    private float crossSectionRadius = 0.05f;

    /// <summary>
    /// Conductivity of the material of the coil
    /// </summary>
    public float Conductivity { get; set; } = 57f; // Copper has 57 m/(Ω*mm²)

    /// <summary>
    /// The electrical resistance of the coil, based on the conductor length, cross section radius, and the conductivity
    /// </summary>
    public float Resistance { 
        get {
            float conductorLength = Diameter * Mathf.PI * numberOfTurns;
            float conductorCrossSectionArea = crossSectionRadius * crossSectionRadius * Mathf.PI;
            float resistance = conductorLength / (Conductivity * conductorCrossSectionArea);
            return resistance;
        }
    }

    /// <summary>
    /// The number of turns
    /// </summary>
    [SerializeField] private int numberOfTurns = 6;

    /// <summary>
    /// The start flux for resetting
    /// </summary>
    private float _startFlux;

    /// <summary>
    /// The start conductivity (for resetting)
    /// </summary>
    private float _startConductivity;

    /// <summary>
    /// The current flux
    /// </summary>
    private float flux;


    /// <summary>
    /// The length of the coil
    /// </summary>
    private float Length => Diameter * Mathf.PI * numberOfTurns;

    private float _current = 0.0f;

    /// <summary>
    /// The electrical current in the conductor
    /// </summary>
    public float Current
    {
        set => _current = value;
        get => _current;
    }

    /// <summary>
    /// Initialization
    /// </summary>
    protected override void Start()
    {
        base.Start();

        Radius = Diameter / 2f;
        flux = GetMagneticFluxInCoil();
        _startFlux = flux;
        _startConductivity = Conductivity;
    }

    /// <summary>
    /// Update is called every frame
    /// </summary>
    protected override void HandleUpdate()
    {
    
    }

    /// <summary>
    /// This function is called every fixed frame rate frame and calculates induction and the field strength.
    /// Also adds the force to the rigidbody if force effects are active.
    /// </summary>
    protected override void HandleFixedUpdate()
    {
        CalculateInduction();

        FieldStrength = (Current * numberOfTurns) / Mathf.Sqrt(Length * Length + Diameter * Diameter);

        if (forceActive)
            _rigidBody.AddForce(GetExternalForce() * transform.up);
    }

    /// <summary>
    /// Gets the magnetic flux in the coil.
    /// </summary>
    /// <returns>The magnetic flux</returns>
    private float GetMagneticFluxInCoil()
    {
        return  GetExternalField() * 
                Radius * Radius * Mathf.PI;
    }

    /// <summary>
    /// Gets the external field in the coil without own field.
    /// This is just a approximation for the B field in the coil.
    /// </summary>
    /// <returns>The external field in the coil</returns>
    private float GetExternalField()
    {
        if (field == null)
            return 0.0f;

        return field.get(transform.position + new Vector3(0, 0, Radius), gameObject).magnitude * Radius * Radius * Mathf.PI;
    }


    private const float u0 = 4 * Mathf.PI * 1e-7f; // Permeability of free space

    /// <summary>
    /// Gets the magnetic field at a given position
    /// </summary>
    /// <param name="point">The required position</param>
    /// <returns>The magnetic field vector at the position</returns>
    public override Vector3 GetB(Vector3 point)
    {
        Vector3 B = Vector3.zero;
        const float stepTheta = Mathf.PI / 20f; // Step size for the integration
        float muOver4Pi = u0 * Current / (4 * Mathf.PI);

        // Get the coil's up direction and right direction for orientation
        Vector3 coilUp = transform.up;     // Coil's up direction
        Vector3 coilRight = transform.right; // Perpendicular to the coil plane
        Vector3 coilForward = transform.forward; // Perpendicular vector forming coil's local plane

        // Loop over the current loop (coil)
        for (float theta = 0; theta < 2 * Mathf.PI; theta += stepTheta)
        {
            float cosTheta = Mathf.Cos(theta);
            float sinTheta = Mathf.Sin(theta);

            // Calculate the local position of the current element in the coil's plane
            Vector3 localPosition = Radius * (cosTheta * coilRight + sinTheta * coilForward);

            // The actual world position of the current element on the coil
            Vector3 coilPosition = transform.position + localPosition;

            // Vector from the current element to the observation point
            Vector3 r = point - coilPosition;
            float rMagnitude = r.magnitude;

            if (rMagnitude == 0) continue; // Avoid division by 0

            // Tangential direction of the current element in the coil
            Vector3 dl = Radius * (-sinTheta * coilRight + cosTheta * coilForward) * stepTheta;

            // Biot-Savart law: dB = (μ0 * I / 4π) * (dl × r) / r^3
            Vector3 dB = muOver4Pi * Vector3.Cross(dl, r) / Mathf.Pow(rMagnitude, 3);

            // Add the small dB to the total field B
            B += dB;
        }

        // Multiply by the number of turns of the coil
        B *= numberOfTurns;
        const float visualizationMultiplier = 1000000f; // Workaround, as otherwise Coil field is not visible
        return B * visualizationMultiplier;
    }

    /// <summary>
    /// Gets the external force which affects the coil.
    /// </summary>
    /// <returns>The external force</returns>
    public float GetExternalForce()
    {
        return -2 * Mathf.PI * Radius * numberOfTurns * Current * GetExternalField();
    }

    /// <summary>
    /// Calculates the induction by the flux change.
    /// </summary>
    private void CalculateInduction()
    {
        var newFlux = GetMagneticFluxInCoil();
        var deltaFlux = newFlux - flux;
        var voltage = (numberOfTurns * -deltaFlux) / Time.fixedDeltaTime;

        Current += voltage / Resistance;
        flux = newFlux;
    }

    /// <summary>
    /// Resets the object
    /// </summary>
    public override void ResetObject()
    {
        if (_rigidBody)
        {
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
        }
        transform.position = startPos;
        transform.rotation = startRot;
        Current = 0.0f;
        fieldStrength = 0.0f;
        flux = _startFlux;
        Conductivity = _startConductivity;
    }
}
