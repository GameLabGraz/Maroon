using UnityEngine;
using VRTK;

public class ColorPicker : MonoBehaviour
{

    [SerializeField]
    private GameObject colorObject;

    [SerializeField]
    private string colorPropertyName = "_Color";

    private float hue, saturation, value = 1f;

    private GameObject colorWheel;


	private void Awake ()
    {
	    if(GetComponent<VRTK_ControllerEvents>() == null)
        {
            Debug.LogError("Need VRTK_ControllerEvents component");
            return;
        }

        colorWheel = transform.Find("ColorWheel").gameObject;
    }

    private void OnEnable()
    {
        GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged += DoTouchpadAxisChanged;
        colorWheel.SetActive(true);
    }

    private void OnDisable()
    {
        GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged -= DoTouchpadAxisChanged;
        colorWheel.SetActive(false);
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
        colorObject.GetComponent<Renderer>().material.SetColor(colorPropertyName, Color.HSVToRGB(hue, saturation, value));
    }
}
