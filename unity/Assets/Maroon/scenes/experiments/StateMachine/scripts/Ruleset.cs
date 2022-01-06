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

    private List<List<SurroundingField>> _surrounding;

    public Ruleset(State start, State end, Direction direction, Mode mode, Figure figure, List<List<SurroundingField>> surrounding) {
        _startState = start;
        _endState = end;
        _direction = direction;
        _mode = mode;
        _figure = figure;
        _surrounding = surrounding;
    }

    public List<string> ToStringArray() {
        List<string> data = new List<string>();
        data.Add(_startState.GetStateName());
        data.Add(_endState.GetStateName());
        data.Add(_direction.GetDirectionName());
        data.Add(_mode.GetModeName());
        return data;
    }

    public State GetStartState() {
        return _startState;
    }

     public State GetEndState() {
        return _endState;
    }

    public Direction GetDirection() {
        return _direction;
    }

    public Mode GetMode() {
        return _mode;
    }

}
