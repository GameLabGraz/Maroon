using System;
using System.Collections.Generic;
using UnityEngine;

public class CapacitorBaseController : MonoBehaviour, IVoltagePoleTrigger
{
    [SerializeField]
    private Capacitor capacitor;

    [SerializeField]
    private GameObject capacitorPlate;


    public void PullVoltagePoleTrigger(Collider other, GameObject source)
    {
        if(GetPlateChargeValue() > 0)
        {
            CapacitorPlateController plateController = capacitorPlate.GetComponent<CapacitorPlateController>();
            Charge chargeToRemove = plateController.GetChargeAt(0);
            plateController.RemoveCharge(chargeToRemove);

            Destroy(chargeToRemove.gameObject);
            Destroy(other.gameObject);
        }
        else
        {
            other.GetComponent<PathFollower>().followPath = false;
            capacitor.SetElectronOnPlate(other.gameObject, capacitorPlate);
        }
    }

    private float GetPlateChargeValue()
    {
        float chargeValue = 0;

        List<Charge> chargesOnPlate = capacitorPlate.GetComponent<CapacitorPlateController>().GetCharges();
        foreach(Charge charge in chargesOnPlate)
            chargeValue += charge.ChargeValue;

        return chargeValue;
    }
}
