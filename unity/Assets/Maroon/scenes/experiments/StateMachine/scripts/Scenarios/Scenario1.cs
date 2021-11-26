using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenario1
{

    public Scenario1 () {
    }

    public List<List<Field>> InitScenario(List<List<Field>> map) {

        //Init first map
        foreach (List<Field> column in map) {
            foreach(Field field in column) {
                
                Figure figure = field.GetFigure();
                
                if (figure && figure.gameObject.name != "pawn.003" && figure.gameObject.name != "pawn.003black") {
                    field.RemoveFigure();
                }               
            }
        }

        return map;
    }
}
