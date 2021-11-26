﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maroon.UI;

public class States : MonoBehaviour
{
    private List<State> _states = new List<State>();

    void Start() {
        initDropdownMenus();
    }

    private void initDropdownMenus() {
        _states.Add(new State("Start"));
        _states.Add(new State("Ende"));

       
        
        initDropdownMenu("StartStateDropdown", "Start");
        initDropdownMenu("EndStateDropdown", "Ende");
    }

    public void initDropdownMenu(string dropdownName, string state) {

        GameObject dropdownObject = GameObject.Find(dropdownName);
        Dropdown dropdown = dropdownObject.GetComponent(typeof(Dropdown)) as Dropdown;

        dropdown.ClearOptions();
        foreach (var item in _states)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = item.getStateName();
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
            _states.Add(new State(inputField.text));
            addOption(inputField.text);
        }
    }

    public State FindState(string name) {
        return _states.Find(element => element.getStateName() == name);
    }

    public void removeState(int position) {
        _states.RemoveAt(position);
    }

    public State getStateAtPosition(int position) {
        return _states[position];
    }

}