using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maroon.UI;
using GEAR.Localization;

public class States : MonoBehaviour, IEnumerable
{
    private List<State> _states = new List<State>();
    private DialogueManager _dialogueManager;
    void Start() {
        _dialogueManager = FindObjectOfType<DialogueManager>();
        initDropdownMenus();
    }

    private void initDropdownMenus() {
        _states.Add(new State("Start"));
        _states.Add(new State("Ende"));

        initDropdownMenu("StartStateDropdown", "Start");
        initDropdownMenu("EndStateDropdown", "Ende");
    }

    public IEnumerator GetEnumerator() {
        return _states.GetEnumerator();
    }

    public void initDropdownMenu(string dropdownName, string state) {

        GameObject dropdownObject = GameObject.Find(dropdownName);
        Dropdown dropdown = dropdownObject.GetComponent(typeof(Dropdown)) as Dropdown;

        dropdown.ClearOptions();
        foreach (var item in _states)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = item.GetStateName();
            dropdown.options.Add(option);
        }
        
        // extend .value method so refresh is called automatically
        dropdown.value = dropdown.options.FindIndex(text => text.text == state);
        dropdown.RefreshShownValue();
    }

    public void addOption(string option) {
        addDropdownOption("StartStateDropdown", option);
        addDropdownOption("EndStateDropdown", option);
    }

    public void addDropdownOption(string dropdownName, string optionName) {
        GameObject dropdownObject = GameObject.Find(dropdownName);
        Dropdown dropdown = dropdownObject.GetComponent(typeof(Dropdown)) as Dropdown;

        Dropdown.OptionData option = new Dropdown.OptionData();
        option.text = optionName;
        dropdown.options.Add(option);
    }

    public void addState() {
        GameObject inputFieldObject = GameObject.Find("AddNewStateInputField");
        InputField inputField = inputFieldObject.GetComponent(typeof(InputField)) as InputField;

        if (inputField != null) {
            if (inputField.text == "") {
                 _dialogueManager.ShowMessage(LanguageManager.Instance.GetString("ErrorNoStateEntered"));
                return;
            }

            bool stateNameAlreadyInUse = false;
            foreach(State stateToCheck in _states) {
                if (stateToCheck.GetStateName().ToUpper() == inputField.text.ToUpper()) {
                    stateNameAlreadyInUse = true;
                }
            }

            if (!stateNameAlreadyInUse) {
                _states.Add(new State(inputField.text));
                addOption(inputField.text);
            } else {
                if (_dialogueManager != null) {
                    _dialogueManager.ShowMessage(LanguageManager.Instance.GetString("ErrorSameState"));
                }
            }
        }
        inputField.text = "";
    }

    public State FindState(string name) {
        return _states.Find(element => element.GetStateName() == name);
    }

    public void removeState(int position) {
        _states.RemoveAt(position);
    }

    public State getStateAtPosition(int position) {
        return _states[position];
    }

}