using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;
public class Field : MonoBehaviour
{
    [SerializeField] Figure _figure;
    [SerializeField] bool _color {get; set;} // 0 = white, 1 = black

    [SerializeField] string _name;

    bool _isDestination = false;
    
    public void SetFigure(Figure newFigure) {
        _figure = newFigure;
    }

    public void RemoveFigure() {
        if (_figure != null) {
            _figure.gameObject.SetActive(false);
            _figure = null;
        }
    }

    public void SetDestination(bool isDestination) {
        _isDestination = isDestination;
    }

    public bool IsDestination() {
        if (_isDestination == true) {
            return true;
        }
        return false;
    }
    public string GetName() {
        return _name;
    }
    
    public Figure GetFigure() {
        return _figure;
    }
    
}
