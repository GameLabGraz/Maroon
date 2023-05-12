using UnityEngine;
using UnityEngine.UI;

public class ChangeGravitationalConstantG : MonoBehaviour
{
    [SerializeField]
    private SolarSystem solarSystem;

    [SerializeField]
    private Slider gSlider;

    [SerializeField]
    private float minG = 9.81f;

    [SerializeField]
    private float maxG = 300f;

    [SerializeField]
    private float startG = 100f;

    private void Start()
    {
        gSlider.minValue = minG;
        gSlider.maxValue = maxG;
        gSlider.value = startG;
        solarSystem.G = startG;
        gSlider.onValueChanged.AddListener(OnGChanged);
    }

    private void OnGChanged(float value)
    {
        solarSystem.G = value;
    }
}
