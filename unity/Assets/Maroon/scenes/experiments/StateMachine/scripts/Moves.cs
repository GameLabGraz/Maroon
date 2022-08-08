using System.Collections.Generic;
using UnityEngine;

namespace Maroon.CSE.StateMachine
{
    public class Moves : MonoBehaviour
    {
        private List<Move> _moves = new List<Move>();

        public void addMove(Move newMove)
        {
            _moves.Add(newMove);
        }

        public void removeMoveAtPosition(int position)
        {
            _moves.RemoveAt(position);
        }

        public Move getMoveAtPosition(int position)
        {
            return _moves[position];
        }
    }
}
