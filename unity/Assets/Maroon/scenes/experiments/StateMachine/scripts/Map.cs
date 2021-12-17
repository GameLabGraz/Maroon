using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map 
{
    private List<List<Field>> _map = new List<List<Field>>();
    private int _amountFieldsPerRow = 8;
    private int _amountFieldsPerColumn = 8;

    public Map() {
    }
    
    public void InitMap() {

        GameObject tiles = GameObject.Find("tiles");

        int columnCount = 0;
        List<Field> column = new List<Field>();
        int rowCount = 0;

        for (int counter = 0; counter < tiles.transform.childCount; counter++) {

            if (columnCount % _amountFieldsPerColumn == 0 && columnCount != 0) {
                _map.Add(column);
                column = new List<Field>();
                columnCount = 0;
                rowCount++;
            }

            GameObject fieldObject = tiles.transform.GetChild(counter).gameObject;
            Field field = fieldObject.GetComponent(typeof(Field)) as Field;
            if (field.GetFigure()) {
                field.GetFigure()._positionColumn = rowCount;
                field.GetFigure()._positionRow = columnCount;
            }
            column.Add(field);
            columnCount++;
        }
        _map.Add(column);
    }

    public void ClearMap() {
        foreach (var column in _map)
        {
            foreach (var field in column)
            {
                field.RemoveFigure();
                field.SetDestination(false);
            }
        }
    }

    public List<List<Field>> GetMap() {
        return _map;
    }

    public List<Field> GetColumnByIndex(int index) {
        return _map[index];
    }

    public Field GetFieldByIndices(int column, int row) {
        
        if (column >= _map.Count || column < 0) {
            return null;
        }
        if (row >= _map[column].Count || row < 0) {
            return null;
        }

        return _map[column][row];
    }
}
