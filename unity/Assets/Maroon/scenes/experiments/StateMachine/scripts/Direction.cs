using System.Collections.Generic;
using UnityEngine;
using Maroon.CSE.StateMachine;

public class Direction
{
    private string _name;

    private string _directionKey;
    // +1 = move up
    // -1 = move down
    private int _columnMovement;
    // +1 = move right
    // -1 = move left
    private int _rowMovement;

    public Direction (string directionName, string directionKey, int rowMovement, int columnMovement) {
        _name = directionName;
        _directionKey = directionKey;
        _columnMovement =  columnMovement;
        _rowMovement = rowMovement;
    }

    public string GetDirectionName() {
        return _name;
    }

    public string GetDirectionKey()
    {
        return _directionKey;
    }

    public int GetColumnMovementFactor() {
        return _columnMovement;
    }

    public int GetRowMovementFactor() {
        return _rowMovement;
    }

    public void SetDirectionName(string directionName) {
        _name = directionName;
    }
}
