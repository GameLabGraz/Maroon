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

    private void Start()
    {
        if (valueDisplay == null)
            GameObject.Find("ValueDisplay").GetComponent<Text>();

        valueDisplay.text = valueDisplayText;
    }

    private void FixedUpdate()
    {
        var messageArgs = new MessageArgs();
        valueGetterObject.SendMessage(valueGetterMethodByReference, messageArgs);
        valueDisplay.text = valueDisplayText + messageArgs.value.ToString();
    }
}
