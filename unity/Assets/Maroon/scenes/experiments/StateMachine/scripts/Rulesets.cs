using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class Rulesets : IEnumerable
{
    private List<Ruleset> _rules = new List<Ruleset>();

    public void AddRuleset(Ruleset newRuleset) {

        //TODO check if ruleset with same start and surrounding exist => error

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

    public IEnumerator GetEnumerator() {
        return _rules.GetEnumerator();
    }

    public int GetCount() {
        return _rules.Count;
    }
}
