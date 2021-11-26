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
               field.RemoveFigure();
            }
        }

        return map;
    }
}
