using UnityEngine;
using UnityEngine.UI;

public class AddToSimulationResetObject : MonoBehaviour
{
    void Awake()
    {
        if(gameObject.GetComponent<Slider>() != null)
            SimulationController.Instance.AddNewResetObject(gameObject.GetComponent<Slider>().GetComponent<IResetObject>());

        if (gameObject.GetComponent<Dropdown>() != null)
            SimulationController.Instance.AddNewResetObject(gameObject.GetComponent<Dropdown>().GetComponent<IResetObject>());
    }

}
