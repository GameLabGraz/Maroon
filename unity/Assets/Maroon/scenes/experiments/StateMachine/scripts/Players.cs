using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class Players : IEnumerable
{
    private List<Player> _players = new List<Player>();

    public Player GetPlayerAtIndex(int index) {
        if (index >= _players.Count) {
            return null;
        }
        return _players[index];
    }

    public IEnumerator GetEnumerator() {
        return _players.GetEnumerator();
    }


    public void AddPlayer(Player player) {
        _players.Add(player);
    }

    public Player getPlayerByName(string name) {
        return _players.Find(element => element._playerName == name);
    }
}
