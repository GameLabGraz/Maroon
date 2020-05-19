//-----------------------------------------------------------------------------
// Coil.cs
//
// Class for a live coil.
// Is also a electro magnetic object.  
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;

/// <summary>
/// Class for a live coil.
/// Is also a electro magnetic object. 
/// </summary>
public class Coil : EMObject, IResetObject
{
    /// <summary>
    /// The magnetic field
    /// </summary>
    public BField field;

    /// <summary>
    /// The electrical current in the conductor
    /// </summary>
    public float current = 0.0f;

    /// <summary>
    /// The coild diameter
    /// </summary>
    public float diameter = 5f;

    /// <summary>
    /// The length of the coil
    /// </summary>
    public float length = 0.75f;

    /// <summary>
    /// The electrical resistance of the coil
    /// </summary>
    public float resistance = 250;

    /// <summary>
    /// The number of turns
    /// </summary>
    public int number_of_turns = 6;

    /// <summary>
    /// The start flux for reseting
    /// </summary>
    private float start_flux;

    /// <summary>
    /// The current flux
    /// </summary>
    private float flux;

    /// <summary>
    /// The resistance factor
    /// </summary>
    private float resistance_factor = 0;

    /// <summary>
    /// Initialization
    /// </summary>
    protected override void Start()
    {
        base.Start();

        flux = getMagneticFluxInCoil();
        start_flux = flux;
    }

    /// <summary>
    /// Update is called every frame
    /// </summary>
    protected override void HandleUpdate()
    {
    
    }

    /// <summary>
    /// This function is called every fixed framerate frame and calculates induction and the field strength.
    /// Also adds the force to the rigidbody if force effects are active.
    /// </summary>
    protected override void HandleFixedUpdate()
    {
        calculateInduction();
        field_strength = (current * number_of_turns) / Mathf.Sqrt(length * length + diameter * diameter);

        if (force_active)
            GetComponent<Rigidbody>().AddForce(getExternalForce() * transform.up);
    }

    /// <summary>
    /// Sets the current
    /// </summary>
    /// <param name="current">The current value</param>
    public void setCurrent(float current)
    {
        this.current = current;
    }

    /// <summary>
    /// Gets the current
    /// </summary>
    /// <returns></returns>
    public float getCurrent()
    {
        return this.current;
    }

    public void getCurrentByReference(MessageArgs args)
    {
        args.value = this.current;
    }

    /// <summary>
    /// Sets the risistance factor.
    /// User can sets the risistance factor by a slider.
    /// </summary>
    /// <param name="resistance_factor">The silder to get selected value from user</param>
    public void setResistanceFactor(float resistance_factor)
    {
        this.resistance_factor = resistance_factor;
    }

    public float GetResistanceFactor()
    {
        return resistance_factor;
    }

    /// <summary>
    /// Gets the magnetic flux in the coil.
    /// </summary>
    /// <returns>The magnetix flux</returns>
    private float getMagneticFluxInCoil()
    {
        float field = getExternalField();
        float flux = field * (diameter / 2.0f) * (diameter / 2.0f) * Mathf.PI;
        return flux;
    }

    /// <summary>
    /// Gets the external field in the coil without own field.
    /// This is just a approximation for the B field in the coil.
    /// </summary>
    /// <returns>The external field in the coil</returns>
    private float getExternalField()
    {
        if (field == null)
            return 0.0f;
        else
        {
            float B = field.get(transform.position + new Vector3(0, 0, diameter / 2f), gameObject).magnitude * (diameter / 2f) * (diameter / 2f) * Mathf.PI;
            return B;
        }
    }

    /// <summary>
    /// Gets the external force which affects the coil.
    /// </summary>
    /// <returns>The external force</returns>
    public float getExternalForce()
    {
        float F = -2 * Mathf.PI * (diameter / 2.0f) * number_of_turns * current * getExternalField();
        return F;
    }

    /// <summary>
    /// Calculates the induction by the flux change.
    /// </summary>
    private void calculateInduction()
    {
        float new_flux = getMagneticFluxInCoil();
        float delta_flux = new_flux - flux;
        float voltage = (number_of_turns * -delta_flux) / Time.fixedDeltaTime;

        current += voltage / (resistance + resistance * resistance_factor);
        flux = new_flux;
    }

    /// <summary>
    /// Resets the object
    /// </summary>
    public override void ResetObject()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        transform.position = startPos;
        transform.rotation = startRot;
        current = 0.0f;
        field_strength = 0.0f;
        flux = start_flux;
    }
}
