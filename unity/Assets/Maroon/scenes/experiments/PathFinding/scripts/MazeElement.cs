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
    [SerializeField] private GameObject _highlightObject;
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

    public int X { get { return _xCoord; } }
    public int Y { get { return _yCoord; } }

    private MeshRenderer _groundRenderer;

    public void Init(MazeElementType type, int x, int y)
    {
        _type = type;
        _wallObject.SetActive(type == MazeElementType.WALL);
        _groundObject.SetActive(type == MazeElementType.PATH);
        _highlightObject.SetActive(false);
        _groundRenderer = _groundObject.GetComponent<MeshRenderer>();
        _xCoord = x;
        _yCoord = y;
    }


    public void ResetObject()
    {
        if(_type != MazeElementType.WALL)
        {
            Init(MazeElementType.PATH, _xCoord, _yCoord);
        }
    }

    public void ApplyStep(MazeElementType type, string inspectorText)
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
    }

    public void MarkInspected()
    {
        if(_type != MazeElementType.WALL)
        {
            _highlightObject.SetActive(true);
        }
    }

    public void UnmarkInspected()
    {
        _highlightObject.SetActive(false);
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
