using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;
using UnityEngine.UI;

public class State
{
    private string _stateName;

    public State(string name) {
        _stateName = name;
    }

    public void setStateName(string name) {

        //_stateName = name;
    }

    public bool isStartState() {
        return _stateName.Equals("Start");
    }
     public bool isEndState() {
        return _stateName.Equals("End");
    }
    public string getStateName() {
        return _stateName;
    }
}
