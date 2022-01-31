using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maroon.UI;
using Button = UnityEngine.UI.Button;
using VerticalLayoutGroup = UnityEngine.UI.VerticalLayoutGroup;
using Selectable = UnityEngine.UI.Selectable;
using TMPro;
using System.Threading;

namespace StateMachine {
    public class GameController : MonoBehaviour
    {
        private Players _players = new Players();
        private Rulesets _rulesets = new Rulesets();

        private Scenario _scenario;
        private States _states;
        private State _actualState;
        private Player _playerToPlay;
        private Direction _actualDirection;
        private Directions _directions = new Directions();
        private Moves _moves = new Moves();

        private EnemyMoves _enemyMoves = new EnemyMoves();
        private Modes _modes = new Modes();
        // Rule counter increases every time a rule is added, but does NOT decrease (so names are always unique)
        private int _ruleCounter = 0;
        private bool _isLastRowColorFirstColor = false;
        private Color _rowColor1 = new Color(0.9f, 0.9f, 0.9f);
        private Color _rowColor2 = new Color(1.0f, 1.0f, 1.0f);
        private Map _map = new Map();
        private Logger _logger = new Logger();
        private Surrounding _surrounding;
        private GameObject _stateMenu;
        private GameObject _rulesetMenu;
        
        private int _dataTableRowLength = 5;

        // Start is called before the first frame update
        void Start()
        {
            _stateMenu = GameObject.Find("StateMenu");   
            _rulesetMenu = GameObject.Find("RulesetMenu");
            GameObject surroundingObject = GameObject.Find("Surrounding");
            Surrounding surrounding = surroundingObject.GetComponent(typeof(Surrounding)) as Surrounding;
            _surrounding = surrounding;
            InitStates();
            InitDirections();
            InitMoves();
            InitScenarios();
        }

        private void InitScenarios() {
            _map.InitMap();
            _players.AddPlayer(new Player("white"));
            _players.AddPlayer(new Player("black"));
            _scenario = new Scenario(_map);
            _scenario.InitScenario(_players, _enemyMoves, "scenario1");

            GameObject dropdownObject = GameObject.Find("ScenarioSelectionDropdown");
            Dropdown dropdown = dropdownObject.GetComponent(typeof(Dropdown)) as Dropdown;
            dropdown.ClearOptions();

            foreach (string scenarioName in _scenario.GetScenarioList()) {
                Dropdown.OptionData option = new Dropdown.OptionData();
                option.text = scenarioName;
                dropdown.options.Add(option);
            }
            dropdown.value = 0;
            dropdown.RefreshShownValue();

            _actualState = new State("Start");
        }

        private void InitStates() {
            GameObject test = GameObject.Find("States");
            _states = test.GetComponent(typeof(States)) as States;
        }

        private void InitDirections() {
            GameObject dropdownObject = GameObject.Find("DirectionDropdown");
            Dropdown dropdown = dropdownObject.GetComponent(typeof(Dropdown)) as Dropdown;
            dropdown.ClearOptions();

            _directions.AddDirection(new Direction("Oben", 1, 0));
            _directions.AddDirection(new Direction("Unten", -1, 0));
            _directions.AddDirection(new Direction("Links", 0, -1));
            _directions.AddDirection(new Direction("Rechts", 0, 1));

            foreach (Direction item in _directions)
            {
                Dropdown.OptionData option = new Dropdown.OptionData();
                option.text = item.GetDirectionName();
                dropdown.options.Add(option);
            }
            
            dropdown.value = 0;
            dropdown.RefreshShownValue();
        }

        private void InitMoves() {
            GameObject dropdownObject = GameObject.Find("ModeDropdown");
            Dropdown dropdown = dropdownObject.GetComponent(typeof(Dropdown)) as Dropdown;
            dropdown.ClearOptions();

            // TODO remove magic values and make enum
            _modes.AddMode(new Mode("Figur schlagen", 1));
            _modes.AddMode(new Mode("Leeres Feld betreten", 0));
            _modes.AddMode(new Mode("Leeres Feld + Zug beenden", 2));


            foreach (Mode item in _modes)
            {
                Dropdown.OptionData option = new Dropdown.OptionData();
                option.text = item.GetModeName();
                dropdown.options.Add(option);
            }

            dropdown.value = 1;
        }

        public void AddRulesetButtonClicked() {
            GameObject directionDropdownObject = GameObject.Find("DirectionDropdown");
            Dropdown directionDropdown = directionDropdownObject.GetComponent(typeof(Dropdown)) as Dropdown;
            int directionValue = directionDropdown.value;

            GameObject modeDropdownObject = GameObject.Find("ModeDropdown");
            Dropdown modeDropdown = modeDropdownObject.GetComponent(typeof(Dropdown)) as Dropdown;
            int modeValue = modeDropdown.value;

            GameObject startStateDropdownObject = GameObject.Find("StartStateDropdown");
            Dropdown startStateDropdown = startStateDropdownObject.GetComponent(typeof(Dropdown)) as Dropdown;
            int startStateValue = startStateDropdown.value;

            GameObject endStateDropdownObject = GameObject.Find("EndStateDropdown");
            Dropdown endStateDropdown = endStateDropdownObject.GetComponent(typeof(Dropdown)) as Dropdown;
            int endStateValue = endStateDropdown.value;

            string testOutput = directionDropdown.options[directionValue].text + modeDropdown.options[modeValue].text + startStateDropdown.options[startStateValue].text + endStateDropdown.options[endStateValue].text;

            State start = _states.FindState(startStateDropdown.options[startStateValue].text);
            State end = _states.FindState(endStateDropdown.options[endStateValue].text);
            Direction direction = _directions.FindDirection(directionDropdown.options[directionValue].text);
            Mode mode = _modes.FindMode(modeDropdown.options[modeValue].text);
            Ruleset ruleset = new Ruleset(start, end, direction, mode, null, _surrounding.CloneSurrounding());

            bool isAdded = _rulesets.AddRuleset(ruleset);
            
            if (isAdded) {
                ResetDeleteRulesetDropdown();
                CreateNewRulesetGameObject(ruleset);
            }
        }

        public void ResetDeleteRulesetDropdown() {
            GameObject deleteSingleRulesetDropdownObject = GameObject.Find("DeleteSingleRulesetDropdown");
            Dropdown deleteSingleRulesetDropdown = deleteSingleRulesetDropdownObject.GetComponent(typeof(Dropdown)) as Dropdown;

            if (deleteSingleRulesetDropdown) {
                deleteSingleRulesetDropdown.ClearOptions();
            } else {
                Debug.Log("[ERROR]: There is no deleteSingleRulesetDropdown element!");
                return;
            }

            for (int rulesetCounter = 0; rulesetCounter < _rulesets.GetCount(); rulesetCounter++) {
                Dropdown.OptionData option = new Dropdown.OptionData();
                option.text = System.Convert.ToString(rulesetCounter + 1);
                deleteSingleRulesetDropdown.options.Add(option);
            }

            deleteSingleRulesetDropdown.RefreshShownValue();
        }

        void CreateNewRulesetGameObject(Ruleset ruleset) {

            GameObject rulesetTextTableObject = GameObject.Find("RulesetTextTable");
            
            if (rulesetTextTableObject == null) {
                Debug.Log("[ERROR]: RulesetTextTable cound not be found!");
                return;
            }
            List<string> rulesetTextArray = ruleset.ToStringArray();

            // For every column clone default element and fill it with new data
            for (int counter = 0; counter < rulesetTextArray.Count; counter++) {
               
                (GameObject backgroundObject, GameObject rulesetTextObject) = CreateBackgroundObject(rulesetTextTableObject);

                if (rulesetTextObject == null) {
                    return;
                }

                TextMeshProUGUI textmeshObject = rulesetTextObject.GetComponent(typeof(TextMeshProUGUI)) as TextMeshProUGUI;

                if (textmeshObject == null) {
                    Debug.Log("[ERROR]: TextMeshProUGUI cloning did not work properly!");
                    return;
                }

                textmeshObject.text = rulesetTextArray[counter];
            }

            // add surrounding info
            CloneSurroundingUIAndDisableButtons(rulesetTextTableObject);

            _ruleCounter += 1;
            _isLastRowColorFirstColor = !_isLastRowColorFirstColor;
        }

        private (GameObject backgroundObject, GameObject textObject) CreateBackgroundObject(GameObject rulesetTextTableObject) {

            GameObject rulesetTextBackgroundObject;
            GameObject rulesetTextObject;

            GameObject defaultRulesetTextBackgroundObject = rulesetTextTableObject.transform.GetChild(0).gameObject;

            if (defaultRulesetTextBackgroundObject == null) {
                Debug.Log("[ERROR]: There is no default rulesetBackgroundText element!");
                return (null, null);
            }

            if (defaultRulesetTextBackgroundObject.transform.childCount > 0) {
                rulesetTextBackgroundObject = Instantiate(defaultRulesetTextBackgroundObject);
            } else {
                Debug.Log("[ERROR]: RulesetTextColumn child could not be found!");
                return (null, null);
            }

            if (rulesetTextBackgroundObject != null && rulesetTextBackgroundObject.transform.childCount > 0) {
                rulesetTextObject = rulesetTextBackgroundObject.transform.GetChild(0).gameObject;
            } else {
                Debug.Log("[ERROR]: RulesetTextBackgroundObject or its child could not be found!");
                return (null, null);
            }

            string defaultName = rulesetTextObject.name;
            rulesetTextObject.name = defaultName + "_" + _ruleCounter;

            defaultName = rulesetTextBackgroundObject.name;
            string stringToRemove = "(Clone)";
            int index = defaultName.IndexOf(stringToRemove);
            if (index != -1) {
                defaultName = defaultName.Remove(index, stringToRemove.Length);
            }
            rulesetTextBackgroundObject.name = defaultName + "_" + _ruleCounter;

            rulesetTextBackgroundObject.transform.SetParent(rulesetTextTableObject.transform);
            rulesetTextObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            UnityEngine.UI.Image background = rulesetTextBackgroundObject.GetComponent(typeof(UnityEngine.UI.Image)) as UnityEngine.UI.Image;
            
            if (_isLastRowColorFirstColor) {
                background.color = _rowColor2;
            } else {
                background.color = _rowColor1;
            }

            rulesetTextBackgroundObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            return (rulesetTextBackgroundObject, rulesetTextObject);
        }


        private void CloneSurroundingUIAndDisableButtons(GameObject objectToClone) {
            GameObject output = Instantiate(GameObject.Find("SurroundingButtonGrid"));
            output.transform.SetParent(objectToClone.transform);
            output.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);    

            GameObject rulesetTextTableObject = GameObject.Find("RulesetTextTable");
            (GameObject backgroundObject, GameObject textObject) = CreateBackgroundObject(rulesetTextTableObject);

            VerticalLayoutGroup group = output.GetComponent(typeof(VerticalLayoutGroup)) as VerticalLayoutGroup;
            group.childAlignment = TextAnchor.UpperLeft;
            RectOffset offset = new RectOffset(20,0,0,0);
            group.padding = offset;
            group.childForceExpandWidth = false;

            for (int counter = 0; counter < output.transform.childCount; counter++) {
                GameObject child = output.transform.GetChild(counter).gameObject;
                for (int counter2 = 0; counter2 < child.transform.childCount; counter2++) {
                    GameObject buttonGameObject = child.transform.GetChild(counter2).gameObject;
                    Button buttonObject = buttonGameObject.GetComponent(typeof(Button)) as Button;
                    buttonObject.transition = Selectable.Transition.None;
                    buttonObject.interactable = !buttonObject.interactable;
                }
            }

            output.transform.SetParent(backgroundObject.transform);
            textObject.SetActive(false);;
        }

        public void RunStateMachine() {
            SetVisibilityOfMenus(false);
            ClearStateMenu();
            StartCoroutine(MakeMove());
        }

        private void ClearStateMenu() {
            GameObject overviewObject = GameObject.Find("StateMachineOverview");

            for (int counter = 0; counter < overviewObject.transform.childCount; counter++) {
                if (counter > 1) {
                    GameObject.Destroy(overviewObject.transform.GetChild(counter).gameObject);
                }  
            }
            _logger.ResetLogCounter();
        }

        private void SetVisibilityOfMenus(bool isVisible) {
            if (_rulesetMenu) {
                _rulesetMenu.SetActive(isVisible);
            } else {
                Debug.Log("[ERROR]: RulesetMenu GameObject could not be found!");
            }
            if (_stateMenu) {
                _stateMenu.SetActive(isVisible);
            } else {
                Debug.Log("[ERROR]: StateMenu GameObject could not be found!");
            }
        }
        public void ChangeToEditMode() {
            SetVisibilityOfMenus(true);
            ResetScenario();
        }

        public void DeleteRuleset() {
            GameObject deleteSingleRulesetDropdownObject = GameObject.Find("DeleteSingleRulesetDropdown");
            Dropdown deleteSingleRulesetDropdown = deleteSingleRulesetDropdownObject.GetComponent(typeof(Dropdown)) as Dropdown;

            int deleteSingleRulesetValue = deleteSingleRulesetDropdown.value;

            if (deleteSingleRulesetDropdown.options.Count == 0) {
                Debug.Log("[ERROR]: There is no ruleset to delete!");
                return;
            }

            GameObject rulesetTextTableObject = GameObject.Find("RulesetTextTable");

            if (rulesetTextTableObject == null) {
                Debug.Log("[ERROR]: RulesetTextTable cound not be found!");
                return;
            }
            
            int index = 0;
            
            foreach (Transform ruleTextElement in rulesetTextTableObject.transform)
            {
                if (index >= deleteSingleRulesetValue * _dataTableRowLength + _dataTableRowLength && index < deleteSingleRulesetValue * _dataTableRowLength + 2 * _dataTableRowLength) {
                    GameObject.Destroy(ruleTextElement.gameObject);
                }
                index++;
            }

            _rulesets.RemoveRuleset(deleteSingleRulesetValue);

            //TODO change color of all textElements depending on the color of the ruleset removed

            ResetDeleteRulesetDropdown();
        }

        public void ResetScenario() {

            GameObject scenarioDropdownObject = GameObject.Find("ScenarioSelectionDropdown");
            Dropdown scenarioDropdown = scenarioDropdownObject.GetComponent(typeof(Dropdown)) as Dropdown;
            int scenarioValue = scenarioDropdown.value;
            string scenarioName = scenarioDropdown.options[scenarioValue].text;

            foreach(Player player in _players) {
                player.RemoveFigures();
            }
            _scenario.InitScenario(_players, _enemyMoves, scenarioName);
            _actualState = new State("Start");
        }

        public void ResetRules() {
            _rulesets = new Rulesets();

            ResetDeleteRulesetDropdown();

            GameObject rulesetTextTableObject = GameObject.Find("RulesetTextTable");

            if (rulesetTextTableObject == null) {
                Debug.Log("[ERROR]: RulesetTextTable cound not be found!");
                return;
            }
            
            int index = 0;
            foreach (Transform ruleTextElement in rulesetTextTableObject.transform)
            {
                if (index > _dataTableRowLength - 1) {
                    GameObject.Destroy(ruleTextElement.gameObject);
                }
                index++;
            }
        }

        private (Ruleset ruleToExecute, int rulesetId, Field fieldAfterMove) FindRuleToExecute(Figure figureToMove, Rulesets rulesets, bool isPlayer) {
            for (int rulesetCounter = 0; rulesetCounter < rulesets.GetCount(); rulesetCounter++)
                    {   
                        Ruleset ruleset = rulesets.GetRulesetAtPosition(rulesetCounter);
                        
                        // check if rule has right start state
                        if (ruleset.GetStartState().GetStateName() == _actualState.GetStateName()) {
                            Direction direction = ruleset.GetDirection();
                            Field fieldToCheck = null;
                            fieldToCheck = _map.GetFieldByIndices(figureToMove._positionColumn + direction.GetColumnMovementFactor(), figureToMove._positionRow + direction.GetRowMovementFactor());

                            // check if move would end outside of the board
                            if (fieldToCheck == null) {
                                continue;
                            }

                            // check if field is empty (when mode is 0)
                            // TODO remove magic value
                            if ((ruleset.GetMode().GetModeCode() == 0 || ruleset.GetMode().GetModeCode() == 2) && fieldToCheck.GetFigure() != null) {
                                continue;
                            }                        

                            // if no figure is on field to hit
                            if (ruleset.GetMode().GetModeCode() == 1 && fieldToCheck.GetFigure() == null) {
                                continue;
                            }

                            // if own figure would be hit
                            if (ruleset.GetMode().GetModeCode() == 1 && fieldToCheck.GetFigure() && figureToMove._player._playerName == fieldToCheck.GetFigure()._player._playerName) {
                                continue;
                            }

                            // check surrounding
                            if (!_surrounding.CompareSurrounding(ruleset.GetSurrounding()) && isPlayer) {
                                continue;
                            }
                            
                            return (ruleset, rulesetCounter, fieldToCheck);
                        }
                    }
            return (null, 0, null);
        }

        
        public void CheckEndConditions() {

            Player player = _players.GetUserPlayer();
            Figure figureToMove = player.GetFigures().GetFigureAtPosition(0);

            _logger.LogStateMachineMessage("Keine Regel gefunden", new Color32(0, 0, 0, 255), player._isUser);

            Field endField = _map.GetFieldByIndices(figureToMove._positionColumn, figureToMove._positionRow);
            
            bool isSuccess = true;
            bool allFiguresHit = true;

            if (_scenario.MustHitAllEnemies()) {
                foreach(Player playerToCheck in _players) {
                    if (playerToCheck.GetFigures().Count() != 0 && playerToCheck != _players.GetUserPlayer()) {
                        _logger.LogStateMachineMessage("Nicht alle Figuren wurden geschlagen", new Color32(154, 0, 11, 255), player._isUser);
                        isSuccess = false;
                        allFiguresHit = false;
                    }
                }
                if (allFiguresHit) {
                    _logger.LogStateMachineMessage("Alle Figuren wurden geschlagen", new Color32(65, 154, 40, 255), player._isUser);
                }
            }

            

            if (!endField ||! endField.IsDestination() || !_actualState.IsEndState()) {
                isSuccess = false;
            } 

            if (isSuccess) {
                _logger.LogStateMachineMessage("Endziel mit richtigen State erreicht", new Color32(65, 154, 40, 255), player._isUser);
            } else {
                _logger.LogStateMachineMessage("Endziel oder End State falsch", new Color32(154, 0, 11, 255), player._isUser);
            }

        }

        IEnumerator MakeMove() {

            Player player = _players.GetUserPlayer();
           
            _actualDirection = null;
            bool moveEnds = false;

            Figure figureToMove = player.GetFigures().GetFigureAtPosition(0);
            Field actualField = _map.GetFieldByIndices(figureToMove._positionColumn, figureToMove._positionRow);
            _surrounding.UpdateSurrounding(actualField, _map);

            while (true) {
                while (!figureToMove.gameObject.activeSelf) {
                    player.GetFigures().RemoveFigure(0);

                    // no figure to move available
                    if (player.GetFigures().Count() == 0) {
                        _logger.LogStateMachineMessage("Keine bewegbare Figur mehr vorhanden", new Color32(0, 0, 0, 255), player._isUser);
                        
                        CheckEndConditions();
                        yield break;
                    }

                    figureToMove = player.GetFigures().GetFigureAtPosition(0);
                }

                string name = figureToMove.gameObject.name;
                Rulesets rulesets =  _enemyMoves.GetNextMove(name,_actualState, _modes);

                Ruleset ruleToExecute = null;
                int rulesetId = 0;
                Field fieldAfterMove = null;

                if (player == _players.GetUserPlayer()) {
                    (ruleToExecute, rulesetId, fieldAfterMove) = FindRuleToExecute(figureToMove, _rulesets, true);
                } else {
                    (ruleToExecute, rulesetId, fieldAfterMove) = FindRuleToExecute(figureToMove, rulesets, false);
                }

                // Check if field is empty (no hitting move) 
                if (ruleToExecute == null) {
                    CheckEndConditions();
                    yield break;
                } else {
                    _logger.LogStateMachineMessage("Regel " + (rulesetId + 1) + " wird ausgeführt", new Color32(0, 0, 0, 255), player._isUser);
                }
                
                // Set moving factors according to move
                int columnMovementFactor = ruleToExecute.GetDirection().GetColumnMovementFactor();
                int rowMovementFactor = ruleToExecute.GetDirection().GetRowMovementFactor();

                if (_actualDirection != null && _actualDirection.GetDirectionName() != ruleToExecute.GetDirection().GetDirectionName()) {
                    Player nextPlayer =  _players.GetNextPlayer();
                    if (nextPlayer != null) {
                        player = nextPlayer;
                    }
                    moveEnds = false;
                    _actualDirection = null;
                    Debug.Log("new direction is chosen");


                    figureToMove = player.GetFigures().GetFigureAtPosition(0);
                    actualField = _map.GetFieldByIndices(figureToMove._positionColumn, figureToMove._positionRow);
                    _surrounding.UpdateSurrounding(actualField, _map);

                    continue;
                }


                // Set new state
                _actualState = ruleToExecute.GetEndState();

                _actualDirection = ruleToExecute.GetDirection();

                Vector3 position = figureToMove.transform.position;

                figureToMove.transform.position = new Vector3(position.x + (float)0.055 * columnMovementFactor, position.y, position.z + (float)0.055 * rowMovementFactor);

                // figure is hit
                if (fieldAfterMove.GetFigure() != null) {
                    fieldAfterMove.RemoveFigure();
                    moveEnds = true;
                } 

                //TODO now every figure of ai just moves one step
                if (moveEnds || player == _players.GetPlayerAtIndex(1) || ruleToExecute.GetMode().GetModeCode() == 2) {
                    Player nextPlayer =  _players.GetNextPlayer();
                    if (nextPlayer != null) {
                        player = nextPlayer;
                    }
                    if (ruleToExecute.GetMode().GetModeCode() == 2) {
                        Debug.Log("move end is chosen");
                    }
                    moveEnds = false;
                    _actualDirection = null;
                }

                // set figure to new field
                actualField = _map.GetFieldByIndices(figureToMove._positionColumn, figureToMove._positionRow);
                actualField.MoveFigureAway();
                fieldAfterMove.SetFigure(figureToMove);
                
                // set new position in figure
                figureToMove._positionColumn += ruleToExecute.GetDirection().GetColumnMovementFactor(); 
                figureToMove._positionRow += ruleToExecute.GetDirection().GetRowMovementFactor();
                
                figureToMove = player.GetFigures().GetFigureAtPosition(0);
                actualField = _map.GetFieldByIndices(figureToMove._positionColumn, figureToMove._positionRow);
                _surrounding.UpdateSurrounding(actualField, _map);

                yield return new WaitForSeconds(1);
            }
           
        }
    }
}