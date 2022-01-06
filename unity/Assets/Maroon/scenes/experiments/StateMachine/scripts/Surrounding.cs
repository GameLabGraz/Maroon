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
    }

    private void SetValue(int column, int row, string objectName) {
        GameObject surroundingFieldObject = GameObject.Find(objectName);   
        GameObject textObject =  surroundingFieldObject.transform.GetChild(0).gameObject;
        TextMeshProUGUI textmeshObject = textObject.GetComponent(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
        _surrounding[column][row].UpdateValue();
        textmeshObject.text = _surrounding[column][row].GetName();
    }

    public void SetUpperLeftButton() {
        SetValue(0, 0, "UpperLeftButton");
    }
    public void SetUpperMiddleButton() {
        SetValue(0, 1, "UpperMiddleButton");
    }
    public void SetUpperRightButton() {
        SetValue(0, 2, "UpperRightButton");
    }
    public void SetMiddleLeftButton() {
        SetValue(1, 0, "MiddleLeftButton");
        //_surrounding[1][0] = _surrounding[1][0] + 1;
    }
    public void SetMiddleRightButton() {
        SetValue(1, 2, "MiddleRightButton");
    }
    public void SetLowerLeftButton() {
       SetValue(2, 0, "LowerLeftButton");
    }
    public void SetLowerMiddleButton() {
       SetValue(2, 1, "LowerMiddleButton");
    }
    public void SetLowerRightButton() {
        SetValue(2, 2, "LowerRightButton");
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
}
