using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : MonoBehaviour
{
    private bool justCreated = false;

    public bool JustCreated
    {
        get { return justCreated; }
        set { justCreated = value; }
    }

}
