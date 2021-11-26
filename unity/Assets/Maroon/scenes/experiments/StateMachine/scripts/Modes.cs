using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modes : IEnumerable
{
    private List<Mode> _modes = new List<Mode>();
    
    public void AddMode(string modeName) {
         _modes.Add(new Mode(modeName));
    }
    public IEnumerator GetEnumerator() {
        return _modes.GetEnumerator();
    }

     public Mode FindMode(string name) {
        return _modes.Find(element => element.getModeName() == name);
    }
}
