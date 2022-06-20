using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MazeElement : MonoBehaviour, IResetObject
{
    public enum MazeElementType
    {
        WALL,
        PATH,
        WALKED,
        IGNORED,
        CORRECT,
    }
    [SerializeField] private GameObject _wallObject;
    [SerializeField] private GameObject _groundObject;
    [SerializeField] private Transform _hightlightLocation;
    [SerializeField] private GameObject _northArrow;
    [SerializeField] private GameObject _southArrow;
    [SerializeField] private GameObject _westArrow;
    [SerializeField] private GameObject _eastArrow;
    [SerializeField] private Material _pathMaterial;
    [SerializeField] private Material _ignoredMaterial;
    [SerializeField] private Material _walkedMaterial;
    [SerializeField] private Material _correctMaterial;
    private MazeElementType _type;
    private string _inspectorText;
    private int _xCoord;
    private int _yCoord;

    public MazeElementType ElementType { get { return _type; } }

    public string InspectorText { get { return _inspectorText; } }

    public Transform HightlightLocation { get { return _hightlightLocation; } }

    public int X { get { return _xCoord; } }
    public int Y { get { return _yCoord; } }

    private MeshRenderer _groundRenderer;

    public void Init(MazeElementType type, int x, int y)
    {
        _type = type;
        _wallObject.SetActive(type == MazeElementType.WALL);
        _groundObject.SetActive(type == MazeElementType.PATH);
        _groundRenderer = _groundObject.GetComponent<MeshRenderer>();
        _xCoord = x;
        _yCoord = y;
        _northArrow.SetActive(false);
        _southArrow.SetActive(false);
        _eastArrow.SetActive(false);
        _westArrow.SetActive(false);
    }


    public void ResetObject()
    {
        if(_type != MazeElementType.WALL)
        {
            Init(MazeElementType.PATH, _xCoord, _yCoord);
        }
        _northArrow.SetActive(false);
        _southArrow.SetActive(false);
        _eastArrow.SetActive(false);
        _westArrow.SetActive(false);
    }

    public void ShowParent(int parentX, int parentY)
    {
        if (parentX < 0 || parentY < 0)
            return;
        if(parentX < _xCoord)
        {
            _northArrow.SetActive(true);
        }
        else if (parentX > _xCoord)
        {
            _southArrow.SetActive(true);
        }
        else if (parentY < _yCoord)
        {
            _eastArrow.SetActive(true);
        }
        else if (parentY > _yCoord)
        {
            _westArrow.SetActive(true);
        }
    }

    public void ApplyStep(MazeElementType type, string inspectorText, Vector2Int parent)
    {
        _type = type;
        switch (type)
        {
            case MazeElementType.WALL:
                _wallObject.SetActive(true);
                _groundObject.SetActive(false);
                break;
            case MazeElementType.PATH:
                _wallObject.SetActive(false);
                _groundObject.SetActive(true);
                _groundRenderer.material = _pathMaterial;
                break;
            case MazeElementType.WALKED:
                _wallObject.SetActive(false);
                _groundObject.SetActive(true);
                _groundRenderer.material = _walkedMaterial;
                break;
            case MazeElementType.IGNORED:
                _wallObject.SetActive(false);
                _groundObject.SetActive(true);
                _groundRenderer.material = _ignoredMaterial;
                break;
            case MazeElementType.CORRECT:
                _wallObject.SetActive(false);
                _groundObject.SetActive(true);
                _groundRenderer.material = _correctMaterial;
                break;
            default:
                break;
        }
        _inspectorText = inspectorText;
        ShowParent(parent.x, parent.y);
    }

    #region MazeGeneration
    // Most maze generation works by starting with walls and making paths
    public void MakePath()
    {
        Debug.Assert(_type == MazeElementType.WALL);
        _type = MazeElementType.PATH;
        _wallObject.SetActive(false);
        _groundObject.SetActive(true);
    }

    // But some maze generations also retreat in case of a loop for example
    public void MakeWall()
    {
        Debug.Assert(_type == MazeElementType.PATH);
        _type = MazeElementType.WALL;
        _wallObject.SetActive(true);
        _groundObject.SetActive(false);
    }


    #endregion
}
