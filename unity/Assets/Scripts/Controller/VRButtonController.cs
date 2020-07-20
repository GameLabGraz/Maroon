using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class VRButtonController : VRTK_InteractableObject
{
    [System.Obsolete]
    [SerializeField]
    private GameObject invokeObject;

    [System.Obsolete]
    [SerializeField]
    private string methodName;

    [SerializeField]
    private bool isToggleButton;

    private bool toggleValue;

    public UnityEvent OnButtonClicked;

    public bool IsToogleButton
    {
        get => isToggleButton;
        set => isToggleButton = value;
    }

    public bool ToogleValue
    {
        get => toggleValue;
        set => toggleValue = value;
    }

    public override void StartUsing(VRTK_InteractUse currentUsingObject = null)
    {
        base.StartUsing(usingObject);
        OnButtonClicked.Invoke();
        
        if(invokeObject)
            invokeObject.SetActive(true);

        if(isToggleButton)
        {
            toggleValue = !toggleValue;
            invokeObject.SendMessage(methodName, toggleValue);

            GetComponent<Renderer>().material.color = toggleValue ? Color.green : Color.red;
        }
        else
        {
            invokeObject.SendMessage(methodName);
        }
           
    }
}
