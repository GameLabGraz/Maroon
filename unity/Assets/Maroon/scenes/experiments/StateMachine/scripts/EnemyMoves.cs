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

    public Rulesets GetNextMove(string name, State state, Modes modes) {
        EnemyMove move = _enemyMoves.Find(element => element.GetFigure().gameObject.name == name);

        // TODO build from move one hit and one empty field move

        if (move == null) {
            return null;
        }

        // TODO check which mode is possible for a figure
        Rulesets rulesets = new Rulesets();

        foreach(Mode mode in modes) {
            if (mode.GetModeName() == "Figur schlagen" && move.GetFigure().gameObject.name.Contains("pawn")) {
                continue;
            }
            rulesets.AddRuleset(new Ruleset(state, state, move.GetDirection(), mode, null, new List<List<SurroundingField>>()));
        }
        return rulesets;
    }
}
