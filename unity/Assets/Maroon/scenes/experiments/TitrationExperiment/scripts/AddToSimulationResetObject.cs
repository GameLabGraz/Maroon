using UnityEngine;

public class AddToSimulationResetObject : MonoBehaviour
{
    private void Awake()
    {
        var resetObject = GetComponent<IResetObject>();
    }
}
