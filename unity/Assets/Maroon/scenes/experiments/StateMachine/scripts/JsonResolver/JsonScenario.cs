using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JsonScenario 
{
    public JsonFigure[] figures;
    public JsonFigure[] figuresToMove;
    public JsonDirection[] directions;
    public JsonEnemyMove[] enemyMoves;
    public string description;
    public string playerToPlay;
    public string destination;
}
