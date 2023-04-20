using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFieldOfView : MonoBehaviour
{
    public Camera mainCamera;
    public Slider fovSlider;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (fovSlider != null)
        {
            fovSlider.minValue = 1;
            fovSlider.maxValue = 179;
            fovSlider.value = mainCamera.fieldOfView;
            fovSlider.onValueChanged.AddListener(OnFOVSliderValueChanged);
        }
    }

    private void OnFOVSliderValueChanged(float value)
    {
        mainCamera.fieldOfView = value;
    }
}