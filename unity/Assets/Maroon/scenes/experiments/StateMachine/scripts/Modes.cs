using System.Collections;
using System.Collections.Generic;

namespace Maroon.CSE.StateMachine
{
    public class Modes : IEnumerable
    {
        private List<Mode> _modes = new List<Mode>();

        public void AddMode(Mode mode)
        {
            _modes.Add(mode);
        }

        public IEnumerator GetEnumerator()
        {
            return _modes.GetEnumerator();
        }

        public Mode FindMode(string name)
        {
            return _modes.Find(element => element.GetModeName() == name);
        }
    }
}
