using UnityEngine;

public class AddToSimulationResetObject : MonoBehaviour
{
    private void Awake()
    {
        var resetObject = GetComponent<IResetObject>();
        if(resetObject != null) SimulationController.Instance.AddNewResetObject(resetObject);
    }
}
