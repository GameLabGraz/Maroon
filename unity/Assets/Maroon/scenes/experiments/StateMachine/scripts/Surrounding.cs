using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;
using TMPro;

public enum SurroundingValue {
        EMPTY = 0,
        EDGE = 1,
        WHITE = 2,
        BLACK = 3,
    }
public class Surrounding : MonoBehaviour
{
    List<List<SurroundingField>> _surrounding = new List<List<SurroundingField>>();
    List<GameObject> _surroundingFieldObjects = new List<GameObject>();
    private int _columnAmount = 3;
    private int _rowAmount = 3;

    void Start() {
        for (int column = 0; column < _columnAmount; column++) {
            List<SurroundingField> tempList = new List<SurroundingField>();
            for (int row = 0; row < _rowAmount; row++) {
                tempList.Add(new SurroundingField());
            }
            _surrounding.Add(tempList);
        }
        _surroundingFieldObjects.Add(GameObject.Find("UpperLeftButton"));
        _surroundingFieldObjects.Add(GameObject.Find("UpperMiddleButton"));
        _surroundingFieldObjects.Add(GameObject.Find("UpperRightButton"));
        _surroundingFieldObjects.Add(GameObject.Find("MiddleLeftButton"));
        _surroundingFieldObjects.Add(GameObject.Find("MiddleRightButton"));
        _surroundingFieldObjects.Add(GameObject.Find("LowerLeftButton"));
        _surroundingFieldObjects.Add(GameObject.Find("LowerMiddleButton"));
        _surroundingFieldObjects.Add(GameObject.Find("LowerRightButton"));
    }

    private void SetValue(int column, int row, string objectName) {

        GameObject surroundingFieldObject = _surroundingFieldObjects.Find(entry => entry.name == objectName);
        GameObject textObject =  surroundingFieldObject.transform.GetChild(0).gameObject;
        TextMeshProUGUI textmeshObject = textObject.GetComponent(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
        _surrounding[column][row].UpdateValue();
        textmeshObject.text = _surrounding[column][row].GetName();
    }

    private void SetValue(int column, int row, string objectName, SurroundingValue value) {
        GameObject surroundingFieldObject = _surroundingFieldObjects.Find(entry => entry.name == objectName);
        GameObject textObject =  surroundingFieldObject.transform.GetChild(0).gameObject;
        TextMeshProUGUI textmeshObject = textObject.GetComponent(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
        _surrounding[column][row].SetValue(value);
        textmeshObject.text = _surrounding[column][row].GetName();
    }

    public void SetUpperLeftButton() {
        SetValue(2, 0, "UpperLeftButton");
    }
    public void SetUpperMiddleButton() {
        SetValue(2, 1, "UpperMiddleButton");
    }
    public void SetUpperRightButton() {
        SetValue(2, 2, "UpperRightButton");
    }
    public void SetMiddleLeftButton() {
        SetValue(1, 0, "MiddleLeftButton");
    }
    public void SetMiddleRightButton() {
        SetValue(1, 2, "MiddleRightButton");
    }
    public void SetLowerLeftButton() {
       SetValue(0, 0, "LowerLeftButton");
    }
    public void SetLowerMiddleButton() {
       SetValue(0, 1, "LowerMiddleButton");
    }
    public void SetLowerRightButton() {
        SetValue(0, 2, "LowerRightButton");
    }

    public SurroundingField GetSurroundingValueAt(int row, int column) {

        if (column >= _surrounding.Count || column < 0) {
            return null;
        }
        if (row >= _surrounding[column].Count || row < 0) {
            return null;
        }

        return _surrounding[column][row];
    }

    public List<List<SurroundingField>> CloneSurrounding() {
        List<List<SurroundingField>> clonedList = new List<List<SurroundingField>>();
        foreach(List<SurroundingField> list in _surrounding) {
            List<SurroundingField> tempList = new List<SurroundingField>();
            foreach(SurroundingField field in list) {
                tempList.Add(new SurroundingField(field.GetValue()));
            }
            clonedList.Add(tempList);
        }
        return clonedList;
    }

    public bool CompareSurrounding(List<List<SurroundingField>> surroundingToCheck) {

        if (surroundingToCheck.Count != _surrounding.Count) {
            return false;
        }

        for (int counter = 0; counter < _surrounding.Count; counter++) {
            if (surroundingToCheck[counter].Count != _surrounding[counter].Count) {
                return false;
            }
            for(int counter2 = 0; counter2 < _surrounding[counter].Count; counter2++) {
                if (surroundingToCheck[counter][counter2].GetValue() != _surrounding[counter][counter2].GetValue()) {
                    return false;
                }
            }
        }
        return true;
    }

    public void UpdateSurrounding(Field field, Map map) {
    
        SetElement(map, field, 1, -1, "UpperLeftButton");
        SetElement(map, field, 1, 0, "UpperMiddleButton");
        SetElement(map, field, 1, 1, "UpperRightButton");

        SetElement(map, field, 0, -1, "MiddleLeftButton");
        SetElement(map, field, 0, 1, "MiddleRightButton");

        SetElement(map, field, -1, 1, "LowerRightButton");
        SetElement(map, field, -1, 0, "LowerMiddleButton");
        SetElement(map, field, -1, -1, "LowerLeftButton");
    }

    public void SetElement(Map map, Field actualField, int column, int row, string objectName) {
        Figure figure = actualField.GetFigure();
        int positionRow = figure._positionRow;
        int positionColumn = figure._positionColumn;

        Field field = map.GetFieldByIndices(positionColumn + row, positionRow + column);

        if (field == null) {
            SetValue(column + 1, row + 1, objectName, SurroundingValue.EDGE);
            return;
        }

        if (field.GetFigure() == null) {
            SetValue(column + 1, row + 1, objectName, SurroundingValue.EMPTY);
            return;
        }

        if (field.GetFigure()._player._playerName == "white") {
            SetValue(column + 1, row + 1, objectName, SurroundingValue.WHITE);
            return;
        }

        if (field.GetFigure()._player._playerName == "black") {
            SetValue(column + 1, row + 1, objectName, SurroundingValue.BLACK);
            return;
        }

       
    }
}
