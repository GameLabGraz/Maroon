using System.Collections;
using System.Collections.Generic;
using GEAR.Localization;
using UnityEngine;
using Maroon.UI;
using Button = UnityEngine.UI.Button;
using VerticalLayoutGroup = UnityEngine.UI.VerticalLayoutGroup;
using Selectable = UnityEngine.UI.Selectable;
using LocalizedTMP = GEAR.Localization.Text.LocalizedTMP;
using TMPro;

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
        
        private Color _errorColor = new Color32(154, 0, 11, 255);
        private Color _winColor = new Color32(65, 154, 40, 255);
        private Map _map = new Map();
        private Logger _logger = new Logger();
        private Surrounding _surrounding;
        private GameObject _stateMenu;
        private GameObject _rulesetMenu;
        
        private int _dataTableRowLength = 5;

        private DialogueManager _dialogueManager;

        private bool _runStateMachine = false;

        // Start is called before the first frame update
        void Start()
        {
            _dialogueManager = FindObjectOfType<DialogueManager>();
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

            // ToDo: Use a Localized DropDown!!!
            _directions.AddDirection(new Direction("Up", 1, 0));
            _directions.AddDirection(new Direction("Down", -1, 0));
            _directions.AddDirection(new Direction("Left", 0, -1));
            _directions.AddDirection(new Direction("Right", 0, 1));

            LanguageManager.Instance.OnLanguageChanged.AddListener(language =>
            {
                dropdown.ClearOptions();
                foreach (Direction item in _directions)
                {
                    string directionName = LanguageManager.Instance.GetString(item.GetDirectionName(), language);
                    dropdown.options.Add(new TMP_Dropdown.OptionData(directionName));
                    
                    if (directionName != null) {
                        item.SetDirectionName(directionName);
                    }
                }
                dropdown.RefreshShownValue();
            });

            foreach (Direction item in _directions)
            {
                string directionName = LanguageManager.Instance.GetString(item.GetDirectionName());
                dropdown.options.Add(new TMP_Dropdown.OptionData(directionName));
                if (directionName != null) {
                    item.SetDirectionName(directionName);
                }
            }
            
            dropdown.value = 0;
            dropdown.RefreshShownValue();
        }

        private void InitMoves() {
            GameObject dropdownObject = GameObject.Find("ModeDropdown");
            Dropdown dropdown = dropdownObject.GetComponent(typeof(Dropdown)) as Dropdown;
            dropdown.ClearOptions();

            // TODO remove magic values and make enum
            // ToDO use localized DropDown
            _modes.AddMode(new Mode("CaptureFigure", 1));
            _modes.AddMode(new Mode("EnterEmptyField", 0));
            _modes.AddMode(new Mode("EmptyFieldEndMove", 2));

            LanguageManager.Instance.OnLanguageChanged.AddListener(language =>
            {
                dropdown.ClearOptions();
                foreach (Mode item in _modes)
                {
                    string modeName = LanguageManager.Instance.GetString(item.GetModeName(), language);
                    dropdown.options.Add(new TMP_Dropdown.OptionData(modeName));

                    if (modeName != null) {
                        item.SetModeName(modeName);
                    }
                }
                dropdown.RefreshShownValue();
            });

            foreach (Mode item in _modes)
            {
                string modeName = LanguageManager.Instance.GetString(item.GetModeName());
                dropdown.options.Add(new TMP_Dropdown.OptionData(modeName));
                if (modeName != null) {
                    item.SetModeName(modeName);
                }
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

            (bool isAdded, string message) = _rulesets.AddRuleset(ruleset);
            
            if (isAdded) {
                ResetDeleteRulesetDropdown();
                CreateNewRulesetGameObject(ruleset);
            } else {
                _dialogueManager.ShowMessage(new Message(LanguageManager.Instance.GetString("ErrorPawnToQueen"), _errorColor, MessageIcon.MI_Error));
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

                LocalizedTMP localizedTMPObject = rulesetTextObject.GetComponent<LocalizedTMP>();
                localizedTMPObject.enabled = false;

                if (textmeshObject == null) {
                    Debug.Log("[ERROR]: TextMeshProUGUI cloning did not work properly!");
                    return;
                }
                
                for (var counter2 = 0; counter2 < rulesetTextObject.transform.childCount; counter2++) {
                    GameObject objtest = rulesetTextObject.transform.GetChild(counter2).gameObject;
                    textmeshObject.text = rulesetTextArray[counter];
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
            _runStateMachine = true;
            SetVisibilityOfMenus(false);
            ClearStateMenu();
            GameObject scenarioDropdownObject = GameObject.Find("ScenarioSelectionDropdown");
            Dropdown scenarioDropdown = scenarioDropdownObject.GetComponent(typeof(Dropdown)) as Dropdown;
            scenarioDropdown.enabled = false;
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

        IEnumerator PlayDisable() {
            _runStateMachine = false;
            StopCoroutine(MakeMove());
            GameObject playButtonObject = GameObject.Find("ButtonPlay"); 
            Button playButton = playButtonObject.GetComponent<Button>();
            playButton.interactable = false;
            yield return new WaitForSeconds(0.5f);
            playButton.interactable = true;
            SetVisibilityOfMenus(true);
            ResetScenario();
            GameObject scenarioDropdownObject = GameObject.Find("ScenarioSelectionDropdown");
            Dropdown scenarioDropdown = scenarioDropdownObject.GetComponent(typeof(Dropdown)) as Dropdown;
            scenarioDropdown.enabled = true;
        }
        public void ChangeToEditMode() {
            StartCoroutine(PlayDisable());
        }

        public void DeleteRuleset() {
            GameObject deleteSingleRulesetDropdownObject = GameObject.Find("DeleteSingleRulesetDropdown");
            Dropdown deleteSingleRulesetDropdown = deleteSingleRulesetDropdownObject.GetComponent(typeof(Dropdown)) as Dropdown;

            int deleteSingleRulesetValue = deleteSingleRulesetDropdown.value;

            if (deleteSingleRulesetDropdown.options.Count == 0) {
                _dialogueManager.ShowMessage(new Message(LanguageManager.Instance.GetString("ErrorNoRuleToDelete"), _errorColor, MessageIcon.MI_Error));
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


            //TODO change color of all textElements depending on the color of the ruleset removed
            var colorCounter = 0;
            bool isFirstColor = true;
            rulesetTextTableObject = GameObject.Find("RulesetTextTable");
            for (var counter = 5; counter < rulesetTextTableObject.transform.childCount; counter++) {
                if (counter >= deleteSingleRulesetValue * _dataTableRowLength + _dataTableRowLength && counter < deleteSingleRulesetValue * _dataTableRowLength + 2 * _dataTableRowLength) {
                   continue;
                }
                GameObject rulesetTextBackgroundObject = rulesetTextTableObject.transform.GetChild(counter).gameObject;               
                UnityEngine.UI.Image background = rulesetTextBackgroundObject.GetComponent(typeof(UnityEngine.UI.Image)) as UnityEngine.UI.Image;
                
                if (colorCounter % _dataTableRowLength == 0 && colorCounter != 0) {
                    isFirstColor = !isFirstColor;
                }
                Debug.LogFormat("{0} {1} {2}", counter, isFirstColor, colorCounter % _dataTableRowLength);

                if (isFirstColor) {
                    background.color = _rowColor1;
                } else {
                    background.color = _rowColor2;
                }

                colorCounter++;
            }

            if (isFirstColor) {
                _isLastRowColorFirstColor = true;
            } else {
                _isLastRowColorFirstColor = false;
            }

            
            _rulesets.RemoveRuleset(deleteSingleRulesetValue);
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
            _enemyMoves = new EnemyMoves();
            _players.ResetPlayerCounter();
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
            Figure figureToMove = player.GetFigures().GetNextActiveFigure();
        
            if (figureToMove == null) {
                _dialogueManager.ShowMessage(new Message(LanguageManager.Instance.GetString("ErrorNoFigureToMove"), _errorColor, MessageIcon.MI_Error));
                return;
            }

            Field endField = _map.GetFieldByIndices(figureToMove._positionColumn, figureToMove._positionRow);
            
            bool isSuccess = true;
            bool allFiguresHit = true;

            if (_scenario.MustHitAllEnemies()) {
                foreach(Player playerToCheck in _players) {
                    if (playerToCheck.GetFigures().Count() != 0 && playerToCheck != _players.GetUserPlayer()) {
                        _logger.LogStateMachineMessage(LanguageManager.Instance.GetString("ErrorNotAllFiguresCaptured"), _errorColor, player._isUser);
                        isSuccess = false;
                        allFiguresHit = false;
                    }
                }
                if (allFiguresHit) {
                    _logger.LogStateMachineMessage(LanguageManager.Instance.GetString("AllFiguresCaptured"), _winColor, player._isUser);
                }
            }

            // TODO implement end field in a reasonable way. if (!endField ||! endField.IsDestination() || !_actualState.IsEndState())
            if (!_actualState.IsEndState()) {
                isSuccess = false;
            } 

            if (isSuccess) {
                _dialogueManager.ShowMessage(new Message(LanguageManager.Instance.GetString("CorrectTargetAndEndState"), _errorColor, MessageIcon.MI_Ok));
                _logger.LogStateMachineMessage(LanguageManager.Instance.GetString("CorrectTargetAndEndState"), _winColor, player._isUser);
            } else {
                _dialogueManager.ShowMessage(new Message(LanguageManager.Instance.GetString("ErrorIncorrectTargetOrEndState"), _errorColor, MessageIcon.MI_Error));
                _logger.LogStateMachineMessage(LanguageManager.Instance.GetString("ErrorIncorrectTargetOrEndState"), _errorColor, player._isUser);
            }

        }

        IEnumerator MakeMove() {

            Player player = _players.GetUserPlayer();
           
            _actualDirection = null;
            bool moveEnds = false;

            Figure figureToMove = player.GetFigures().GetNextActiveFigure();
            Field actualField = _map.GetFieldByIndices(figureToMove._positionColumn, figureToMove._positionRow);
            _surrounding.UpdateSurrounding(actualField, _map);

            while (_runStateMachine) {

                if (player == _players.GetUserPlayer()) {
                    if (figureToMove == null) {
                        _logger.LogStateMachineMessage(LanguageManager.Instance.GetString("ErrorNoFigureToMove"), _errorColor, player._isUser);
                        CheckEndConditions();
                        yield break;
                    }
                    actualField = _map.GetFieldByIndices(figureToMove._positionColumn, figureToMove._positionRow);
                    _surrounding.UpdateSurrounding(actualField, _map);
                }

                figureToMove = player.GetFigures().GetNextActiveFigure();

                if (figureToMove == null) {
                    _logger.LogStateMachineMessage(LanguageManager.Instance.GetString("ErrorNoFigureToMove"), _errorColor, player._isUser);
                    CheckEndConditions();
                    yield break;
                }

                string name = figureToMove.gameObject.name;
                Ruleset ruleToExecute = null;
                int rulesetId = 0;
                Field fieldAfterMove = null;

                if (player == _players.GetUserPlayer()) {
                    (ruleToExecute, rulesetId, fieldAfterMove) = FindRuleToExecute(figureToMove, _rulesets, true);
                } else {
                    Rulesets rulesets =  _enemyMoves.GetNextMove(name, _actualState, _modes, _map);
                    (ruleToExecute, rulesetId, fieldAfterMove) = FindRuleToExecute(figureToMove, rulesets, false);
                    if (ruleToExecute == null) {
                        figureToMove._canMove = false;
                        continue;
                    }
                }

                // Check if field is empty (no hitting move) 
                if (ruleToExecute == null) {
                    _logger.LogStateMachineMessage(LanguageManager.Instance.GetString("ErrorNoRule"), _errorColor, player._isUser);
                    CheckEndConditions();
                    yield break;
                } else {
                    _logger.LogStateMachineMessage("Regel " + (rulesetId + 1) + " wird ausgeführt", new Color32(0, 0, 0, 255), player._isUser);
                }

                // reset can move flag of figures
                player.GetFigures().ResetFiguresToActive();
                
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

                    figureToMove = player.GetFigures().GetNextActiveFigure();
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

                // set figure to new field
                actualField = _map.GetFieldByIndices(figureToMove._positionColumn, figureToMove._positionRow);
                actualField.MoveFigureAway();
                fieldAfterMove.SetFigure(figureToMove);
                
                // set new position in figure
                figureToMove._positionColumn += ruleToExecute.GetDirection().GetColumnMovementFactor(); 
                figureToMove._positionRow += ruleToExecute.GetDirection().GetRowMovementFactor();

                // check if pawn is on last row
                if (player != _players.GetUserPlayer()) {
                    if (player._playerName == "black" && figureToMove._positionRow == 0 ||
                        player._playerName == "white" && figureToMove._positionRow == 8
                        ) {
                            _dialogueManager.ShowMessage(new Message(LanguageManager.Instance.GetString("ErrorPawnToQueen"), _errorColor, MessageIcon.MI_Error));
                            _logger.LogStateMachineMessage(LanguageManager.Instance.GetString("ErrorPawnToQueen"), _errorColor, player._isUser);
                            yield break;
                    }
                }


                //TODO now every figure of ai just moves one step
                if (moveEnds || player != _players.GetUserPlayer() || ruleToExecute.GetMode().GetModeCode() == 1 || ruleToExecute.GetMode().GetModeCode() == 2) {
                    Player nextPlayer =  _players.GetNextPlayer();
                    if (nextPlayer != null) {
                        player = nextPlayer;
                    }
                    moveEnds = false;
                    _actualDirection = null;
                }
                
                figureToMove = player.GetFigures().GetNextActiveFigure();

                yield return new WaitForSeconds(0.5f);
            }
           
        }
    }
}