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

    /**
     * on mouse click event listener on every island to then select it
     * */
    private void OnMouseDown()
    {
        string text = this.name;
        //Debug.Log("OnMouseDown " + text);
        StartCoroutine(MSTController.Instance.SelectIsland(text));
    }
}
