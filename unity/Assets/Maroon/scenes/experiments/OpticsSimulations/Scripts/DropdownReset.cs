using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maroon.UI;

public class DropdownReset : MonoBehaviour
{

    public void resetDropDownMenu()
    {
        var dropdown = gameObject.GetComponent<Dropdown>();

        dropdown.ResetObject();
    }

}
