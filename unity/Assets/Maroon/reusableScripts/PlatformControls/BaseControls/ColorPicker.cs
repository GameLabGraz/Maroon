using UnityEngine;

namespace PlatformControls.BaseControls
{
    public class ColorPicker : MonoBehaviour
    {
        [SerializeField]
        private GameObject _colorObject;

        [SerializeField]
        private string _colorPropertyName = "_Color";

        private float _hue, _saturation, _value;

        public float Hue
        {
            get { return _hue;}
            set { _hue = value; UpdateColor(); }
        }

        public float Saturation
        {
            get { return _saturation; }
            set { _saturation = value; UpdateColor(); }
        }

        public float Value
        {
            get { return _value; }
            set { _value = value; UpdateColor(); }
        }

        protected virtual void Start()
        {
            _saturation = 1f;
            _value = 1f;
        }

        protected void UpdateColor()
        {
            if (!_colorObject) return;

            _colorObject.GetComponent<Renderer>().material.SetColor(_colorPropertyName, Color.HSVToRGB(_hue, _saturation, _value));
        }
    }
}