using System.Collections;
using System.Collections.Generic;
using Maroon.CSE.StateMachine;
public class Ruleset
{
    private State _startState;
    private State _endState;
    private Figure _figure;
    private Direction _direction;
    private Mode _mode;
    
    private int _id = 0;

    private List<List<SurroundingField>> _surrounding;

    public Ruleset(State start, State end, Direction direction, Mode mode, Figure figure, List<List<SurroundingField>> surrounding, int id) {
        _startState = start;
        _endState = end;
        _direction = direction;
        _mode = mode;
        _figure = figure;
        _surrounding = surrounding;
        _id = id;
    }

    public List<string> ToStringArray() {
        List<string> data = new List<string> {
            _startState.GetStateName(),
            _endState.GetStateName(),
            _direction.GetDirectionName(),
            _mode.GetModeName()
        };
        return data;
    }
    
    public int GetId() {
        return _id;
    }

    public State GetStartState() {
        return _startState;
    }

    public bool IsSameRule(Ruleset newRuleset) {

        List<List<SurroundingField>> surroundingToCheck = newRuleset.GetSurrounding();

        if (HasSameStartStateName(newRuleset) && HasSameEndStateName(newRuleset) && 
            HasSameDirection(newRuleset) && HasSameMode(newRuleset) &&
            HasSameSurrounding(surroundingToCheck)) {
                return true;
        }

        return false;
    }

    public bool IsRuleNotDesidable(Ruleset newRuleset) {
        
        List<List<SurroundingField>> surroundingToCheck = newRuleset.GetSurrounding();

        if (HasSameStartStateName(newRuleset) && HasSameEndStateName(newRuleset) && 
            HasSameSurrounding(surroundingToCheck)) {
            return true;
        }
        return false;
    }

    private bool HasSameSurrounding(List<List<SurroundingField>> surroundingToCheck) {
        if (_surrounding.Count != surroundingToCheck.Count) {
            return false;
        }
        for (int counter = 0; counter < surroundingToCheck.Count; counter++) {
            if (_surrounding[counter].Count != surroundingToCheck[counter].Count) {
                return false;
            }
            for(int counter2 = 0; counter2 < surroundingToCheck[counter].Count; counter2++) {
                if (_surrounding[counter][counter2].GetValue() != surroundingToCheck[counter][counter2].GetValue()) {
                    return false;
                }
            }
        }
        return true;
    }

    public bool HasSameStartStateName(Ruleset newRuleset) {
        return GetStartState().GetStateName() == newRuleset.GetStartState().GetStateName();
    }

    public bool HasSameEndStateName(Ruleset newRuleset) {
        return GetEndState().GetStateName() == newRuleset.GetEndState().GetStateName();
    }

    public bool HasSameDirection(Ruleset newRuleset) {
        return GetDirection().GetDirectionName() == newRuleset.GetDirection().GetDirectionName();
    }

    public bool HasSameMode(Ruleset newRuleset) {
        return GetMode().GetModeName() == newRuleset.GetMode().GetModeName();
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

    public List<List<SurroundingField>> GetSurrounding() {
        return _surrounding;
    }

}
