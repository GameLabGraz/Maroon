using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Text))]
    public class TextDisplay : ValueDisplay
    {
        [SerializeField]
        private string _unit;

        private Text _text;

        private void Start()
        {
            _text = GetComponent<Text>();
        }

        private void FixedUpdate()
        {
            _text.text = GetValue<float>() + " " + _unit;
        }
    }
}
