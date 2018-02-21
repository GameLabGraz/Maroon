using UnityEngine;

public class CapacitorBaseController : MonoBehaviour, IVoltagePoleTrigger
{
    public void PullVoltagePoleTrigger(Collider other, GameObject source)
    {
        Destroy(other.gameObject);
    }
}
