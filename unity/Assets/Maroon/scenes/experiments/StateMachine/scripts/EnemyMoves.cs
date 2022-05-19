using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;
using GEAR.Localization;

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

        foreach(EnemyMove enemyMove in moves) {
            if ((enemyMove.GetDirection().GetDirectionName() == "hitleftdown" || enemyMove.GetDirection().GetDirectionName() == "hitrightdown" ||
                    enemyMove.GetDirection().GetDirectionName() == "hitleftup" || enemyMove.GetDirection().GetDirectionName() == "hitrightup")){//&&  enemyMove.GetFigure().gameObject.name.Contains("pawn")) {

                    Field field = map.GetFieldByIndices(enemyMove.GetFigure()._positionColumn + enemyMove.GetDirection().GetColumnMovementFactor(), enemyMove.GetFigure()._positionRow + enemyMove.GetDirection().GetRowMovementFactor());
                    if (field != null && field.GetFigure() != null && enemyMove.GetFigure()._player._playerName != field.GetFigure()._player._playerName) {
                        rulesets = new Rulesets();
                        Mode mode = modes.FindMode(LanguageManager.Instance.GetString("CaptureFigure"));
                        if (mode != null) {
                            rulesets.AddRuleset(new Ruleset(state, state, enemyMove.GetDirection(), mode, null, new List<List<SurroundingField>>(), 0));
                        }
                            return rulesets;
                        }
            } 
            else {
                
                Mode mode = modes.FindMode(LanguageManager.Instance.GetString("EnterEmptyField"));
                if (mode != null) {
                    rulesets.AddRuleset(new Ruleset(state, state, enemyMove.GetDirection(), mode, null, new List<List<SurroundingField>>(), 0));
                }
            }
        }
        
        return rulesets;
    }
}
