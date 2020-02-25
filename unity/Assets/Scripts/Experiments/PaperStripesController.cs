using UnityEngine;
using System.Collections.Generic;

public class PaperStripesController : MonoBehaviour, ICharge {

	/// <summary>
	/// The glow corresponding to the charge.
	/// For simplicity only one point light for all
	/// Paper Stripes is used.
	/// </summary>
	public Light glow;
	public float maxChargeStrength = 1e-9f;	// per Paper Stripe in Coulomb [C]
	public GameObject vandeGraaff;
	public GameObject[] charges;	// the Static Charges of the Paper Stripes
	public bool isAttachedToInducer;

	private List<StaticChargeScript> chargeScripts;
	private float chargeStrength;

	public bool IsAttachedToInducer
	{
		get{ return this.isAttachedToInducer; }
		set{ this.isAttachedToInducer = value; }
	}

	public bool GlowEnabled
	{
		get{ 
			if(null == this.glow)
				return false;
			return this.glow.enabled;
		}
		set{
			if (null != this.glow)
				this.glow.enabled = value;
		}
	}

	// Use this for initialization
	public void Start () {
		this.chargeScripts = new List<StaticChargeScript> ();
		foreach (GameObject charge in this.charges) {
			if(null != charge) {
				this.chargeScripts.Add(charge.GetComponentInChildren<StaticChargeScript> ());
			}
		}
	}

	public float GetMaxCharge()
	{
		return this.maxChargeStrength;
	}

	public float GetCharge()
	{
		return this.chargeStrength;
	}

	private void SetCharges(float chargeStrength)
	{
		foreach (StaticChargeScript chargeScript in this.chargeScripts) {
			if(null != chargeScript) {
				chargeScript.strength = chargeStrength;
			}
		}
	}

	public void SetCharge(float chargeStrength)
	{
		int sign = (chargeStrength > 0f) ? 1 : -1;
		
		if (Mathf.Abs (chargeStrength) <= this.maxChargeStrength) {
			this.chargeStrength = chargeStrength;
		} else {
			this.chargeStrength = sign * this.maxChargeStrength;
		}
		this.SetCharges (this.chargeStrength);
		this.UpdateGlow ();
	}
	
	public void AddCharge(float chargeStrength)
	{
		if (Mathf.Abs (this.chargeStrength + chargeStrength) <= this.maxChargeStrength) 
		{
			this.chargeStrength += chargeStrength;
			this.SetCharges(this.chargeStrength);
			this.UpdateGlow ();
		}
	}

	public void Discharge()
	{
		this.SetCharges (0f);
	}

	private void UpdateGlow()
	{
		if (null == this.glow)
			return;
		
		if (this.chargeStrength < 0f) {
			this.glow.color = Color.blue;
		} else {
			this.glow.color = Color.red;		
		}
		
		this.glow.intensity = 0.5f * Mathf.Abs (this.chargeStrength / this.maxChargeStrength);
	}
}
