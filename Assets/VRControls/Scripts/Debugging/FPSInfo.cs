using UnityEngine;
using UnityEngine.UI;

public class FPSInfo : MonoBehaviour
{
    private float m_DeltaTime;                      // This is the smoothed out time between frames.


    private const float k_SmoothingCoef = 0.1f;     // This is used to smooth out the displayed fps.

    public Text fpsText;
    public Image fpsPanel;


    private void Update()
    {
        // This line has the effect of smoothing out delta time.
        m_DeltaTime += (Time.deltaTime - m_DeltaTime) * k_SmoothingCoef;

        // The frames per second is the number of frames this frame (one) divided by the time for this frame (delta time).
        float fps = 1.0f / m_DeltaTime;

        // Set the displayed value of the fps to be an integer.
        fpsText.text = Mathf.FloorToInt(fps).ToString();

        // Turn the fps display on and off using the F key.
        if (Input.GetKeyDown(KeyCode.F))
        {
            fpsText.enabled = !fpsText.enabled;
        }

        if (fps >= 59)
        {
            fpsPanel.color = Color.green;
        }
        else if (fps >= 50)
        {
            fpsPanel.color = Color.yellow;
        }
        else
        {
            fpsPanel.color = Color.red;
        }
    }
}