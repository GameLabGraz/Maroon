using UnityEngine;

public class VoltagePole : MonoBehaviour
{
    [SerializeField]
    private bool debug = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Charge"))
            return;

        Charge charge = other.GetComponent<Charge>();

        if (debug)
        {
            Debug.Log("VoltagePole triggered");
            Debug.Log("Charge.JustCreated = " + charge.JustCreated);
        }

        if (!charge.JustCreated)
        {
            IVoltagePoleTrigger voltagePoleTrigger = GetComponentInParent<IVoltagePoleTrigger>();
            if(voltagePoleTrigger != null)
                voltagePoleTrigger.PullVoltagePoleTrigger(other, gameObject);
        }
           
        else
            charge.JustCreated = false;      
    }
}
