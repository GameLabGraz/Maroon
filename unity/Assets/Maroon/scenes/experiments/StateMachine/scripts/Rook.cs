using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class Brook : Figure
{
    [SerializeField] private Moves _moves;
    public bool isMovePossible(int positionX, int positionY) {
        // TODO implement it
        return true;
    }
}
