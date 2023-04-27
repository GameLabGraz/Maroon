using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFieldOfView : MonoBehaviour
{
    public Camera fovCamera;
    public Slider fovSlider;

    void Start()
    {
        if (fovCamera == null)
        {
            fovCamera = Camera.main;
        }

        if (fovSlider != null)
        {
            fovSlider.minValue = 10;
            fovSlider.maxValue = 200;
            fovSlider.value = fovCamera.fieldOfView;
            fovSlider.onValueChanged.AddListener(OnFOVSliderValueChanged);
        }
    }

    private void OnFOVSliderValueChanged(float value)
    {
        fovCamera.fieldOfView = value;
    }
}