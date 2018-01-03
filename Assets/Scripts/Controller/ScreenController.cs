using UnityEngine;
using UnityEngine.UI;

public class ScreenController : MonoBehaviour
{
    [SerializeField]
    private string valueDisplayText;

    [SerializeField]
    private Text valueDisplay;

    [SerializeField]
    private GameObject valueGetterObject;

    [SerializeField]
    private string valueGetterMethodByReference;

    private SimulationController simController;

    void Start()
    {
        GameObject simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();

        if (valueDisplay == null)
            GameObject.Find("ValueDisplay").GetComponent<Text>();

        valueDisplay.text = valueDisplayText;
    }

    void FixedUpdate()
    {
        MessageArgs messageArgs = new MessageArgs();
        valueGetterObject.SendMessage(valueGetterMethodByReference, messageArgs);
        valueDisplay.text = valueDisplayText + messageArgs.value.ToString();
    }
}
