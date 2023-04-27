using UnityEngine;
using UnityEngine.UI;

public class CameraAngle : MonoBehaviour
{
    public Slider slider;
    public float distance = 10f;
    public float height = 5f;

    private Vector3 topViewPosition;
    private Quaternion topViewRotation;

    private void Start()
    {
        // Save the current position and rotation of the camera as the top view
        topViewPosition = transform.position;
        topViewRotation = transform.rotation;

        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        float angle = value * 90f;

        Vector3 position = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad) * distance,
            height,
            Mathf.Sin(angle * Mathf.Deg2Rad) * distance
        );

        Quaternion rotation = Quaternion.LookRotation(
            Vector3.zero - position,
            Vector3.up
        );

        transform.position = position;
        transform.rotation = rotation;
    }

    public void ResetCamera()
    {
        // Reset the camera to the top view position and rotation
        transform.position = topViewPosition;
        transform.rotation = topViewRotation;
    }
}