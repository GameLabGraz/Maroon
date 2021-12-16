using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

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
public class Scenario
{
    private Map _map;
    private List<string> _scenarios;
    public Scenario (Map map) {
        _map = map;
    }

    
    public void InitScenario(Players players, string scenarioName) {
        InitScenariosList();
        LoadJson(players, scenarioName);
        return;
    }
    public List<string> GetScenarioList() {
        return _scenarios;
    }

    private void InitScenariosList() {
        _scenarios = new List<string>();
        var files = Directory.EnumerateFiles(@"Assets/Maroon/scenes/experiments/StateMachine/Resources", "*.json");
        foreach (var file in files) {
            string name = Path.GetFileNameWithoutExtension(file);
            if (name != null) {
                _scenarios.Add(name);
            }
        }
    }
    private void LoadJson(Players players, string scenarioName) {

        // load json file and resolve it to object
        string scenarioFile;
        JsonScenario jsonScenario;

        try {
            scenarioFile = Resources.Load<TextAsset>(scenarioName).ToString();
            jsonScenario = JsonUtility.FromJson<JsonScenario>(scenarioFile);
        } catch (Exception ex) {
            Debug.LogFormat("There exists no file with name {0}.json", scenarioName);
            return;
        }

        // remove every figure from map
        _map.ClearMap();
        
        // initialise not moveable figures
        int index = 0;
        foreach (var figureToMove in jsonScenario.figures)
        {
            moveFigureToDestination(players, figureToMove, scenarioName, index);
            index++;
        }

        // initialse moveable figures
        index = 0;
        foreach (var figureToMove in jsonScenario.figuresToMove) {
            moveFigureToDestination(players, figureToMove, scenarioName, index);
            index++;
        }

        if (players.getPlayerByName(jsonScenario.playerToPlay).GetFigures() == null) {
            Debug.LogFormat("There is no figure to move definded ({0}.json)", scenarioName);
            return;
        }

        return;
    }

    private void moveFigureToDestination(Players players, JsonFigure jsonFigureToMove, string scenarioName, int figureLineNumber) {

        // get destination field
        ChessBoardColumn column;
        try {
            column = (ChessBoardColumn)Enum.Parse(typeof(ChessBoardColumn), jsonFigureToMove.column);
        } catch (Exception ex) {
            if (jsonFigureToMove.column == null) {
                Debug.LogFormat("There is no column defined (figures: line {0}, {1}.json)", figureLineNumber, scenarioName);
                return;
            } 
            Debug.LogFormat("There exists no column {0} on the map (figures: line {1}, {2}.json)", jsonFigureToMove.column, figureLineNumber, scenarioName);
            return;
        }

        if (column == null || jsonFigureToMove.row <= 0) {
            Debug.LogFormat("There exists no row {0} where {1} should be moved (figures: line {2}, {3}.json)", (jsonFigureToMove.row), jsonFigureToMove.name, figureLineNumber, scenarioName);
            return;
        }

        Field field = _map.GetFieldByIndices((int)column, jsonFigureToMove.row - 1);

        if (field == null) {
            Debug.LogFormat("There exists no field {0}{1} where {2} should be placed (figures: line {3}, {4}.json)", column, (jsonFigureToMove.row), jsonFigureToMove.name, figureLineNumber, scenarioName);
            return;
        }

        if (field.GetFigure() != null) {
            Debug.LogFormat("There already has been placed {0} on field field {1}{2} where {3} should be placed (figures: line {4}, {5}.json)", field.GetFigure().gameObject.name, column, (jsonFigureToMove.row), jsonFigureToMove.name, figureLineNumber, scenarioName);
            return;
        }

        if (jsonFigureToMove.name == null) {
            Debug.LogFormat("No figure to move is given (figures: line {0}, {1}.json)", figureLineNumber, scenarioName);
            return;
        }

        // get figure object
        GameObject figuresGameObject = GameObject.Find("figures");
        Transform figureTransform = figuresGameObject.transform.Find(jsonFigureToMove.name);

        if (figureTransform == null) {
            Debug.LogFormat("{0} is no valid figure (figures: line {1}, {2}.json)", jsonFigureToMove.name, figureLineNumber, scenarioName);
            return;
        }

        GameObject figureGameObject = figureTransform.gameObject;
        
        if (figureGameObject == null) {
            return;
        }

        Figure figureToMove = figureGameObject.GetComponent(typeof(Figure)) as Figure;
        
        if (figureToMove == null) {
            return;
        }

        // Find player depending on figure
        foreach (Player player in players) {
            if (jsonFigureToMove.name.Contains(player._playerName)) {
                figureToMove._player = player;
                player.GetFigures().AddFigure(figureToMove);
                break;
            }
        }

        if (figureToMove._player == null) {
            Debug.LogFormat("No defined player could be found for {0} (figures: line {1}, {2}.json)", jsonFigureToMove.name, figureLineNumber, scenarioName);
            return;
        }

        // move figure to field
        Vector3 position = figureToMove.transform.position;

        int rowMovementFactor = (jsonFigureToMove.row - 1) - figureToMove._positionRow;
        int columnMovementFactor = (int)column - figureToMove._positionColumn;

        figureToMove.transform.position = new Vector3(position.x + (float)0.18 * columnMovementFactor, position.y + (float)0.18 * rowMovementFactor, position.z);
        
        figureToMove._positionColumn = (int)column;
        figureToMove._positionRow = jsonFigureToMove.row - 1;
        
        field.SetFigure(figureToMove);
        figureToMove.gameObject.SetActive(true);
        
        return;
    }
}
