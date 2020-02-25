using UnityEngine;
using VRTK;

public class VRButtonController : VRTK_InteractableObject
{
    [SerializeField]
    private GameObject invokeObject;

    [SerializeField]
    private string methodName;

    [SerializeField]
    private bool isToggleButton = false;

    private bool toggleValue = false;

    public bool IsToogleButton
    {
        get
        {
            return this.isToggleButton;
        }
        set
        {
            this.isToggleButton = value;
        }
    }

    public bool ToogleValue
    {
        get
        {
            return this.toggleValue;
        }
        set
        {
            this.toggleValue = value;
        }
    }

    public override void StartUsing(VRTK_InteractUse currentUsingObject = null)
    {
        base.StartUsing(usingObject);

        Debug.Log("Iron Filling Button pressed, start Iron Filling");

        invokeObject.SetActive(true);

        if(isToggleButton)
        {
            toggleValue = !toggleValue;
            invokeObject.SendMessage(methodName, toggleValue);

            if (toggleValue)
            {
                GetComponent<Renderer>().material.color = Color.green;
            }
            else
            {
                GetComponent<Renderer>().material.color = Color.red;
            }
        }
        else
        {
            invokeObject.SendMessage(methodName);
        }
           
    }
}
