using UnityEngine;

public class GuiGlassrodBalloon : MonoBehaviour {
	
	// Charge game objects + scripts
	public GameObject chargeBalloon;
	public GameObject chargeGlassrod;
	private StaticChargeScript chargeScriptBalloon;
	private StaticChargeScript chargeScriptGlassrod;
	
	// slider values for charges
	private float sliderValueChargeBalloon = 0.0f;
	private float sliderValueChargeGlassrod = 0.0f;
	private float minCharge = -10.0f;	//in uC
	private float maxCharge = 10.0f;	//in uC
	
	// Glow
	public GameObject glowBalloon;
	private Light balloonLight;
	public GameObject glowGlassrod;
	private Light[] glassrodLights;
	
	void Start() {
        chargeScriptBalloon = chargeBalloon.GetComponent<StaticChargeScript>();
		chargeScriptGlassrod = chargeGlassrod.GetComponent<StaticChargeScript>();
		
		balloonLight = glowBalloon.GetComponent<Light>();
		glassrodLights = glowGlassrod.GetComponentsInChildren<Light>();
	}

	void OnGUI()
	{
		// show at the bottom left
		GUI.BeginGroup (new Rect (10, Screen.height - 100, 200, 100));
			sliderValueChargeBalloon = GUI.HorizontalSlider(new Rect(0, 0, 200, 30), sliderValueChargeBalloon, minCharge, maxCharge);
			sliderValueChargeGlassrod = GUI.HorizontalSlider(new Rect(0, 40, 200, 30), sliderValueChargeGlassrod, minCharge, maxCharge);
		GUI.EndGroup ();
		
		GUI.BeginGroup (new Rect (220, Screen.height - 105, 200, 100));
			GUI.Label (new Rect(0, 0, 500, 30), "Charge of Balloon: "+ sliderValueChargeBalloon + "uC");
			GUI.Label (new Rect(0, 40, 500, 30), "Charge of Glass Rod: "+ sliderValueChargeGlassrod + "uC");
		GUI.EndGroup ();
	}
	
	void FixedUpdate()
	{
		chargeScriptBalloon.strength = 1e-6f * sliderValueChargeBalloon;
		chargeScriptGlassrod.strength = 1e-6f *  sliderValueChargeGlassrod;
		
		// change glow of objects, if charge is negative to blue, if positive to red
		// balloon
		if(sliderValueChargeBalloon < 0f) {
			balloonLight.color = Color.blue;
		}
		else if(sliderValueChargeBalloon > 0f) {
			balloonLight.color = Color.red;
		}
		else {
			balloonLight.color = Color.white;
			balloonLight.intensity = 0f;
		}
		// set glows intensity depending on amount of charge
		balloonLight.intensity = 2.0f * Mathf.Abs(sliderValueChargeBalloon / maxCharge);
		
		// glass rod
		float glassrodLightIntensity = 2.0f * Mathf.Abs(sliderValueChargeGlassrod / maxCharge);
		if(sliderValueChargeGlassrod < 0f) {
			SetGlassRodLights(Color.blue, glassrodLightIntensity);
		}
		else if(sliderValueChargeGlassrod > 0f) {
			SetGlassRodLights(Color.red, glassrodLightIntensity);
		}
		else {
			SetGlassRodLights(Color.white, 0f);
		}
	}
	
	void SetGlassRodLights(Color color, float intensity)
	{
		for(int i = 0; i < glassrodLights.Length; i++)
		{
			glassrodLights[i].color = color;
			glassrodLights[i].intensity = intensity;
		}
	}
}
