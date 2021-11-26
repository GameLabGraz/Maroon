using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;
public class Field : MonoBehaviour
{
    [SerializeField] Figure _figure;
    [SerializeField] bool _color {get; set;} // 0 = white, 1 = black

    [SerializeField] string _name;
    
    public void setFigure(Figure newFigure) {
        _figure = _figure;
    }

    public void removeFigure() {
        _figure = null;
    }

    public string getName() {
        return _name;
    }
    

    
}
