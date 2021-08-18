using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NetworkComponentScript : MonoBehaviour, IPointerClickHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
        Debug.Log(name + " Game Object Clicked!");
        if (pointerEventData.clickCount >= 2)
        {
            //change to new scene
            Debug.Log(name + " Scene will be changed soon!!!!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
