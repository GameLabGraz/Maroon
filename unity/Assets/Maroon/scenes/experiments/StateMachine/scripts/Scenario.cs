using System;
using System.IO;
using System.Collections.Generic;
using GEAR.Localization;
using UnityEngine;
using TMPro;

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
    private bool _mustHitAllEnemies;
    public Scenario (Map map) {
        _map = map;
    }

    public bool MustHitAllEnemies() {
        return _mustHitAllEnemies;
    }
    
    public void InitScenario(Players players, EnemyMoves enemyMoves, string scenarioName) {
        InitScenariosList();
        LoadJson(players, enemyMoves, scenarioName);
        return;
    }
    public List<string> GetScenarioList() {
        return _scenarios;
    }

    private void InitScenariosList() {
        _scenarios = new List<string>();
        var files = Directory.EnumerateFiles(@"Assets/Maroon/scenes/experiments/StateMachine/Resources/Scenarios", "*.json");
        foreach (var file in files) {
            string name = Path.GetFileNameWithoutExtension(file);
            if (name != null) {
                _scenarios.Add(name);
            }
        }
    }
    private void LoadJson(Players players, EnemyMoves enemyMoves, string scenarioName) {

        // load json file and resolve it to object
        string scenarioFile;
        JsonScenario jsonScenario;

        try {
            scenarioFile = Resources.Load<TextAsset>($"Scenarios/{scenarioName}").ToString();
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
        foreach (Player player in players) {
            if (jsonScenario.playerToPlay == player._playerName) {
                player._isUser = true;
            } else {
                player._isUser = false;
            }
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

        GameObject descriptionObject = GameObject.Find("ScenarioDescription");
        TextMeshProUGUI descriptionTextMesh = descriptionObject.GetComponent(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
        if (descriptionTextMesh != null) {
            LanguageManager.Instance.OnLanguageChanged.AddListener(language =>
            {
                descriptionTextMesh.text = LanguageManager.Instance.GetString(jsonScenario.description, language);
            });
            descriptionTextMesh.text = LanguageManager.Instance.GetString(jsonScenario.description);
        }

        GameObject destination = GameObject.Find(jsonScenario.destination);
        if (destination) {
            var dest = destination.GetComponent(typeof(Field)) as Field;
            dest.SetDestination(true);
        } else {
            Debug.LogFormat("There is destination definded ({0}.json)", scenarioName);
            return;
        }

        if (jsonScenario.mustHitAllEnemies) {
            _mustHitAllEnemies = true;
        } else {
            _mustHitAllEnemies = false;
        }

        InitialiseEnemyMoves(jsonScenario, enemyMoves);
        return;
    }

    private void InitialiseEnemyMoves(JsonScenario jsonScenario, EnemyMoves enemyMoves) {
        // initialise directions for enemies moves
        Directions enemyDirections = new Directions();

        foreach (var direction in jsonScenario.directions) {
            enemyDirections.AddDirection(new Direction(direction.name, direction.name, direction.rowMovement, direction.columnMovement));
        }

        // initialise enemies moves
        foreach (var enemyMove in jsonScenario.enemyMoves) {

            Direction direction = enemyDirections.FindDirection(enemyMove.move);
            
            if (direction == null) {
                continue;
            }
            
            Figure figureToMove = GetFigure(enemyMove.enemyFigureToMove);

            if (figureToMove == null) {
                continue;
            }
            enemyMoves.AddEnemyMove(new EnemyMove(figureToMove, direction));
        }
        int counter = 0;
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

        Figure figureToMove = GetFigure(jsonFigureToMove.name);
        
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

        figureToMove.transform.position = new Vector3(position.x + (float)0.055 * columnMovementFactor, position.y, position.z + (float)0.055 * rowMovementFactor);
        
        figureToMove._positionColumn = (int)column;
        figureToMove._positionRow = jsonFigureToMove.row - 1;
        
        field.SetFigure(figureToMove);
        figureToMove.gameObject.SetActive(true);
        
        return;
    }

    private Figure GetFigure(string figureName) {

        // get figure object
        GameObject figuresGameObject = GameObject.Find("figures");
        Transform figureTransform = figuresGameObject.transform.Find(figureName);

        if (figureTransform == null) {
            // TODO update log
            // Debug.LogFormat("{0} is no valid figure (figures: line {1}, {2}.json)", figureName, figureLineNumber, scenarioName);
            return null;
        }

        GameObject figureGameObject = figureTransform.gameObject;
        
        if (figureGameObject == null) {
            return null;
        }

        return figureGameObject.GetComponent(typeof(Figure)) as Figure;
    }
}
