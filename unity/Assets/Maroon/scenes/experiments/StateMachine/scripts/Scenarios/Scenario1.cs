using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class Scenario1
{
    private Figure _figure;
    private Map _map;
    public Scenario1 (Map map) {
        _map = map;
    }

    public void InitScenario(Players players) {
        
        int scenarioId = 1;
        loadJson(scenarioId);

        return;


        //Init first map
        /* foreach (List<Field> column in _map) {
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
        } */
    }

    private void loadJson(int scenarioId) {

        // load json file and resolve it to object
        var scenarioFile = Resources.Load<TextAsset>("scenario" + scenarioId).ToString();
        var jsonScenario = JsonUtility.FromJson<JsonScenario>(scenarioFile);

        // remove every figure from map#
        _map.ClearMap();
        
        // initialise new map
        var fig = jsonScenario.figures;
        foreach (var figureToMove in jsonScenario.figures)
        {
            moveFigureToDestination(figureToMove);
        }
        return;
    }

    private void moveFigureToDestination(JsonFigure jsonFigureToMove) {

        // get destination field
        
        ChessBoardColumn column = (ChessBoardColumn)Enum.Parse(typeof(ChessBoardColumn), jsonFigureToMove.column);
        
        if (column == null || jsonFigureToMove.row <= 0) {
            return;
        }

        Field field = _map.GetFieldByIndices((int)column, jsonFigureToMove.row - 1);

        if (field == null) {
            return;
        }

        // get figure object
        GameObject figuresGameObject = GameObject.Find("figures");
        GameObject figureGameObject = figuresGameObject.transform.Find(jsonFigureToMove.name).gameObject;
        
        if (figureGameObject == null) {
            return;
        }

        Figure figureToMove = figureGameObject.GetComponent(typeof(Figure)) as Figure;
        
        if (figureToMove == null) {
            return;
        }

        

        // move figure to field
        Vector3 position = figureToMove.transform.position;

        int rowMovementFactor = (jsonFigureToMove.row - 1) - figureToMove._positionRow;
        int columnMovementFactor = (int)column - figureToMove._positionColumn;

        figureToMove.transform.position = new Vector3(position.x + (float)0.18 * columnMovementFactor, position.y + (float)0.18 * rowMovementFactor, position.z);

        figureToMove._positionColumn = (int)column;
        figureToMove._positionRow = jsonFigureToMove.row - 1;

        figureToMove.gameObject.SetActive(true);
        
        return;
    }

    public enum ChessBoardColumn {
        a = 0,
        b = 1,
        c = 2,
        d = 3,
        e = 4,
        f = 5,
        g = 6,
        h = 7
    }
}
