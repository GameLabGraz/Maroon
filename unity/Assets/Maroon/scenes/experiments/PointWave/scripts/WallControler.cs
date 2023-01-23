using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class WallControler : MonoBehaviour
{

    public ComputeShader compute;
    public Button Top;
    private bool clickedTop;
    // Start is called before the first frame update
    void Start()
    {
        Top.image.color = Color.red;
        clickedTop = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setTop()
    {

        //Top.isPressed()
        if (clickedTop)
        {
             Top.image.color = Color.red;
            clickedTop = false;
            Debug.Log("de pressed");

        }
        else
        {
            clickedTop = true;
            Top.image.color = Color.green;
            Debug.Log("pressed");


        }

        compute.SetBool("Top", true);
      
    }

    public void removeTop()
    {
        compute.SetBool("Top", false);
    }

}
