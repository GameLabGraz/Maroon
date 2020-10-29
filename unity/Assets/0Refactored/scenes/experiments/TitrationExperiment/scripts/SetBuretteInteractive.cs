using UnityEngine;

public class SetBuretteInteractive : MonoBehaviour 
{

    [SerializeField] private GameObject burette;
    private OpenBurette openBuretteScript;

    void Start () {
        openBuretteScript = burette.gameObject.GetComponent<OpenBurette>();
    }
    
    public void EnableBuretteTap()
    {
        openBuretteScript.interactable = true;
    }

    public void DisableBuretteTap()
    {
        openBuretteScript.interactable = false;
    }
}
