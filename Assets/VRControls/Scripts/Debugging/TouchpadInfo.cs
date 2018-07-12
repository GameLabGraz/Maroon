using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TouchpadInfo : MonoBehaviour
{
    public Image touchPressedPanel;
    public Text touchDeltaText;

    public Color mousePressedColor;
    public Color mouseNotPressedColor;
    public Color backButtonPressedColor;

    void Start () {
	    
	}
	
	void Update () {
	    if (Input.GetMouseButton(0))
	    {
	        touchPressedPanel.color = mousePressedColor;
	    }
        else if (Input.GetMouseButton(1))
        {
            touchPressedPanel.color = backButtonPressedColor;
        }
	    else
	    {
	        touchPressedPanel.color = mouseNotPressedColor;
	    }

	    touchDeltaText.text = Input.GetAxis("Mouse X") + "\n" + Input.GetAxis("Mouse Y");
	}
}
