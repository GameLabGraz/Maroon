using UnityEngine;
using UnityEditor;
using VRTK;
using System;

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

    public override void StartUsing(GameObject usingObject)
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

[CustomEditor(typeof(VRButtonController))]
public class VRButtonEditor : Editor
{
    private int selectedToggleValue = 0;
    private string[] toogleValueOptions = { "true", "false" };

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VRButtonController VRButtonTarget = target as VRButtonController;

        if (VRButtonTarget.IsToogleButton)
        {
            selectedToggleValue = EditorGUILayout.Popup("Toogle Value", selectedToggleValue, toogleValueOptions);
            VRButtonTarget.ToogleValue = Boolean.Parse(toogleValueOptions[selectedToggleValue]);
        }
    }
}
