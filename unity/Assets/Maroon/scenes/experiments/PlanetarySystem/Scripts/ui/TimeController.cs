using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    [SerializeField] private Slider timeSlider;

    /*
     * 
     */
    void Start()
    {
        timeSlider.minValue = 0f;
        timeSlider.maxValue = 20f;
        Time.timeScale = timeSlider.value;

        timeSlider.onValueChanged.AddListener(OnTimeSliderValueChanged);
    }

    /*
     * 
     */
    void OnTimeSliderValueChanged(float value)
    {
        Time.timeScale = value;
    }
}