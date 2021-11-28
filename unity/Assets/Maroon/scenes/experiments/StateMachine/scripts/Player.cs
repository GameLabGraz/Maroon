using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class Player
{
    private string _playerName;

    private Figure _figure;
   
   public Player(string name, Figure figure) {
       _figure = figure;
       _playerName = name;
   }

   public Figure GetFigure() {
       return _figure;
   }
}
