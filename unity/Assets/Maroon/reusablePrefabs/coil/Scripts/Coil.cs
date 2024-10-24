//-----------------------------------------------------------------------------
// Coil.cs
//
// Class for a live coil.
// Is also a electromagnetic object.  
//-----------------------------------------------------------------------------
//

using System;
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

    /// <summary>
    /// The electrical resistance of the coil
    /// </summary>
    [SerializeField] private float resistance = 33f;

    /// <summary>
    /// The number of turns
    /// </summary>
    [SerializeField] private int numberOfTurns = 6;

    /// <summary>
    /// The start flux for resetting
    /// </summary>
    private float _startFlux;

    /// <summary>
    /// The current flux
    /// </summary>
    private float flux;

    /// <summary>
    /// The resistance factor
    /// </summary>
    private float _resistanceFactor = 0;

    /// <summary>
    /// The electrical current in the conductor
    /// </summary>
    private float _current = 0.0f;

    /// <summary>
    /// The length of the coil
    /// </summary>
    private float Length => diameter * Mathf.PI * numberOfTurns;

    public float Current
    {
        set => _current = value;
        get => _current;
    }

    public float ResistanceFactor
    {
        set => _resistanceFactor = value;
        get => _resistanceFactor;
    }

    /// <summary>
    /// Initialization
    /// </summary>
    protected override void Start()
    {
        base.Start();

        flux = GetMagneticFluxInCoil();
        _startFlux = flux;
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

        FieldStrength = (Current * numberOfTurns) / Mathf.Sqrt(Length * Length + diameter * diameter);

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
                (diameter / 2.0f) * (diameter / 2.0f) * Mathf.PI;
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

        return field.get(transform.position + new Vector3(0, 0, diameter / 2f), gameObject).magnitude * (diameter / 2f) * (diameter / 2f) * Mathf.PI;
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
        float stepTheta = 0.1f; // Step size for the integration

        // Get the coil's up direction and right direction for orientation
        Vector3 coilUp = transform.up;     // Coil's up direction
        Vector3 coilRight = transform.right; // Perpendicular to the coil plane
        Vector3 coilForward = transform.forward; // Perpendicular vector forming coil's local plane
        float radius = diameter / 2f;

        // Loop over the current loop (coil)
        for (float theta = 0; theta < 2 * Mathf.PI; theta += stepTheta)
        {
            // Calculate the local position of the current element in the coil's plane
            Vector3 localPosition = radius * (Mathf.Cos(theta) * coilRight + Mathf.Sin(theta) * coilForward);

            // The actual world position of the current element on the coil
            Vector3 coilPosition = transform.position + localPosition;

            // Vector from the current element to the observation point
            Vector3 r = point - coilPosition;
            float rMagnitude = r.magnitude;

            if (rMagnitude == 0) continue; // Avoid division by 0

            // Tangential direction of the current element in the coil
            Vector3 dl = radius * (-Mathf.Sin(theta) * coilRight + Mathf.Cos(theta) * coilForward) * stepTheta;

            // Biot-Savart law: dB = (μ0 * I / 4π) * (dl × r) / r^3
            Vector3 dB = (u0 * Current / (4 * Mathf.PI)) * Vector3.Cross(dl, r) / Mathf.Pow(rMagnitude, 3);

            // Add the small dB to the total field B
            B += dB;
        }

        // Multiply by the number of turns of the coil
        B *= numberOfTurns;
        return B;
    }

    /// <summary>
    /// Gets the external force which affects the coil.
    /// </summary>
    /// <returns>The external force</returns>
    public float GetExternalForce()
    {
        return -2 * Mathf.PI * (diameter / 2.0f) * numberOfTurns * _current * GetExternalField();
    }

    /// <summary>
    /// Calculates the induction by the flux change.
    /// </summary>
    private void CalculateInduction()
    {
        var newFlux = GetMagneticFluxInCoil();
        var deltaFlux = newFlux - flux;
        var voltage = (numberOfTurns * -deltaFlux) / Time.fixedDeltaTime;

        Current += voltage / (resistance + resistance * _resistanceFactor);
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
    }
}
