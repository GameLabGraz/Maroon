using UnityEngine;
using VRTK;

public class TeslaCollider : MonoBehaviour {

	public GameObject teslaTrafo;

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
        Debug.Log("Trigger Enter: " + other.gameObject.name);

		if (other.CompareTag ("Player") || other.GetComponent<VRTK_PlayerObject>() != null) 
		{
			if(null != this.teslaTrafoController)
            {
				this.teslaTrafoController.Switch(true);
				Debug.Log("Tesla Trafo switched on");
			}
		}
	}

    public void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") || other.GetComponent<VRTK_PlayerObject>() != null)
        {
            if(null != this.teslaTrafoController)
            {
                this.teslaTrafoController.Switch(false);
                Debug.Log("Tesla Trafo switched off");
            }
        }
    }

}
