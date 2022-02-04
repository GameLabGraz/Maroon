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

    public Rulesets GetNextMove(string name, State state, Modes modes, Map map) {
        List<EnemyMove> moves = _enemyMoves.FindAll(element => element.GetFigure().gameObject.name == name);

        if (moves == null) {
            return null;
        }

        // TODO check which mode is possible for a figure
        Rulesets rulesets = new Rulesets();

        foreach(Mode mode in modes) {
            foreach(EnemyMove enemyMove in moves) {
                if (mode.GetModeCode() == 1 && 
                    (enemyMove.GetDirection().GetDirectionName() == "hitleftdown" || enemyMove.GetDirection().GetDirectionName() == "hitrightdown" ||
                     enemyMove.GetDirection().GetDirectionName() == "hitleftup" || enemyMove.GetDirection().GetDirectionName() == "hitrightup")){//&&  enemyMove.GetFigure().gameObject.name.Contains("pawn")) {

                       Field field = map.GetFieldByIndices(enemyMove.GetFigure()._positionColumn + enemyMove.GetDirection().GetColumnMovementFactor(), enemyMove.GetFigure()._positionRow + enemyMove.GetDirection().GetRowMovementFactor());
                       if (field != null && field.GetFigure() != null && enemyMove.GetFigure()._player._playerName != field.GetFigure()._player._playerName) {
                           rulesets = new Rulesets();
                           rulesets.AddRuleset(new Ruleset(state, state, enemyMove.GetDirection(), mode, null, new List<List<SurroundingField>>()));
                           return rulesets;
                       }
                }
                rulesets.AddRuleset(new Ruleset(state, state, enemyMove.GetDirection(), mode, null, new List<List<SurroundingField>>()));

            }
            //if (mode.GetModeName() == "Figur schlagen" && move.GetFigure().gameObject.name.Contains("pawn")) {
            //    continue;
            //}
            //rulesets.AddRuleset(new Ruleset(state, state, move.GetDirection(), mode, null, new List<List<SurroundingField>>()));
        }
        return rulesets;
    }
}
