using System;
using UnityEngine;

public class CapacitorBaseController : MonoBehaviour, IVoltagePoleTrigger
{
    [SerializeField]
    private Capacitor capacitor;

    [SerializeField]
    private GameObject capacitorPlate;


    public void PullVoltagePoleTrigger(Collider other, GameObject source)
    {
        other.GetComponent<PathFollower>().followPath = false;
        capacitor.SetElectronOnPlate(other.gameObject, capacitorPlate);
    }
}
