using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class Figures : IEnumerable
{
    private List<Figure> _figures = new List<Figure>();

    public void AddFigure(Figure newFigure) {

        _figures.Add(newFigure);
    }

    public Figure GetNextActiveFigure() {
        foreach(Figure figure in _figures) {
            if (figure._canMove && figure.gameObject.activeSelf) {
                return figure;
            }
        }
        return null;
    }

    public void ResetFiguresToActive() {
        foreach(Figure figure in _figures) {
            figure._canMove = true;
        }
    }

    public void RemoveFigure(int position) {
        if (_figures.Count > position) {
            _figures.RemoveAt(position);
        }
    }

    public Figure GetFigureAtPosition(int position) {
        if (position < _figures.Count) {
            return _figures[position];
        }
        return null;
    }
    public Figure GetFigureByName(string name) {
        return _figures.Find(element => element.gameObject.name == name);
    }
    public int Count() {
        int counter = 0;
        foreach(Figure figure in _figures) {
            if (figure.gameObject.activeSelf) {
                counter++;
            }
        }
        return counter;
    }

    public IEnumerator GetEnumerator() {
        return _figures.GetEnumerator();
    }
}
