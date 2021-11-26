using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

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
    
    public bool isDirectionPossible(Direction directionToCheck) {
        for (int counter = 0; counter < _directions.Count; counter++) {
            if (_directions[counter] == directionToCheck) {
                return true;
            }
        }
        return false;
    }

}
