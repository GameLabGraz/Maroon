using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class Gamefield
{
    private List<List<Field>> _map;

    public Gamefield(List<Field> gamefield) {
        int rowLength = 8;

        if (gamefield.Count != 64) {
            // Is not possible
        }

        // init map
        for (int counter = 0; counter < gamefield.Count; counter++) {

            List<Field> tempList = new List<Field>();

            // Todo test
            if ((counter + 1) % rowLength == 0 && counter != 0) {
                tempList.Add(gamefield[counter]);
                _map.Add(tempList);
                tempList = new List<Field>();
            } else {
                tempList.Add(gamefield[counter]);
            }
        } 
    }

    public Field getField(int positionX, int positionY) {
        return _map[positionY][positionX];
    }
}
