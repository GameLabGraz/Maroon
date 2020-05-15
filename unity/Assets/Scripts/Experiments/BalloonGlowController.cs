using UnityEngine;

public class BalloonGlowController : MonoBehaviour 
{
	public GameObject chargeInducer;
	public GameObject charge;
	public Light glow;
	public Light leftGlow;
	public Light rightGlow;

	private StaticChargeScript chargeScript;
	private VandeGraaffController vandeGraaffController;
	private const float minGlowRangeOffset = 0.65f;
	private const float maxGlowRangeOffset = 0.75f;
	private const float minGlowRangeDynamic = 0.25f;
	private const float maxGlowRangeDynamic = 0.4f;
	private bool glowEnabled;

	public bool GlowEnabled
	{
		get { return this.glowEnabled; }
		set { this.glowEnabled = value; }
	}
	
	public void Start () {
		this.chargeScript = charge.GetComponent<StaticChargeScript> ();
		this.vandeGraaffController = chargeInducer.GetComponent<VandeGraaffController> ();
	}

	public void Update () 
	{
		bool inducerChargeStrengthIsZero = vandeGraaffController.ChargeStrength <= 0;
		this.leftGlow.enabled = !inducerChargeStrengthIsZero && this.glowEnabled;
		this.rightGlow.enabled = !inducerChargeStrengthIsZero && this.glowEnabled;
		this.glow.enabled = inducerChargeStrengthIsZero && this.glowEnabled;

		float glowFactor = this.vandeGraaffController.GetVoltage () / this.vandeGraaffController.MaxVoltage;

		if (this.chargeScript.strength < 0f) {
			this.leftGlow.range = minGlowRangeOffset + minGlowRangeDynamic * glowFactor;
			this.rightGlow.range = maxGlowRangeOffset + maxGlowRangeDynamic * glowFactor;
			this.glow.color = Color.blue;
		} else {
			this.leftGlow.range = maxGlowRangeOffset + maxGlowRangeDynamic * glowFactor;
			this.rightGlow.range = minGlowRangeOffset + minGlowRangeDynamic * glowFactor;
			this.glow.color = Color.red;
		}
	}
}
