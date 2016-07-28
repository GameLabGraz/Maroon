using UnityEngine;
using System.Collections;

public class TeslaCollider : MonoBehaviour {

	public GameObject teslaTrafo;
	public bool SwitchOn;

	private TeslaTrafoController teslaTrafoController;
	
	public void Start () {
		if (null == this.teslaTrafo) {
			throw new System.Exception("No TeslaTrafo GameObject found");
		}
		this.teslaTrafoController = this.teslaTrafo.GetComponent<TeslaTrafoController> ();
		if (null == this.teslaTrafoController) {
			throw new System.Exception("No TeslaTrafoController script attached to GameObject");
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Player")) 
		{
			if(null != this.teslaTrafoController) {
				this.teslaTrafoController.Switch(this.SwitchOn);
				Debug.Log(string.Format("Tesla Trafo switched {0}", this.SwitchOn ? "on" : "off"));
			}
		}
	}
}
