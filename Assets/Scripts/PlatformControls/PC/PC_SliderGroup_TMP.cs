using TMPro;
using UnityEngine;

namespace PlatformControls.PC
{
    public class PC_SliderGroup_TMP : MonoBehaviour
    {
        [SerializeField]
        private PC_Slider _slider;

        [SerializeField]
        private TMP_InputField _inputField;
        
        [SerializeField]
        private string _textFormat = "F";
        private void Start()
        {
            if (!_slider || !_inputField)
                return;
            
            _inputField.textComponent.text = _slider.value.ToString(_textFormat);
            var obj = _inputField.textComponent.gameObject.GetComponent<PC_InputParser_Float_TMP>();
            if (obj != null)
            {
                obj.onValueChangedFloat.AddListener(UpdateSliderValue);
            }
            
            _slider.onValueChanged.AddListener(UpdateInputFieldValue);
        }



        private void UpdateInputFieldValue(float newValue)
        {
            _inputField.text = _slider.value.ToString(_textFormat);
        }

        private void UpdateSliderValue(float newValue)
        {
            _slider.SetSliderValue(newValue);
        }
    }
}
