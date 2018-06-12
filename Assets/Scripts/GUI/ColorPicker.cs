using UnityEngine;
using VRTK;

public class ColorPicker : MonoBehaviour
{

    [SerializeField]
    private GameObject colorCube;

    private float hue, saturation, value = 1f;

    private GameObject colorWheel;


	void Start ()
    {
	    if(GetComponent<VRTK_ControllerEvents>() == null)
        {
            Debug.LogError("Need VRTK_ControllerEvents component");
            return;
        }

        colorWheel = transform.Find("ColorWheel").gameObject;

        GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged += new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
	}

    private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
    {
        if (GetComponent<VRTK_ControllerEvents>().triggerPressed)
            ChangeValue(e.touchpadAxis);
        else
            ChangeHueSaturation(e.touchpadAxis, e.touchpadAngle);

        UpdateColor();
    }

    private void ChangeValue(Vector2 touchpadAxis)
    {
        value = (touchpadAxis.y + 1) / 2;
        Color currColor = colorWheel.GetComponent<Renderer>().material.color;
        currColor.r = value;
        currColor.g = value;
        currColor.b = value;
        colorWheel.GetComponent<Renderer>().material.color = currColor;
    }

    private void ChangeHueSaturation(Vector2 touchpadAxis, float touchpadAngle)
    {
        float normalAngle = touchpadAngle - 90;
        if (normalAngle < 0)
            normalAngle += 360;

        float rads = normalAngle * Mathf.Deg2Rad;
        float maxX = Mathf.Cos(rads);
        float maxY = Mathf.Sin(rads);

        float currX = touchpadAxis.x;
        float currY = touchpadAxis.y;

        float percentX = Mathf.Abs(currX / maxX);
        float percentY = Mathf.Abs(currY / maxY);

        hue = normalAngle / 360f;
        saturation = (percentX + percentY) / 2;
    }

    private void UpdateColor()
    {
        Color color = Color.HSVToRGB(hue, saturation, value);
        colorCube.GetComponent<Renderer>().material.color = color;
    }
}
