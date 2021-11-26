using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Directions : IEnumerable
{
    private List<Direction> _directions = new List<Direction>();
    
    public void AddDirection(string directionName) {
         _directions.Add(new Direction(directionName));
    }
    public IEnumerator GetEnumerator() {
        return _directions.GetEnumerator();
    }

     public Direction FindDirection(string name) {
        return _directions.Find(element => element.getDirectionName() == name);
    }
}
