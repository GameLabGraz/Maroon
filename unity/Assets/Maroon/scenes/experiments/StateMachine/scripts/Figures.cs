using System.Collections.Generic;
using System.Collections;
using Maroon.CSE.StateMachine;
using System.Linq;

public class Figures : IEnumerable
{
    private List<Figure> _figures = new List<Figure>();

    public void AddFigure(Figure newFigure) {
        _figures.Add(newFigure);
    }

    public Figure GetNextActiveFigure() {
        return _figures.FirstOrDefault(figure => figure._canMove && figure.gameObject.activeSelf);
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
        return position < _figures.Count ? _figures[position] : null;
    }
    public Figure GetFigureByName(string name) {
        return _figures.Find(element => element.gameObject.name == name);
    }
    public int Count() {
        return _figures.Count(figure => figure.gameObject.activeSelf);
    }

    public IEnumerator GetEnumerator() {
        return _figures.GetEnumerator();
    }
}
