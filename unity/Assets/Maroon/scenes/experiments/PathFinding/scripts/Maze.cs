using Maroon.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Maze : PausableObject, IResetObject
{
    [SerializeField] private Vector2Int _playerPosition;
    [SerializeField] private Vector2Int _goalPosition;
    [SerializeField] private GameObject _mazeElementPrefab;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _goalPrefab;
    [SerializeField] private GameObject _camera;
    [SerializeField] private Transform _leftBorder;
    [SerializeField] private Transform _rightBorder;
    [SerializeField] private Transform _upperBorder;
    [SerializeField] private Transform _lowerBorder;
    [SerializeField] private TextMeshProUGUI _algoPseudoCode;

    private GameObject _player;
    private GameObject _goal;
    private GameObject[,] _layout;
    private MazeElement[,] _mazeElements;
    private MazeGeneration _mazeGenerator;
    private PathFindingAlgorithm _pathFinding;
    private int inspectedX = -1;
    private int inspectedY = -1;
    private int _currentSize = 0;
    private List<PathFindingStep> _steps;
    private int _currentStep = 0;
    private float _lastUpdateTime;
    private float _speedModifier;
    private Vector3 _cameraBaseLocation;
    private Quaternion _cameraBaseRotation;
    // Save these up here as they can be useful for spawning and
    // moving the player, goal or other things
    private Vector3 _horizontalStep;
    private Vector3 _verticalStep;
    private Vector3 _basePosition;
    private Vector3 _elementScale;

    public void Init(int size, PathFindingAlgorithm algo)
    {
        if (_currentSize != 0)
        {
            for (int i = 0; i < _currentSize; ++i)
            {
                for (int j = 0; j < _currentSize; j++)
                {
                    Destroy(_layout[i, j]);
                }
            }
            Destroy(_player);
            Destroy(_goal);
        }
        _mazeGenerator = new RandomizedPrimsAlgorithm();
        _pathFinding = algo;
        _currentSize = size;
        _speedModifier = 1.0f;
        _steps = new List<PathFindingStep>();
        _layout = new GameObject[size, size];
        _mazeElements = new MazeElement[size, size];
        _cameraBaseLocation = _camera.transform.position;
        _cameraBaseRotation = _camera.transform.rotation;
        _elementScale = new Vector3(1.0f / size, 1.0f / size, 1.0f / size);
        _horizontalStep = (_rightBorder.position - _leftBorder.position) / size;
        _verticalStep = (_lowerBorder.position - _upperBorder.position) / size;
        _basePosition = new Vector3(_leftBorder.position.x, _upperBorder.position.y, _upperBorder.position.z)
                                    + (_horizontalStep / 2.0f) + (_verticalStep / 2.0f);
        for (int x = 0; x < size; ++x)
        {
            for (int y = 0; y < size; ++y)
            {
                _layout[x, y] = SpawnPrefab(_mazeElementPrefab, x, y);
                _mazeElements[x, y] = _layout[x, y].GetComponent<MazeElement>();
                // Start with all walls, as that is what the maze generation expects
                _mazeElements[x, y].Init(MazeElement.MazeElementType.WALL, x, y);
            }
        }
        _mazeGenerator.Generate(_mazeElements, size);
        SpawnPlayer();
        SpawnGoal();
        SetAlgorithm(algo);
    }

    public void SetMazeSize(int size)
    {
        Init(size, _pathFinding);
    }

    public void SetAlgorithm(PathFindingAlgorithm pathFindingAlgorithm)
    {
        InspectElement(-1, -1);
        _pathFinding = pathFindingAlgorithm;
        SimulationController.Instance.ResetSimulation();
        _steps.Clear();
        _pathFinding.Init(_currentSize, _playerPosition, _goalPosition);
        _steps = _pathFinding.Run(_mazeElements);
        _currentStep = 0;
        ApplyStep();
    }

    public void InspectElement(int elementX, int elementY)
    {
        if (elementX == -1 || elementY == -1 || (inspectedX == elementX && inspectedY == elementY))
        {
            _camera.transform.position = _cameraBaseLocation;
            _camera.transform.rotation = _cameraBaseRotation;
            inspectedX = -1;
            inspectedY = -1;
            for (int x = 0; x < _currentSize; ++x)
            {
                for (int y = 0; y < _currentSize; ++y)
                {
                    _mazeElements[x, y].HideElementText();
                }
            }
        }
        else
        {
            for (int x = 0; x < _currentSize; ++x)
            {
                for (int y = 0; y < _currentSize; ++y)
                {
                    _mazeElements[x, y].ShowElementText();
                    if (x == elementX && y == elementY)
                    {
                        _camera.transform.position = _mazeElements[x, y].HightlightLocation.position;
                        _camera.transform.rotation = _mazeElements[x, y].HightlightLocation.rotation;
                        inspectedX = elementX;
                        inspectedY = elementY;
                    }
                }
            }
        }
    }

    protected override void HandleUpdate()
    {
        InspectElement(-1, -1);
        if (_steps[_currentStep].Complete)
            return;
        _lastUpdateTime += (Time.deltaTime * _speedModifier);
        if(_lastUpdateTime >= _steps[_currentStep].NextStepDelay)
        {
            _lastUpdateTime -= _steps[_currentStep].NextStepDelay;
            _currentStep++;
            ApplyStep();
        }
    }

    protected override void HandleFixedUpdate()
    {
        // Nothing to do
    }

    public void SkipForward()
    {
        //InspectElement(-1, -1);
        _lastUpdateTime = 0;
        _currentStep = Math.Min(_currentStep + 1, _steps.Count - 1);
        ApplyStep();
    }

    public void SkipBackward()
    {
        //InspectElement(-1, -1);
        _lastUpdateTime = 0;
        _currentStep = Math.Max(_currentStep - 1, 0);
        ApplyStep();
    }

    private void SpawnPlayer()
    {
        for (int x = 0; x < _currentSize; ++x)
        {
            for (int y = 0; y < _currentSize; ++y)
            {
                if (_mazeElements[x, y].ElementType == MazeElement.MazeElementType.PATH)
                {
                    _player = SpawnPrefab(_playerPrefab, x, y);
                    _playerPosition.Set(x, y);
                    return;
                }
            }
        }
    }

    private void SpawnGoal()
    {
        for (int x = _currentSize - 1; x >= 0; --x)
        {
            for (int y = _currentSize - 1; y >= 0; --y)
            {
                if (_mazeElements[x, y].ElementType == MazeElement.MazeElementType.PATH)
                {
                    _goal = SpawnPrefab(_goalPrefab, x, y);
                    _goalPosition.Set(x, y);
                    return;
                }
            }
        }
    }

    // Spawns a prefab at the specified x and y location relative to the maze
    private GameObject SpawnPrefab(GameObject prefab, int x, int y)
    {
        GameObject result = Instantiate(prefab, transform);
        result.transform.position = _basePosition + (x * _verticalStep) + (y * _horizontalStep);
        result.transform.localScale = _elementScale;
        return result;
    }

    private void ApplyStep()
    {
        for(int x = 0; x < _currentSize; ++x)
        {
            for(int y = 0; y < _currentSize; ++y)
            {
                _mazeElements[x, y].ApplyStep(_steps[_currentStep].Layout[x, y], _steps[_currentStep].MazeInfos[x, y], _steps[_currentStep].Parents[x, y]);
            }
        }
        DisplayPseudocode(_pathFinding.PseudoCode, -1); //_steps[_currentStep].PseudoCodeLine);
        // dont highlight lines, doesnt really make sense for most steps
    }

    public void ResetObject()
    {
        _currentStep = 0;
        _lastUpdateTime = 0;
        if(_steps.Count > 0)
        {
            ApplyStep();
        }
    }

    public void SetSpeed(float speed)
    {
        _speedModifier = speed;
    }


    public void DisplayPseudocode(List<string> pseudocode, int highlightLine)
    {
        string highlightedCode = "";
        for (int i = 0; i < pseudocode.Count; ++i)
        {
            if (i == highlightLine)
            {
                highlightedCode += "<color=#810000>></color> " + pseudocode[i] + "\n";
            }
            else
            {
                highlightedCode += "  " + pseudocode[i] + "\n";
            }
        }

        _algoPseudoCode.text = highlightedCode;
    }
}
