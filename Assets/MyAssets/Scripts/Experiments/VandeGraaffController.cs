using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VandeGraaffController : MonoBehaviour 
{
	/// <summary>
	/// The charge object of the Van de Graaff Generator
	/// </summary>
	public GameObject charge;
	/// <summary>
	/// The objects influenced through the Van de Graaff 
	/// Generator by Electrostatic Induction
	/// </summary>
	public GameObject[] inducedObjects;
	/// <summary>
	/// The field lines to visualize the Electric Field
	/// </summary>
	public GameObject fieldLines;
	/// <summary>
	/// The glow corresponding to the charge
	/// </summary>
	public Light glow;
	/// <summary>
	/// Noise of the mechanical parts (motor + belt)
	/// </summary>
	public AudioSource sound;
	/// <summary>
	/// The sphere radius in [m]
	/// </summary>
	public float radius = 0.5f;
	/// <summary>
	/// Electric breakdown field strength of the gas surrounding the sphere in [V/m]
	/// 30kV/cm typically for dry air at STP (Standard conditions for temperature and pressure)
	/// </summary>
	public float emax = 3e06f;
	/// <summary>
	/// The charging current in [A], respectively [C/s]
	/// Charge transported by the belt to the sphere
	/// </summary>
	public float chargingCurrent = 10e-6f;

	private StaticChargeScript chargeScript;
	private List<ICharge> inducedObjectsCharge;
	private bool on = false;
    private bool fieldLinesEnabled = true;
	private FieldLineScript[] fieldLineScripts;
	private Object lockObject = new Object();

	/// <summary>
	/// The potential difference between the sphere and earth in [V]
	/// Derived from the charge in the sphere by Gauss' Law
	/// </summary>
	public float GetVoltage()
	{
		return this.ChargeStrength/(4.0f * Mathf.PI * PhysicalConstants.e0 * radius);
	}

	/// <summary>
	/// The maximum voltage produced by the Van de Graaff Generator in [V]
	/// Depending on Emax and radius of sphere.
	/// </summary>
	public float MaxVoltage
	{
		get{ return emax * radius; }
	}

	/// <summary>
	/// The current charge strength in the sphere in [C]
	/// </summary>
	public float ChargeStrength
	{
		get{ return this.chargeScript.strength; }
		set{ this.chargeScript.strength = value; }
	}

	public bool On
	{
		get{ return this.on; }
	}

	public bool FieldLinesEnabled
	{
		get{ return this.fieldLinesEnabled; }
		set{ this.fieldLinesEnabled = value; }
	}

	public bool GlowEnabled
	{
		get{ 
			if(null == this.glow)
				return false;
			return this.glow.enabled;
		}
		set {
			if (null != this.glow)
				this.glow.enabled = value;
		}
	}
	
	public void Start ()
	{
		// Get the Van de Graaff Static Charge Script
		this.chargeScript = charge.GetComponent<StaticChargeScript> ();
		// Get the Static Charge Handles of the induced objects
		this.inducedObjectsCharge = new List<ICharge> ();
		foreach (GameObject inducedObject in this.inducedObjects) {
			if(null != inducedObject) {
				this.inducedObjectsCharge.Add(inducedObject.GetComponent<ICharge>());
			}
		}
		// Get the Field Line Scripts
		this.fieldLineScripts = this.fieldLines.GetComponentsInChildren<FieldLineScript> ();
	}

	public void Update ()
	{
		this.ShowFieldLines ();
		// calculate and update charge of VdG
		this.UpdateCharge ();
		this.UpdateGlow ();
		// update induced charge of other game objects (e.g. Grounder)
		this.UpdateInducedCharges ();
	}

	private void UpdateCharge()
	{
		if (!this.on) 
		{
			// TODO: apply formula for corona discharge and leakage
			// For now just leak a third of the charging current when VdG is switched off
			float leakedChargeSinceLastFrame = Time.deltaTime * (this.chargingCurrent / 3f);
			if (this.ChargeStrength - leakedChargeSinceLastFrame > 0f) 
			{
				this.ChargeStrength -= leakedChargeSinceLastFrame;
			} else 
			{
				this.ChargeStrength = 0f;
			}
		} else if (this.GetVoltage () < MaxVoltage) 
		{
			// add charge transported to the sphere during the last frame
			float transportedChargeSinceLastFrame = Time.deltaTime * this.chargingCurrent;
			this.ChargeStrength += transportedChargeSinceLastFrame;

			//Debug.Log (string.Format ("{0}s {1}s, {2}, {3}, {4}", Time.realtimeSinceStartup, Time.deltaTime, transportedChargeSinceLastFrame, this.ChargeStrength, GetVoltage()));
		}
	}

	public void EnableFieldLines(bool enable)
	{
		if (null != this.fieldLineScripts) {
			foreach (FieldLineScript fieldLine in fieldLineScripts) {
				fieldLine.Line.vectorObject.SetActive(enable);
			}
		}
	}

	private void ShowFieldLines()
	{
		lock (this.lockObject) {
			// Disable field lines when VdG charge is 0 or the user wants to hide them
			this.EnableFieldLines (this.ChargeStrength > 0f && this.fieldLinesEnabled);
		}
	}

	private void UpdateInducedCharges()
	{
		// For now just fake electrostatic induction by applying
		// a charge of opposite value corresponding to the charge strength of the VdG
		// to all induced objects which are not touching the VdG.
		// If an induced object is attached to the VdG it will get a charge of the
		// same sign as the VdG.
		float vanDeGraaffChargeSign = (this.ChargeStrength >= 0f) ? 1f : -1f;
		float inducedChargeSign;

		foreach (ICharge inducedCharge in this.inducedObjectsCharge) {
			if (null != inducedCharge) {
				inducedChargeSign = inducedCharge.IsAttachedToInducer ? vanDeGraaffChargeSign : vanDeGraaffChargeSign * -1f;
				inducedCharge.SetCharge (inducedChargeSign * this.GetVoltage () / MaxVoltage * inducedCharge.GetMaxCharge ());
			}
		}
	}

	private void UpdateGlow()
	{
		if (null == this.glow)
			return;

		if (this.ChargeStrength < 0f) {
			this.glow.color = Color.blue;
		} else {
			this.glow.color = Color.red;
		}

		this.glow.range = 1.35f + 1.4f * Mathf.Abs (this.GetVoltage() / this.MaxVoltage);
	}

	public void Discharge()
	{
		this.ChargeStrength = 0f;
		
		// "Discharge" induced objects
		lock (this.lockObject) 
		{
			// To avoid unwanted field lines from the induced object charges
			// we shortly deactivate the field lines before setting all induced charges to zero
			this.EnableFieldLines (false);
			foreach (ICharge inducedCharge in this.inducedObjectsCharge) {
				if (null != inducedCharge) {
					inducedCharge.Discharge ();
				}
			}
			this.EnableFieldLines (this.fieldLinesEnabled);
		}
	}

	public void Switch()
	{
		this.on = !on;
		// enable sound if VdG is switched on
		this.sound.enabled = this.on;
	}

	public void OnCollisionEnter(Collision col)
	{
		GameObject collidedGameObject = col.gameObject;
		if(collidedGameObject.tag != "Untagged")
			Debug.Log (collidedGameObject.tag);
		if ("Balloon" == collidedGameObject.tag) {
			BalloonController balloonController = collidedGameObject.GetComponent<BalloonController>();
			if(null != balloonController)
			{
				float balloonCharge = balloonController.GetCharge();

				// Move charge from balloon to VdG. The moving charge is the amount 
				// to make the balloon charge positive.
				// For simplicity the balloon charge will always be of the same magnitude and only
				// changes its sign (+/-).
				this.ChargeStrength -= (2f * Mathf.Abs(balloonCharge));

				// invert charge of balloon (set it to positive)
				balloonController.SetCharge(Mathf.Abs(balloonCharge));
			}
		}
	}
}
