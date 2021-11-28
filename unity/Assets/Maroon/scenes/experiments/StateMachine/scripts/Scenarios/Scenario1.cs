using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenario1
{
    private Figure _figure;
    private List<List<Field>> _map;

    public Scenario1 (List<List<Field>> map) {
        _map = map;
    }

    public void InitScenario(Players players) {
        //Init first map
        foreach (List<Field> column in _map) {
            foreach(Field field in column) {
                
                Figure figure = field.GetFigure();
                
                // Defines which figures should not be removed from the field
                // TODO position changes of figures should also be made
                if (!figure) {
                    continue;
                }
                if (figure.gameObject.name != "pawn.003" && figure.gameObject.name != "pawn.003black") {
                    field.RemoveFigure();
                } else {
                    // add player to figure
                    if (figure.gameObject.name.Contains("black")) {
                        figure._player = players.GetPlayerAtIndex(1);
                    } else {
                        figure._player = players.GetPlayerAtIndex(0);
                    }
                }

                // Defines which figure should be moved by the player
                if (figure.gameObject.name == "pawn.003") {
                    _figure  = figure;
                    int indexColumn = _map.IndexOf(column);
                    int indexRow = 0;
                    if (indexColumn < _map.Count) {
                        indexRow = _map[indexColumn].IndexOf(field);
                    }
                    _figure._positionRow = indexRow;
                    _figure._positionColumn = indexColumn;
                    players.GetPlayerAtIndex(0).SetFigure(_figure);
                }              
            }
        }
    }

    public List<List<Field>> GetMap() {
        return _map;
    }
}
