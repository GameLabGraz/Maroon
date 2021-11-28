using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class Players
{
    private List<Player> _players = new List<Player>();

    public Player GetPlayerAtIndex(int index) {
        if (index >= _players.Count) {
            return null;
        }
        return _players[index];
    }

    public void AddPlayer(Player player) {
        _players.Add(player);
    }
}
