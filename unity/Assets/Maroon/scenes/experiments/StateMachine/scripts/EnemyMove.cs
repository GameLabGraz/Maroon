using System.Collections.Generic;
using UnityEngine;
using Maroon.CSE.StateMachine;

public class EnemyMove
{
    private Direction _direction;
    private Figure _figure;
    public EnemyMove(Figure figure, Direction direction) {
        _direction = direction;
        _figure = figure;
    }
    
    public Direction GetDirection() {
        return _direction;
    }

    public Figure GetFigure() {
        return _figure;
    }
}
