using UnityEngine;

public class CapacitorBaseController : MonoBehaviour, IVoltagePoleTrigger
{
    [SerializeField]
    private Capacitor capacitor;

    [SerializeField]
    private GameObject capacitorPlate;

    public void PullVoltagePoleTrigger(Collider other, GameObject source)
    {
        CapacitorPlateController plateController = capacitorPlate.GetComponent<CapacitorPlateController>();
        if (plateController.GetPlateChargeValue() > 0)
        {
            Charge chargeToRemove = plateController.GetCharge(other.GetComponent<Charge>().Id);
            plateController.RemoveCharge(chargeToRemove);

            chargeToRemove.FadingOut(0.5f);
            Destroy(other.gameObject);
        }
        else
        {
            other.GetComponent<PathFollower>().followPath = false;
            capacitor.SetElectronOnPlate(other.gameObject, capacitorPlate);
        }
    }
}
