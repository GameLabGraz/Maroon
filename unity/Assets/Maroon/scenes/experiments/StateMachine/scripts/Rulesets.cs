using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class Rulesets : IEnumerable
{
    private List<Ruleset> _rules = new List<Ruleset>();

    public (bool, string) AddRuleset(Ruleset newRuleset) {

       
        foreach (Ruleset rule in _rules) {
            // If same rule already exists
            List<List<SurroundingField>> surroundingToCheck = rule.GetSurrounding();
            List<List<SurroundingField>> surroundingToCheck2 = newRuleset.GetSurrounding();

            if (rule.GetStartState().GetStateName() == newRuleset.GetStartState().GetStateName() &&
                rule.GetEndState().GetStateName() == newRuleset.GetEndState().GetStateName() &&
                rule.GetDirection().GetDirectionName() == newRuleset.GetDirection().GetDirectionName() &&
                rule.GetMode().GetModeName() == newRuleset.GetMode().GetModeName() &&
                CompareSurrounding(surroundingToCheck, surroundingToCheck2)) {
                    return (false, "ErrorSameRuleTwice");
                }
            
            // If rule could not be desided by the state machine
            if (rule.GetStartState().GetStateName() == newRuleset.GetStartState().GetStateName() &&
                rule.GetEndState().GetStateName() == newRuleset.GetEndState().GetStateName() &&
                CompareSurrounding(surroundingToCheck, surroundingToCheck2)) {
                    return (false, "ErrorSimilarRule");
                }
        }
        _rules.Add(newRuleset);
        return (true, "");
    }

    private bool CompareSurrounding(List<List<SurroundingField>> surroundingToCheck, List<List<SurroundingField>> surroundingToCheck2) {
        if (surroundingToCheck.Count != surroundingToCheck2.Count) {
            return false;
        }
        for (int counter = 0; counter < surroundingToCheck2.Count; counter++) {
            if (surroundingToCheck[counter].Count != surroundingToCheck2[counter].Count) {
                return false;
            }
            for(int counter2 = 0; counter2 < surroundingToCheck2[counter].Count; counter2++) {
                if (surroundingToCheck[counter][counter2].GetValue() != surroundingToCheck2[counter][counter2].GetValue()) {
                    return false;
                }
            }
        }
        return true;
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
