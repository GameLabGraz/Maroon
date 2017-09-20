using UnityEngine;
using System.Collections;
using DigitalRuby.ThunderAndLightning;

public class TeslaTrafoController : MonoBehaviour 
{
	public GameObject Bulbs;

	private bool on;
	private AudioSource sound;
	private LightningBoltShapeConeScript lightning;
	private Light[] bulbsPointLights;

	public bool On { 
		get{ return this.on; }
	}
	
	public void Start () {
		this.sound = this.GetComponent<AudioSource> ();
		if (null == this.sound) {
			throw new System.Exception("No AudioSource attached to TeslaTrafo GameObject");
		}
		this.lightning = this.GetComponentInChildren<LightningBoltShapeConeScript> ();
		if (null == this.lightning) {
			throw new System.Exception("No LightningBoltShapeConeScript found");
		}
		if (null != this.Bulbs) {
			this.bulbsPointLights = this.Bulbs.GetComponentsInChildren<Light>();
		}
	}

	public void Switch(bool on)
	{
		// switch sound on/off
		if (null != this.sound) {
            this.sound.volume = SoundManager.instance.efxSource.volume;
			this.sound.enabled = on;
		}
		// switch lightning on/off
		if (null != this.lightning) {
			this.lightning.enabled = on;
		}
		// switch on/off point lights of bulbs
		if (null != this.bulbsPointLights) {
			foreach (Light light in this.bulbsPointLights) {
				if (null != light) {
					light.enabled = on;
				}
			}
		}
		this.on = on;
	}

	public void Update () 
	{
        if (this.sound.enabled)
            this.sound.volume = SoundManager.instance.efxSource.volume;
        this.UpdateBulbs ();
	}

	private void UpdateBulbs()
	{
		if (!this.on)
			return;

		if (null != this.bulbsPointLights) {
			foreach (Light light in this.bulbsPointLights) {
				if (null != light) {
					// set random intensity to light bulbs to create flickering effect
					light.intensity = Random.Range(0f, 8f);
				}
			}
		}
	}
}