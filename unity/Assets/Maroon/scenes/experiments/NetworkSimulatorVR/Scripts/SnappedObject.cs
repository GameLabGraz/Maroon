using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnappedObject : MonoBehaviour
{
    // Start is called before the first frame update
    //[SerializeField]  public GameObject snappedObject = null;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("IP"))
        {
           
            Debug.Log("Do something.....\n");
        }
    }



}

