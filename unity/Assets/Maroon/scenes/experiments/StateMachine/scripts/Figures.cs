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

    public void RemoveFigure(int position) {
        _figures.RemoveAt(position);
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
        return _figures.Count;
    }

    public IEnumerator GetEnumerator() {
        return _figures.GetEnumerator();
    }
}
