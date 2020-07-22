using UnityEngine;
using Util;
using VRTK;

public class TeslaCollider : MonoBehaviour
{
    [SerializeField]
	private TeslaTrafoController _teslaTrafoController;
	
	public void Start ()
	{
		if (!_teslaTrafoController)
			throw new System.Exception("No TeslaTrafoController attached");
	}

	public void OnTriggerEnter(Collider other)
	{
        Debug.Log("Trigger Enter: " + other.gameObject.name);

	    if (!other.CompareTag("Player") && other.GetComponent<VRTK_PlayerObject>() == null)
	        return;

        _teslaTrafoController.Switch(true);
	    Debug.Log("Tesla Trafo switched on");
	}

    public void OnTriggerExit(Collider other)
    {
        if (!PlayerUtil.IsPlayer(other.gameObject))
            return;

       _teslaTrafoController.Switch(false);
       Debug.Log("Tesla Trafo switched off");
    }

}
