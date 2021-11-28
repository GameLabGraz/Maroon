using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Direction
{
    private string _name;
    // +1 = move up
    // -1 = move down
    private int _columnMovement;
    // +1 = move right
    // -1 = move left
    private int _rowMovement;

    public Direction (string directionName, int columnMovement, int rowMovement) {
        _name = directionName;
        _columnMovement =  columnMovement;
        _rowMovement = rowMovement;
    }

    public string GetDirectionName() {
        return _name;
    }
    
    public int GetColumnMovementFactor() {
        return _columnMovement;
    }

    public int GetRowMovementFactor() {
        return _rowMovement;
    }
}
