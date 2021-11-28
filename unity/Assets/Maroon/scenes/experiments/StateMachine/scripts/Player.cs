using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class Player
{
        public string _playerName {get;set;}

        private Figure _figure = null;
    
    public Player(string name) {;
        _playerName = name;
    }

    public Figure GetFigure() {
        return _figure;
    }

    public void SetFigure(Figure figure) {
        _figure = figure;
    }

}
