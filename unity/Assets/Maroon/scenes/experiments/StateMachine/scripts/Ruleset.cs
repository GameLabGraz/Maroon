using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;


public class Ruleset
{

    private State _startState;
    private State _endState;
    private Figure _figure;
    private Direction _direction;
    private int _moveDistance = 1;
    private Mode _mode;

    public Ruleset(State start, State end, Direction direction, Mode mode, Figure figure) {
        _startState = start;
        _endState = end;
        _direction = direction;
        _mode = mode;
        _figure = figure;
    }

    public List<string> ToStringArray() {
        List<string> data = new List<string>();
        data.Add(_startState.getStateName());
        data.Add(_endState.getStateName());
        data.Add(_direction.getDirectionName());
        data.Add(_mode.getModeName());
        return data;
    }

}
