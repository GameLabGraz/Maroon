using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class ColorPicker : MonoBehaviour
{

    [SerializeField]
    private GameObject colorCube;

    private float hue, saturation, value = 1f;

    private GameObject blackWheel;


	void Start ()
    {
	    if(GetComponent<VRTK_ControllerEvents>() == null)
        {
            Debug.LogError("Need VRTK_ControllerEvents component");
            return;
        }

        blackWheel = transform.Find("CanvasHolder/Canvas/BlackWheel").gameObject;

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
        this.value = (touchpadAxis.y + 1) / 2;
        Color currColor = blackWheel.GetComponent<Image>().color;
        currColor.a = 1 - this.value;
        blackWheel.GetComponent<Image>().color = currColor;
    }

    private void ChangeHueSaturation(Vector2 touchpadAxis, float touchpadAngle)
    {
        float normalAngle = touchpadAngle - 90;
        if (normalAngle < 0)
            normalAngle += 360;

        Debug.Log("ChangeHueSaturation: axis: " + touchpadAxis + " angle: " + normalAngle);

        float rads = normalAngle * Mathf.Deg2Rad;
        float maxX = Mathf.Cos(rads);
        float maxY = Mathf.Sin(rads);

        float currX = touchpadAxis.x;
        float currY = touchpadAxis.y;

        float percentX = Mathf.Abs(currX / maxX);
        float percentY = Mathf.Abs(currY / maxY);

        this.hue = normalAngle / 360f;
        this.saturation = (percentX + percentY) / 2;

        Debug.Log(this.hue + " " + this.saturation);
    }

    private void UpdateColor()
    {
        Color color = Color.HSVToRGB(this.hue, this.saturation, this.value);
        colorCube.GetComponent<Renderer>().material.color = color;
    }

}
