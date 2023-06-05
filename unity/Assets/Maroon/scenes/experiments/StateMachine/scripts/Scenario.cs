using System;
using GEAR.Localization;
using Maroon.CSE.StateMachine.JsonData;
using UnityEngine;
using TMPro;

namespace Maroon.CSE.StateMachine
{
    public enum ChessBoardColumn
    {
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
        private bool _mustHitAllEnemies;

        private string _scenarioName;

        public Scenario(Map map)
        {
            _map = map;
        }

        public bool MustHitAllEnemies()
        {
            return _mustHitAllEnemies;
        }

        public void InitScenario(Players players, EnemyMoves enemyMoves, TextAsset scenarioData)
        {
            _scenarioName = scenarioData.name;
            LoadJson(players, enemyMoves, scenarioData);
        }

        private void LoadJson(Players players, EnemyMoves enemyMoves, TextAsset scenarioData)
        {

            // load json file and resolve it to object
            JsonScenario jsonScenario;

            try
            {
                jsonScenario = JsonUtility.FromJson<JsonScenario>(scenarioData.ToString());
            }
            catch (Exception ex)
            {
                Debug.LogFormat($"There exists no file with name {_scenarioName}.json");
                return;
            }

            // remove every figure from map
            _map.ClearMap();

            // initialise not moveable figures
            int index = 0;
            foreach (var figureToMove in jsonScenario.figures)
            {
                MoveFigureToDestination(players, figureToMove, index);
                index++;
            }

            foreach (Player player in players)
            {
                player._isUser = jsonScenario.playerToPlay == player._playerName ? true : false;
            }

            // initialise moveable figures
            index = 0;
            foreach (var figureToMove in jsonScenario.figuresToMove)
            {
                MoveFigureToDestination(players, figureToMove, index);
                index++;
            }

            if (players.GetPlayerByName(jsonScenario.playerToPlay).GetFigures() == null)
            {
                Debug.LogFormat($"There is no figure to move definded ({_scenarioName}.json)");
                return;
            }

            GameObject descriptionObject = GameObject.Find("ScenarioDescription");
            TextMeshProUGUI descriptionTextMesh = descriptionObject.GetComponent<TextMeshProUGUI>();
            if (descriptionTextMesh != null)
            {
                LanguageManager.Instance.OnLanguageChanged.AddListener(language =>
                {
                    descriptionTextMesh.text =
                        LanguageManager.Instance.GetString(jsonScenario.description, language);
                });
                descriptionTextMesh.text = LanguageManager.Instance.GetString(jsonScenario.description);
            }

            GameObject destination = GameObject.Find(jsonScenario.destination);
            if (destination)
            {
                var dest = destination.GetComponent<Field>();
                dest.SetDestination(true);
            }
            else
            {
                Debug.LogFormat($"There is no destination defined ({_scenarioName}.json)");
                return;
            }

            _mustHitAllEnemies = jsonScenario.mustHitAllEnemies == true ? true : false;

            InitialiseEnemyMoves(jsonScenario, enemyMoves);
            return;
        }

        private void InitialiseEnemyMoves(JsonScenario jsonScenario, EnemyMoves enemyMoves)
        {
            // initialise directions for enemies moves
            Directions enemyDirections = new Directions();

            foreach (var direction in jsonScenario.directions)
            {
                enemyDirections.AddDirection(new Direction(direction.name, direction.name, direction.rowMovement,
                    direction.columnMovement));
            }

            // initialise enemies moves
            foreach (var enemyMove in jsonScenario.enemyMoves)
            {

                Direction direction = enemyDirections.FindDirection(enemyMove.move);

                if (direction == null)
                {
                    continue;
                }

                Figure figureToMove = GetFigure(enemyMove.enemyFigureToMove);

                if (figureToMove == null)
                {
                    continue;
                }

                enemyMoves.AddEnemyMove(new EnemyMove(figureToMove, direction));
            }

            int counter = 0;
        }


        private void MoveFigureToDestination(Players players, JsonFigure jsonFigureToMove, int figureLineNumber)
        {

            // get destination field
            ChessBoardColumn column;
            try
            {
                column = (ChessBoardColumn)Enum.Parse(typeof(ChessBoardColumn), jsonFigureToMove.column);
            }
            catch (Exception ex)
            {
                if (jsonFigureToMove.column == null)
                {
                    Debug.LogFormat(
                        $"There is no column defined (figures: line {figureLineNumber}, {_scenarioName}.json)");
                    return;
                }

                Debug.LogFormat(
                    $"There exists no column {jsonFigureToMove.column} on the map (figures: line {figureLineNumber}, {_scenarioName}.json)");
                return;
            }

            if (column == null || jsonFigureToMove.row <= 0)
            {
                Debug.LogFormat(
                    $"There exists no row {jsonFigureToMove.row} where {jsonFigureToMove.name} should be moved (figures: line {figureLineNumber}, {_scenarioName}.json)");
                return;
            }

            Field field = _map.GetFieldByIndices((int)column, jsonFigureToMove.row - 1);

            if (field == null)
            {
                Debug.LogFormat(
                    $"There exists no field {column}{jsonFigureToMove.row} where {jsonFigureToMove.name} should be placed (figures: line {figureLineNumber}, {_scenarioName}.json)");
                return;
            }

            if (field.GetFigure() != null)
            {
                Debug.LogFormat(
                    $"There already has been placed {field.GetFigure().gameObject.name} on field field {column}{jsonFigureToMove.row} where {jsonFigureToMove.name} should be placed (figures: line {figureLineNumber}, {_scenarioName}.json)");
                return;
            }

            if (jsonFigureToMove.name == null)
            {
                Debug.LogFormat($"No figure to move is given (figures: line {figureLineNumber}, {_scenarioName}.json)");
                return;
            }

            Figure figureToMove = GetFigure(jsonFigureToMove.name);

            if (figureToMove == null)
            {
                return;
            }

            // Find player depending on figure
            foreach (Player player in players)
            {
                if (jsonFigureToMove.name.Contains(player._playerName))
                {
                    figureToMove._player = player;
                    player.GetFigures().AddFigure(figureToMove);
                    break;
                }
            }


            if (figureToMove._player == null)
            {
                Debug.LogFormat(
                    $"No defined player could be found for {jsonFigureToMove.name} (figures: line {figureLineNumber}, {_scenarioName}.json)");
                return;
            }

            // move figure to field
            Vector3 position = figureToMove.transform.position;

            int rowMovementFactor = (jsonFigureToMove.row - 1) - figureToMove._positionRow;
            int columnMovementFactor = (int)column - figureToMove._positionColumn;

            figureToMove.transform.position = new Vector3(position.x + (float)0.055 * columnMovementFactor, position.y,
                position.z + (float)0.055 * rowMovementFactor);

            figureToMove._positionColumn = (int)column;
            figureToMove._positionRow = jsonFigureToMove.row - 1;

            field.SetFigure(figureToMove);
            figureToMove.gameObject.SetActive(true);

            return;
        }

        private Figure GetFigure(string figureName)
        {

            // get figure object
            GameObject figuresGameObject = GameObject.Find("figures");
            Transform figureTransform = figuresGameObject.transform.Find(figureName);

            if (figureTransform == null)
            {
                // TODO update log
                // Debug.LogFormat($"{figureName} is no valid figure (figures: line {figureLineNumber}, {scenarioName}.json)");
                return null;
            }

            GameObject figureGameObject = figureTransform.gameObject;

            if (figureGameObject == null)
            {
                return null;
            }

            return figureGameObject.GetComponent<Figure>();
        }
    }
}
