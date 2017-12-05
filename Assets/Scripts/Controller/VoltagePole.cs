using UnityEngine;

public class VoltagePole : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Electron"))
            return;

        Charge charge = other.GetComponent<Charge>();

        if(!charge.JustCreated)
        {
            IVoltagePoleTrigger voltagePoleTrigger = GetComponentInParent<IVoltagePoleTrigger>();
            if(voltagePoleTrigger != null)
                voltagePoleTrigger.PullVoltagePoleTrigger(other, gameObject);
        }
           
        else
            charge.JustCreated = false;      
    }
}
