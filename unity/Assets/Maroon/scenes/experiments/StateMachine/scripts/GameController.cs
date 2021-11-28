﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maroon.UI;
using TMPro;
using System.Threading;

namespace StateMachine {
    public class GameController : MonoBehaviour
    {

        private Gamefield _gamefield;
        private Players _players = new Players();
        private Rulesets _rulesets = new Rulesets();
        private States _states;
        private Directions _directions = new Directions();
        private Moves _moves = new Moves();
        private Modes _modes = new Modes();
        // Rule counter increases every time a rule is added, but does NOT decrease (so names are always unique)
        private int _ruleCounter = 0;
        private bool _isLastRowColorFirstColor = false;
        private Color _rowColor1 = new Color(0.9f, 0.9f, 0.9f);
        private Color _rowColor2 = new Color(1.0f, 1.0f, 1.0f);

        private Map _map = new Map();


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
            Scenario1 scenario1 = new Scenario1(_map.GetMap());
            Figure figure = scenario1.GetFigure();
            _players.AddPlayer(new Player("Player white", figure));
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

            _directions.AddDirection("Oben");
            _directions.AddDirection("Unten");
            _directions.AddDirection("Links");
            _directions.AddDirection("Rechts");


            foreach (Direction item in _directions)
            {
                Dropdown.OptionData option = new Dropdown.OptionData();
                option.text = item.GetDirectionName();
                dropdown.options.Add(option);
            }
            
            //dropdown.value = 1;
            dropdown.value = 0;
            dropdown.RefreshShownValue();
        }

        void InitMoves() {
            GameObject dropdownObject = GameObject.Find("ModeDropdown");
            Dropdown dropdown = dropdownObject.GetComponent(typeof(Dropdown)) as Dropdown;
            dropdown.ClearOptions();

            _modes.AddMode("Figur schlagen");
            _modes.AddMode("Leeres Feld betreten");

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

        // Update is called once per frame
        void Update()
        {
            
        }


        public void RunStateMachine() {
            StartCoroutine(MakeMove());
        }

        IEnumerator MakeMove() {

            //Test move one pawn forward

            // Get pawn.001
            GameObject directionDropdownObject = GameObject.Find("pawn.001");

            // run state machine

            int fieldMoveDistance = 4;

            // TODO this must be start text from language plugin
            string start = "start";

            Player player = _players.GetPlayerAtIndex(0);
            Figure figureToMove = player.GetFigure();

            for (int counter = 0; counter < fieldMoveDistance; counter++) {

                // Find ruleset which should be executed
                List<Ruleset> startRulesetsFound = new List<Ruleset>();

                foreach (Ruleset ruleset in _rulesets)
                {
                    if (ruleset.GetStartState().IsStartState()) {
                        startRulesetsFound.Add(ruleset);
                    }
                }

                // TODO compare mode 
                // TODO compare surrounding of rule to surrounding of the figure to move => only one rule must be available after that comparison

                // TODO move figure one field
                 Vector3 position = figureToMove.transform.position;
                // move one field up
                figureToMove.transform.position = new Vector3(position.x, position.y + (float)0.18, position.z);
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