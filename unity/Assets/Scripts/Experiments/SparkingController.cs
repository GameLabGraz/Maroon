using UnityEngine;

public class SparkingController : MonoBehaviour {

	public GameObject sparkingStartPoint;
	public GameObject sparkingEndPoint;
	public bool showDistance;
	/// <summary>
	/// The normalization factor to get the distance in meters.
	/// The VdG sphere should have a diameter of 1m. The in-game diameter
	/// is 2.1. So we multiply distances with 0.476 (1/2.1) to get a normalized result.
	/// </summary>
	public float normalizationFactor = 0.476f;
	public GameObject vandeGraaff;
	public float emax = 3e06f;

	private VandeGraaffController vandeGraaffController;
	private AudioSource sound;
	
	public void Start ()
	{
		if (null == sparkingStartPoint || null == sparkingStartPoint.transform) {
			throw new System.Exception("No sparking start point GameObject found");
		}
		if (null == sparkingEndPoint || null == sparkingEndPoint.transform) {
			throw new System.Exception("No sparking end point GameObject found");
		}
		if (null != vandeGraaff) {
			this.vandeGraaffController = vandeGraaff.GetComponent<VandeGraaffController>();
		}

		this.sound = gameObject.GetComponent<AudioSource> ();

	}

	public void Update()
	{
		float vandeGraaffVoltage = this.vandeGraaffController.GetVoltage ();
		float distance = this.GetDistanceBetweenSparkingPoints ();

		// check if electric breakdown voltage of air is reached
		if (vandeGraaffVoltage >= emax * distance) {
			// generate spark
			this.GenerateSpark();
			this.sound.Play();
			// discharge VdG
			this.vandeGraaffController.Discharge();
		}
	}
	
	/// <summary>
	/// Gets the normalized distance between sparking points.
	/// </summary>
	/// <returns>The distance between sparking points in [m].</returns>
	public float GetDistanceBetweenSparkingPoints()
	{
		return Vector3.Distance(sparkingStartPoint.transform.position, sparkingEndPoint.transform.position) * normalizationFactor;
	}

	public void GenerateSpark()
	{
		// Important, make sure this script is assigned properly, or you will get null ref exceptions.
		DigitalRuby.ThunderAndLightning.LightningBoltScript script = gameObject.GetComponent<DigitalRuby.ThunderAndLightning.LightningBoltScript>();
		int count = 1;
		float duration = 0.25f;
		float delay = 0.0f;
		System.Random r = new System.Random();
		Vector3 start = this.sparkingStartPoint.transform.position;
		Vector3 end = this.sparkingEndPoint.transform.position;
		int generations = 6;
		float chaosFactor = 0.2f;
		float trunkWidth = 0.05f;
		float glowIntensity = 0.1793653f;
		float glowWidthMultiplier = 4f;
		float forkedness = 0.5f;
		float singleDuration = Mathf.Max(1.0f / 30.0f, (duration / (float)count));
		float fadePercent = 0.15f;
		float growthMultiplier = 0f;
		
		while (count-- > 0)
		{
			DigitalRuby.ThunderAndLightning.LightningBoltParameters parameters = new DigitalRuby.ThunderAndLightning.LightningBoltParameters
			{
				Start = start,
				End = end,
				Generations = generations,
				LifeTime = (count == 1 ? singleDuration : (singleDuration * (((float)r.NextDouble() * 0.4f) + 0.8f))),
				Delay = delay,
				ChaosFactor = chaosFactor,
				TrunkWidth = trunkWidth,
				GlowIntensity = glowIntensity,
				GlowWidthMultiplier = glowWidthMultiplier,
				Forkedness = forkedness,
				Random = r,
				FadePercent = fadePercent, // set to 0 to disable fade in / out
				GrowthMultiplier = growthMultiplier
			};
			script.CreateLightningBolt(parameters);
			delay += (singleDuration * (((float)r.NextDouble() * 0.8f) + 0.4f));
		}
	}
	
	public void OnGUI () {
		if (showDistance) {
			GUI.Label(new Rect(10f, Screen.height - 30f, 200f, 20f), string.Format("Distance: {0} m", GetDistanceBetweenSparkingPoints()));
		}
	}
}
