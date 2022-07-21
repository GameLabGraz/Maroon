using System.Collections.Generic;

namespace Maroon.CSE.StateMachine
{
    public class SurroundingField
    {

        private int _value = 0;
        private List<string> _names = new List<string> { " ", "E", "W", "B" };

        public SurroundingField(int value = 0)
        {
            _value = value;
        }

        public void UpdateValue()
        {
            _value++;

            if (_value >= _names.Count)
            {
                _value = 0;
            }
        }

        public void SetValue(SurroundingValue value)
        {
            _value = (int)value;
        }

        public string GetName()
        {
            return _names[_value];
        }

        public int GetValue()
        {
            return _value;
        }
    }
}

