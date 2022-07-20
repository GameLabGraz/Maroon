using System.Collections;
using System.Collections.Generic;
using Maroon.CSE.StateMachine;

public class Rulesets : IEnumerable
{
    private List<Ruleset> _rules = new List<Ruleset>();

    public (bool, string) AddRuleset(Ruleset newRuleset) {

       
        foreach (Ruleset rule in _rules) {
            // If same rule already exists
            if (rule.IsSameRule(newRuleset)) {
                return (false, "ErrorSameRuleTwice");
            }
            
            // If rule could not be desided by the state machine
            if (rule.IsRuleNotDesidable(newRuleset)) {
                    return (false, "ErrorSimilarRule");
                }
        }
        _rules.Add(newRuleset);
        return (true, "");
    }

    

    public void RemoveRuleset(int id) {
        int removeIndex = 0;
        for (int counter = 0; counter < _rules.Count; counter++) {
            if (_rules[counter].GetId() == id) {
                removeIndex = counter;
                break;
            }
        }
        _rules.RemoveAt(removeIndex);
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
