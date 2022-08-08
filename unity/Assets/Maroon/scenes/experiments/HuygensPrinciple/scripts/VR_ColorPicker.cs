using GameLabGraz.BaseControls;
using UnityEngine;
using VRTK;

namespace PlatformControls.VR
{
    public class VR_ColorPicker : ColorPicker
    {
        private GameObject colorWheel;

        private void Awake()
        {
            if (GetComponent<VRTK_ControllerEvents>() == null)
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
            Value = (touchpadAxis.y + 1) / 2;
            var currColor = colorWheel.GetComponent<Renderer>().material.color;
            currColor.r = Value;
            currColor.g = Value;
            currColor.b = Value;
            colorWheel.GetComponent<Renderer>().material.color = currColor;
        }

        private void ChangeHueSaturation(Vector2 touchpadAxis, float touchpadAngle)
        {
            var normalAngle = touchpadAngle - 90;
            if (normalAngle < 0)
                normalAngle += 360;

            var rads = normalAngle * Mathf.Deg2Rad;
            var maxX = Mathf.Cos(rads);
            var maxY = Mathf.Sin(rads);

            var currX = touchpadAxis.x;
            var currY = touchpadAxis.y;

            var percentX = Mathf.Abs(currX / maxX);
            var percentY = Mathf.Abs(currY / maxY);

            Hue = normalAngle / 360f;
            Saturation = (percentX + percentY) / 2;
        }
    }
}
