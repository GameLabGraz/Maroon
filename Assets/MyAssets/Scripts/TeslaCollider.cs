using UnityEngine;
using System.Collections;

public class TeslaCollider : MonoBehaviour {

	public GameObject teslaTrafo;
	public bool SwitchOn;

	private TeslaTrafoController teslaTrafoController;
	
	public void Start () {
		if (null == teslaTrafo) {
			throw new System.Exception("No TeslaTrafo GameObject found");
		}
		teslaTrafoController = teslaTrafo.GetComponent<TeslaTrafoController> ();
		if (null == teslaTrafoController) {
			throw new System.Exception("No TeslaTrafoController script attached to GameObject");
		}
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || teslaTrafoController == null)
            return;

        teslaTrafoController.Switch(SwitchOn);
        Debug.Log(string.Format("Tesla Trafo switched {0}", SwitchOn ? "on" : "off"));
    }

    public void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || teslaTrafoController == null)
            return;

        teslaTrafoController.Switch(!SwitchOn);
        Debug.Log(string.Format("Tesla Trafo switched {0}", !SwitchOn ? "on" : "off"));
    }
}
