using UnityEngine;

public class GrounderController : MonoBehaviour, ICharge {
	
	public GameObject charge;
	public Light glow;
	public float maxChargeStrength = 1.6e-5f;	// in Coulomb [C]
	public bool isAttachedToInducer;

	private StaticChargeScript chargeScript;

	public float ChargeStrength
	{
		get{ return this.chargeScript.strength; }
		set{ this.chargeScript.strength = value; }
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

	public bool IsAttachedToInducer
	{
		get{ return this.isAttachedToInducer; }
		set{ this.isAttachedToInducer = value; }
	}

	public void Start ()
	{
		this.chargeScript = charge.GetComponent<StaticChargeScript> ();
	}

	public void Update () {
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
		int sign = (chargeStrength > 0f) ? 1 : -1;

		if (Mathf.Abs (chargeStrength) <= this.maxChargeStrength) {
			this.ChargeStrength = chargeStrength;
		} else {
			this.ChargeStrength = sign * this.maxChargeStrength;
		}
		this.UpdateGlow ();
	}

	public void AddCharge(float chargeStrength)
	{
		if (Mathf.Abs (this.ChargeStrength + chargeStrength) <= this.maxChargeStrength) 
		{
			this.ChargeStrength += chargeStrength;
			this.UpdateGlow ();
		}
	}

	public void Discharge()
	{
		this.SetCharge (0f);
	}

	private void UpdateGlow()
	{
		if (null == this.glow)
			return;

		if (this.ChargeStrength <= 0f) {
			this.glow.color = Color.blue;
		} else {
			this.glow.color = Color.red;		
		}

		this.glow.range = 0.8f + 0.4f * Mathf.Abs (this.ChargeStrength / this.maxChargeStrength);
	}
	
	public void OnCollisionEnter(Collision col)
	{
		GameObject collidedGameObject = col.gameObject;
		Debug.Log (collidedGameObject.tag);
		if ("Balloon" == collidedGameObject.tag) {
			BalloonController balloonController = collidedGameObject.GetComponent<BalloonController>();
			if(null != balloonController)
			{
				float balloonCharge = balloonController.GetCharge();
				
				// Move charge from Grounder to balloon. The moving charge is the amount 
				// to make the balloon charge negative.
				// For simplicity the balloon charge will always be of the same magnitude and only
				// changes its sign (+/-).
				Debug.Log("Before" + this.ChargeStrength);
				this.ChargeStrength += (2f * Mathf.Abs(balloonCharge));
				Debug.Log("After" + this.ChargeStrength);
				this.UpdateGlow();

				// invert charge of balloon (set it to negative)
				balloonController.SetCharge(-1f * Mathf.Abs(balloonCharge));
			}
		}
	}
}
