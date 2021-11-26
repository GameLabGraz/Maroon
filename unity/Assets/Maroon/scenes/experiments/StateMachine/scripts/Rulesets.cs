using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class Rulesets
{
    private List<Ruleset> _rules = new List<Ruleset>();

    public void AddRuleset(Ruleset newRuleset) {
        _rules.Add(newRuleset);
    }

    public void RemoveRuleset(int position) {
        _rules.RemoveAt(position);
    }

    public Ruleset GetRulesetAtPosition(int position) {
        return _rules[position];
    }
    public int Count() {
        return _rules.Count;
    }

}
