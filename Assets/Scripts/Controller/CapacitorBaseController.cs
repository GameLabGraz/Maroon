using UnityEngine;

public class CapacitorBaseController : MonoBehaviour, IVoltagePoleTrigger
{
    [SerializeField]
    private Capacitor capacitor;

    [SerializeField]
    private GameObject capacitorPlate;

    [SerializeField]
    private ChargePoolHandler chargePoolHandler;

    private void Start()
    {
        if(chargePoolHandler == null)
        {
            chargePoolHandler = GameObject.FindObjectOfType<ChargePoolHandler>();
            if (chargePoolHandler == null)
                Debug.LogError("CapacitorBaseController requires a ChargePoolHandler!");
        }
    }

    public void PullVoltagePoleTrigger(Collider other, GameObject source)
    {
        CapacitorPlateController plateController = capacitorPlate.GetComponent<CapacitorPlateController>();
        if (plateController.GetPlateChargeValue() > 0)
        {
            Charge electron = other.GetComponent<Charge>();
            Charge protonToRemove = chargePoolHandler.GetProton(electron.Id); // Get the corresponding proton

            protonToRemove.FadingOut(0.5f);
            Destroy(other.gameObject);
        }
        else
        {
            other.GetComponent<PathFollower>().followPath = false;
            capacitor.SetElectronOnPlate(other.gameObject, capacitorPlate);
        }
    }
}
