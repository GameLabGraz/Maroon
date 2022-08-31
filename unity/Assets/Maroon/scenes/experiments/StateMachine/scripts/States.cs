using System.Collections;
using System.Collections.Generic;
using GameLabGraz.UI;
using UnityEngine;
using Maroon.UI;
using GEAR.Localization;
using TMPro;

namespace Maroon.CSE.StateMachine
{
    public class States : MonoBehaviour, IEnumerable
    {
        private List<State> _states = new List<State>();
        private DialogueManager _dialogueManager;

        private void Start()
        {
            _dialogueManager = FindObjectOfType<DialogueManager>();
            InitDropdownMenus();
        }

        private void InitDropdownMenus()
        {
            _states.Add(new State("Start"));
            _states.Add(new State("End"));

            InitDropdownMenu("StartStateDropdown", "Start");
            InitDropdownMenu("EndStateDropdown", "End");
        }

        public IEnumerator GetEnumerator()
        {
            return _states.GetEnumerator();
        }

        public void InitDropdownMenu(string dropdownName, string state)
        {

            GameObject dropdownObject = GameObject.Find(dropdownName);
            TMP_Dropdown dropdown = dropdownObject.GetComponent<TMP_Dropdown>();

            dropdown.ClearOptions();
            foreach (var item in _states)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
                option.text = item.GetStateName();
                dropdown.options.Add(option);
            }

            // extend .value method so refresh is called automatically
            dropdown.value = dropdown.options.FindIndex(text => text.text == state);
            dropdown.RefreshShownValue();
        }

        public void AddOption(string option)
        {
            AddDropdownOption("StartStateDropdown", option);
            AddDropdownOption("EndStateDropdown", option);
        }

        public void AddDropdownOption(string dropdownName, string optionName)
        {
            GameObject dropdownObject = GameObject.Find(dropdownName);
            TMP_Dropdown dropdown = dropdownObject.GetComponent<TMP_Dropdown>();

            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = optionName;
            dropdown.options.Add(option);
        }

        public void AddState()
        {
            GameObject inputFieldObject = GameObject.Find("AddNewStateInputField");
            InputField inputField = inputFieldObject.GetComponent<InputField>();

            if (inputField != null)
            {
                if (inputField.text == "")
                {
                    _dialogueManager.ShowMessage(new Message(LanguageManager.Instance.GetString("ErrorNoStateEntered"),
                        new Color32(154, 0, 11, 255), MessageIcon.MI_Error));
                    return;
                }

                bool stateNameAlreadyInUse = false;
                foreach (State stateToCheck in _states)
                {
                    if (stateToCheck.GetStateName().ToUpper() == inputField.text.ToUpper())
                    {
                        stateNameAlreadyInUse = true;
                    }
                }

                if (!stateNameAlreadyInUse)
                {
                    _states.Add(new State(inputField.text));
                    AddOption(inputField.text);
                }
                else
                {
                    if (_dialogueManager != null)
                    {
                        _dialogueManager.ShowMessage(new Message(LanguageManager.Instance.GetString("ErrorSameState"),
                            new Color32(154, 0, 11, 255), MessageIcon.MI_Error));
                    }
                }
            }

            inputField.text = "";
        }

        public State FindState(string name)
        {
            return _states.Find(element => element.GetStateName() == name);
        }

        public void RemoveState(int position)
        {
            _states.RemoveAt(position);
        }

        public State GetStateAtPosition(int position)
        {
            return _states[position];
        }

    }
}