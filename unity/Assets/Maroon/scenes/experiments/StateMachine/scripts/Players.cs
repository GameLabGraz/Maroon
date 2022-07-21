using System.Collections;
using System.Collections.Generic;

namespace Maroon.CSE.StateMachine
{
    public class Players : IEnumerable
    {
        private List<Player> _players = new List<Player>();
        private int _actualPlayer = 0;

        public Player GetPlayerAtIndex(int index)
        {
            if (index >= _players.Count)
            {
                return null;
            }

            return _players[index];
        }

        public IEnumerator GetEnumerator()
        {
            return _players.GetEnumerator();
        }


        public void AddPlayer(Player player)
        {
            _players.Add(player);
        }

        public Player GetPlayerByName(string name)
        {
            return _players.Find(element => element._playerName == name);
        }

        public Player GetNextPlayer()
        {
            Player player = GetPlayerAtIndex(_actualPlayer + 1);

            if (player == null)
            {
                _actualPlayer = 0;
                return GetPlayerAtIndex(0);
            }

            _actualPlayer++;
            return player;
        }

        public Player GetUserPlayer()
        {
            return _players.Find(element => element._isUser == true);
        }

        public void ResetPlayerCounter()
        {
            _actualPlayer = 0;
        }
    }
}
