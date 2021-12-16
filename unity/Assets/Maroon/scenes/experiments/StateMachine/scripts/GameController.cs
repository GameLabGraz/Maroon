using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maroon.UI;
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
        private Directions _directions = new Directions();
        private Moves _moves = new Moves();
        private Modes _modes = new Modes();
        // Rule counter increases every time a rule is added, but does NOT decrease (so names are always unique)
        private int _ruleCounter = 0;
        private bool _isLastRowColorFirstColor = false;
        private Color _rowColor1 = new Color(0.9f, 0.9f, 0.9f);
        private Color _rowColor2 = new Color(1.0f, 1.0f, 1.0f);
        private Map _map = new Map();
        private Logger _logger = new Logger();

        // Start is called before the first frame update
        void Start()
        {
            InitGameField();
            InitStates();
            InitDirections();
            InitMoves();
            InitScenarios();
        }

        void InitScenarios() {
            _map.InitMap();
            _players.AddPlayer(new Player("white"));
            _players.AddPlayer(new Player("black"));
            _scenario = new Scenario(_map);
            _scenario.InitScenario(_players, "scenario1");

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


        void InitGameField() {
            GameObject test = GameObject.Find("a2White");
            Field test2 = test.GetComponent(typeof(Field)) as Field;
        }

        void InitStates() {
            GameObject test = GameObject.Find("States");
            _states = test.GetComponent(typeof(States)) as States;
        }

        void InitDirections() {
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

        void InitMoves() {
            GameObject dropdownObject = GameObject.Find("ModeDropdown");
            Dropdown dropdown = dropdownObject.GetComponent(typeof(Dropdown)) as Dropdown;
            dropdown.ClearOptions();

            // TODO remove magic values and make enum
            _modes.AddMode(new Mode("Figur schlagen", 1));
            _modes.AddMode(new Mode("Leeres Feld betreten", 0));

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
            Ruleset ruleset = new Ruleset(start, end, direction, mode, null);
            _rulesets.AddRuleset(ruleset);

            CreateNewRulesetGameObject(ruleset);
        }

        public void CreateNewRulesetGameObject(Ruleset ruleset) {

            GameObject rulesetTextTableObject = GameObject.Find("RulesetTextTable");
            
            if (rulesetTextTableObject == null) {
                Debug.Log("[ERROR]: RulesetTextTable cound not be found!");
                return;
            }
            List<string> rulesetTextArray = ruleset.ToStringArray();

            

            // For every column clone default element and fill it with new data
            for (int counter = 0; counter < rulesetTextArray.Count; counter++) {
               
                GameObject rulesetTextBackgroundObject;
                GameObject rulesetTextObject;

                GameObject defaultRulesetTextBackgroundObject = rulesetTextTableObject.transform.GetChild(counter).gameObject;

                if (defaultRulesetTextBackgroundObject == null) {
                    Debug.Log("[ERROR]: There is no default rulesetBackgroundText element!");
                    return;
                }

                if (defaultRulesetTextBackgroundObject.transform.childCount > 0) {
                    rulesetTextBackgroundObject = Instantiate(defaultRulesetTextBackgroundObject);
                } else {
                    Debug.Log("[ERROR]: RulesetTextColumn child could not be found!");
                    return;
                }

                if (rulesetTextBackgroundObject != null && rulesetTextBackgroundObject.transform.childCount > 0) {
                    rulesetTextObject = rulesetTextBackgroundObject.transform.GetChild(0).gameObject;
                } else {
                    Debug.Log("[ERROR]: RulesetTextBackgroundObject or its child could not be found!");
                    return;
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

                TextMeshProUGUI textmeshObject = rulesetTextObject.GetComponent(typeof(TextMeshProUGUI)) as TextMeshProUGUI;

                if (textmeshObject == null) {
                    Debug.Log("[ERROR]: TextMeshProUGUI cloning did not work properly!");
                    return;
                }

                textmeshObject.text = rulesetTextArray[counter];
            }
            _ruleCounter += 1;
            _isLastRowColorFirstColor = !_isLastRowColorFirstColor;
        }

        public void RunStateMachine() {
            StartCoroutine(MakeMove());
        }

        public void ResetScenario() {

            GameObject scenarioDropdownObject = GameObject.Find("ScenarioSelectionDropdown");
            Dropdown scenarioDropdown = scenarioDropdownObject.GetComponent(typeof(Dropdown)) as Dropdown;
            int scenarioValue = scenarioDropdown.value;
            string scenarioName = scenarioDropdown.options[scenarioValue].text;

            foreach(Player player in _players) {
                player.RemoveFigures();
            }
            _scenario.InitScenario(_players, scenarioName);
            _actualState = new State("Start");
        }

        IEnumerator MakeMove() {

            Player player = _players.GetPlayerAtIndex(0);
            Figure figureToMove = player.GetFigures().GetFigureAtPosition(0);

            while (true) {

                // Find ruleset which should be executed
                Ruleset ruleToExecute = null;
                int rulesetId = 0;
                Field fieldAfterMove = null;


                // TODO make own method
                for (int rulesetCounter = 0; rulesetCounter < _rulesets.GetCount(); rulesetCounter++)
                {   
                    Ruleset ruleset = _rulesets.GetRulesetAtPosition(rulesetCounter);
                    

                    // check if rule has right start state
                    if (ruleset.GetStartState().GetStateName() == _actualState.GetStateName()) {
                        Direction direction = ruleset.GetDirection();
                        Field fieldToCheck = null;
                        fieldToCheck = _map.GetFieldByIndices(figureToMove._positionColumn + direction.GetColumnMovementFactor(), figureToMove._positionRow + direction.GetRowMovementFactor() );

                        // check if move would end outside of the board
                        if (fieldToCheck == null) {
                            continue;
                        }

                        // check if field is empty (when mode is 0)
                        // TODO remove magic value
                        if (ruleset.GetMode().GetModeCode() == 0 && fieldToCheck.GetFigure() != null) {
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
                        
                        ruleToExecute = ruleset;
                        rulesetId = rulesetCounter;
                        fieldAfterMove = fieldToCheck;
                    }
                }

                // TODO compare mode 
                
                // TODO compare surrounding of rule to surrounding of the figure to move => only one rule must be available after that comparison

                // Check if field is empty (no hitting move) 
                if (ruleToExecute == null) {
                    _logger.LogStateMachineMessage("Keine Regel gefunden");
                    yield break;
                } else {
                    _logger.LogStateMachineMessage("Regel " + (rulesetId + 1) + " wird ausgeführt");
                }
                // Set new state
                _actualState = ruleToExecute.GetEndState();

                // Set moving factors according to move
                int columnMovementFactor = ruleToExecute.GetDirection().GetColumnMovementFactor();
                int rowMovementFactor = ruleToExecute.GetDirection().GetRowMovementFactor();

                Vector3 position = figureToMove.transform.position;

                figureToMove.transform.position = new Vector3(position.x + (float)0.18 * columnMovementFactor, position.y + (float)0.18 * rowMovementFactor, position.z);

                // figure is hit
                if (fieldAfterMove.GetFigure() != null) {
                    fieldAfterMove.RemoveFigure();
                }

                // set figure to new field
                fieldAfterMove.SetFigure(figureToMove);
                // set new position in figure
                figureToMove._positionColumn += ruleToExecute.GetDirection().GetColumnMovementFactor(); 
                figureToMove._positionRow += ruleToExecute.GetDirection().GetRowMovementFactor();

                yield return new WaitForSeconds(1);

                
                // bounce check move
                // is field free check
                // can figure hit other one check

                // set figure to new field
                // remove figure from old field
                // depending on mode remove hit figure and remove it from field
            }
           
        }

        

    }
}