namespace Maroon.CSE.StateMachine
{
    public class State
    {
        private string _stateName;

        public State(string name)
        {
            _stateName = name;
        }

        public void SetStateName(string name)
        {
            _stateName = name;
        }

        public bool IsStartState()
        {
            return _stateName.Equals("Start");
        }

        public bool IsEndState()
        {
            return _stateName.Equals("End");
        }

        public string GetStateName()
        {
            return _stateName;
        }
    }
}
