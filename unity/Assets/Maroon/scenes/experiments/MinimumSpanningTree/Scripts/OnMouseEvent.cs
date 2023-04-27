using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMouseEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {       
    }

    private void OnMouseDown()
    {
        string text = this.name;
        Debug.Log("OnMouseDown " + text);
        //StartMSTExperiment.Instance.SelectIsland(text);
        StartCoroutine(MSTController.Instance.SelectIsland(text));
    }
}
