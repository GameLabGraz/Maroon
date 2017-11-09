using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : MonoBehaviour
{
    [SerializeField]
    private bool justCreated = true;

    public bool JustCreated
    {
        get { return justCreated; }
        set { justCreated = value; }
    }

}
