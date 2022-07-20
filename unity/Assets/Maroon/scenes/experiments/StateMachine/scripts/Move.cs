using System.Collections.Generic;
using UnityEngine;
using Maroon.CSE.StateMachine;
using System.Linq;

public class Move : MonoBehaviour
{
    private List<Direction> _directions = new List<Direction>();
    // row movement
    private int _x;
    // column movement
    private int _y;

    public Move(int x, int y) {
        _x = x;
        _y = y;
    }
    
    public bool IsDirectionPossible(Direction directionToCheck) {
        return _directions.Any(direction => direction == directionToCheck);
    }

}
