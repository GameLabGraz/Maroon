using UnityEngine;

namespace PlatformControls.PC
{
    public class PC_SliderGroup : MonoBehaviour
    {
        [SerializeField]
        private PC_Slider _slider;

        [SerializeField]
        private PC_InputField _inputField;

        private void Start()
        {
            if (!_slider || !_inputField)
                return;

            _inputField.SetText(_slider.value);
        }
    }
}
