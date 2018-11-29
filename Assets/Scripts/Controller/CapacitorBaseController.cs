using UnityEngine;

public class CapacitorBaseController : MonoBehaviour, IVoltagePoleTrigger, IResetObject
{
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    public void PullVoltagePoleTrigger(Collider other, GameObject source)
    {
        Destroy(other.gameObject);
    }

    public void resetObject()
    {
        transform.position = startPos;
    }
}
