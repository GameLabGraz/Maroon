using UnityEngine;
using System.Collections;

public class BalloonController : MonoBehaviour, ICharge {

	public GameObject charge;
	public float maxChargeStrength = 1.65e-6f;	// in Coulomb [C]
	public bool isAttachedToInducer;
	
	private StaticChargeScript chargeScript;
	private AudioSource sound;
	
	public float ChargeStrength
	{
		get{ return this.chargeScript.strength; }
		set{ this.chargeScript.strength = value; }
	}

	public bool IsAttachedToInducer
	{
		get{ return this.isAttachedToInducer; }
		set{ this.isAttachedToInducer = value; }
	}

	public void Start () 
	{
		this.chargeScript = charge.GetComponent<StaticChargeScript> ();
		// Randomly set initial charge of ballon - positive or negative
		if (Random.Range (0, 2) > 0) {
			this.chargeScript.strength *= -1f;
		}

		this.sound = gameObject.GetComponent<AudioSource> ();
	}

	public void Update () 
	{
	}

	public float GetMaxCharge()
	{
		return this.maxChargeStrength;
	}
	
	public float GetCharge()
	{
		return this.ChargeStrength;
	}
	
	public void SetCharge(float chargeStrength)
	{
		Debug.Log (chargeStrength);
		if (Mathf.Abs (chargeStrength) <= this.maxChargeStrength)
		{
			this.ChargeStrength = chargeStrength;
		}
	}
	
	public void AddCharge(float chargeStrength)
	{
		if (Mathf.Abs (this.ChargeStrength + chargeStrength) <= this.maxChargeStrength) 
		{
			this.ChargeStrength += chargeStrength;
		}
	}

	public void Discharge()
	{
		this.SetCharge (0f);
	}

	public void OnCollisionEnter(Collision col)
	{
		Debug.Log (col.gameObject.tag);
		this.sound.Play ();
	}
}
