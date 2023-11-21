using GameLabGraz.VRInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterChangerHelper : MonoBehaviour
{    
    public void SetLinearDriveValue(string handleName, string unit, int newValue)
    {        
        VRLinearDrive theDriveIWant = FindParticularInactiveDriveInChildren("ExperimentDrawer", handleName);

        if (theDriveIWant != null)
        {
            GameObject parent = theDriveIWant.transform.parent.gameObject;
            //Debug.Log("Speech: its parent is named <" + parent.name + ">, and its active setting is <" + parent.activeSelf.ToString() + ">");

            bool previousParentActiveSetting = parent.activeSelf;

            if (previousParentActiveSetting.Equals(false))
                parent.SetActive(true);

            if (handleName.Equals("PropagationModeHandle"))
            {
                //Debug.Log("Speech: propagation mode is <" + unit + ">");
                if (unit.Equals("circular"))
                    theDriveIWant.ForceToValue(1);
                else if (unit.Equals("rectilinear"))
                    theDriveIWant.ForceToValue(0);
            }
            else if (unit.Equals("percent"))
            {
                //int percentizedValue = Mathf.RoundToInt((theDrive.maximum - theDrive.minimum) * (newValue / 100f) + theDrive.minimum);
                float percentizedValue = (theDriveIWant.maximum - theDriveIWant.minimum) * (newValue / 100f) + theDriveIWant.minimum;
                //Debug.Log("Speech: so, the new value would be..." + percentizedValue.ToString());
                theDriveIWant.ForceToValue(percentizedValue);
            }
            else
            {
                //Debug.Log("Speech: so they want to change value, not percent, to <" + newValue.ToString() + ">");
                theDriveIWant.ForceToValue(newValue);
            }
            parent.SetActive(previousParentActiveSetting);
        }
    }



    public VRHoverButton FindParticularInactiveButtonInChildren(string parentName, string childName)
    {
        GameObject theParent = GameObject.Find(parentName);
        if (theParent != null)
        {
            VRHoverButton[] theChildren = theParent.GetComponentsInChildren<VRHoverButton>(true);

            if (theChildren != null)
            {
                for (int i = 0; i < theChildren.Length; i++)
                {
                    VRHoverButton currButton = theChildren[i];
                    if (currButton.name.Equals(childName))
                    {
                        return currButton;
                    }
                }
            }
        }
        return null;
    }

    public VRLinearDrive FindParticularInactiveDriveInChildren(string parentName, string childName)
    {
        GameObject theParent = GameObject.Find(parentName);
        if (theParent != null)
        {            
            VRLinearDrive[] theChildren = theParent.GetComponentsInChildren<VRLinearDrive>(true);

            if (theChildren != null)
            {
                for (int i = 0; i < theChildren.Length; i++)
                {
                    VRLinearDrive currButton = theChildren[i];
                    if (currButton.name.Equals(childName))
                    {
                        return currButton;
                    }
                }
            }
        }
        return null;
    }

}
