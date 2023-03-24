using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class WallControler : MonoBehaviour
{

    public ComputeShader compute;
    public Button Top;
    public Button Bottom;
    public Button Left;
    public Button Right;
    private bool clickedTop;
    private bool clickedBottom;
    private bool clickedLeft;
    private bool clickedRight;
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
        }
        else
        {
            clickedTop = true;
            Top.image.color = Color.green;
        }
        compute.SetBool("Top", clickedTop);
    }
    public void setBottom()
    {

        //Top.isPressed()
        if (clickedBottom)
        {
            Bottom.image.color = Color.red;
            clickedBottom = false;
        }
        else
        {
            clickedBottom = true;
            Bottom.image.color = Color.green;
        }
        compute.SetBool("Bottom", clickedBottom);
    }
    public void setLeft()
    {

        //Top.isPressed()
        if (clickedLeft)
        {
            Left.image.color = Color.red;
            clickedLeft = false;
        }
        else
        {
            clickedLeft = true;
            Left.image.color = Color.green;
        }
        compute.SetBool("Left", clickedLeft);
    }
    public void setRight()
    {

        //Top.isPressed()
        if (clickedRight)
        {
            Right.image.color = Color.red;
            clickedRight = false;
        }
        else
        {
            clickedRight = true;
            Right.image.color = Color.green;
        }
        compute.SetBool("Right", clickedRight);
    }


    public void removeTop()
    {
        compute.SetBool("Top", false);
    }

}
