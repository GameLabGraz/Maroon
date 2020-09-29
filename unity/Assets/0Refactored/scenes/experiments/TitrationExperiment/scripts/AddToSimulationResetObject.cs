using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddToSimulationResetObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        if(gameObject.GetComponent<Slider>() != null)
            SimulationController.Instance.AddNewResetObject(gameObject.GetComponent<Slider>().GetComponent<IResetObject>());

        if (gameObject.GetComponent<Dropdown>() != null)
            SimulationController.Instance.AddNewResetObject(gameObject.GetComponent<Dropdown>().GetComponent<IResetObject>());
    }

}
