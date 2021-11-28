using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenario1
{
    private Figure _figure;
    private List<List<Field>> _map;
    private bool _isInitialised = false;

    public Scenario1 (List<List<Field>> map) {
        _map = map;
    }

    private void InitScenario() {
        //Init first map
        foreach (List<Field> column in _map) {
            foreach(Field field in column) {
                
                Figure figure = field.GetFigure();
                
                if (figure && figure.gameObject.name != "pawn.003" && figure.gameObject.name != "pawn.003black") {
                    field.RemoveFigure();
                } 

                if (figure) {
                    _figure  = figure;
                    int indexRow = _map.IndexOf(column);
                    int indexColumn = 0;
                    if (indexRow < _map.Count) {
                        indexColumn = _map[indexRow].IndexOf(field);
                    }
                    _figure._positionRow = indexRow;
                    _figure._positionColumn = indexColumn;
                }              
            }
        }
        _isInitialised = true;
    }

    public List<List<Field>> GetMap() {
        if (!_isInitialised) {
            this.InitScenario();
        }
        return _map;
    }

    public Figure GetFigure() {
        if (!_isInitialised) {
            this.InitScenario();
        }
        return _figure;
    }

}
