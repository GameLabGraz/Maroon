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

    public void SetStateName(string name) {

        //_stateName = name;
    }

    public bool IsStartState() {
        return _stateName.Equals("Start");
    }
     public bool IsEndState() {
        return _stateName.Equals("Ende");
    }
    public string GetStateName() {
        return _stateName;
    }
}
