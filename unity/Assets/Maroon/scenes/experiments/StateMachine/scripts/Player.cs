using System.Collections.Generic;
using Maroon.CSE.StateMachine;

public class Player
{
        public string _playerName {get;set;}
        public bool _isUser {get;set;}

        private Figures _figuresToMove = new Figures();
    
    public Player(string name) {
        _playerName = name;
    }
   
     public Figures GetFigures() {
        return _figuresToMove;
    }

    public void RemoveFigures() {
        _figuresToMove = new Figures();
    }

}
