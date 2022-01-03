using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class EnemyMoves : IEnumerable
{
    private List<EnemyMove> _enemyMoves = new List<EnemyMove>();
    
    public void AddEnemyMove(EnemyMove enemyMove) {
         _enemyMoves.Add(enemyMove);
    }
    public IEnumerator GetEnumerator() {
        return _enemyMoves.GetEnumerator();
    }

    public EnemyMove GetNextMove() {
        // TODO find good variant
        if (_enemyMoves.Count > 0) {
            return _enemyMoves[0];
        }
        return null;
    }
}
