using Maroon.Physics;
using Maroon.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PathFindingLogic : MonoBehaviour
{
    [SerializeField] Maze _maze;
    [SerializeField] Slider _mazeSizeSlider;
    [SerializeField] Slider _speedSlider;
    [SerializeField] TextMeshProUGUI _inspectorText;
    [SerializeField] InputField _inspectorX;
    [SerializeField] InputField _inspectorY;
    [SerializeField] Dropdown _algoDropdown;

    private PathFindingAlgorithm[] algos =
    {
        new DepthFirstSeach(),
        new AStarPathFinding(),
        new DijkstraPathFinding()
    };

    protected void Start()
    {
        _mazeSizeSlider.onValueChanged.AddListener(SetMazeSize);
        _speedSlider.onValueChanged.AddListener(SetSpeed);
        _algoDropdown.onValueChanged.AddListener(SetPathfindingAlgo);
        _inspectorX.onValueChanged.AddListener(InspectorXChanged);
        _inspectorY.onValueChanged.AddListener(InspectorYChanged);
        _maze.Init((int)_mazeSizeSlider.value, algos[0]);
    }

    private void InspectorXChanged(string arg0)
    {
        // on reset it becomes empty
        if (arg0.Length == 0)
        {
            return;
        }
        if(_inspectorY.text.Length == 0)
        {
            _inspectorY.text = "0";
        }
        _maze.InspectElement(int.Parse(arg0), int.Parse(_inspectorY.text));
    }

    private void InspectorYChanged(string arg0)
    {
        // on reset it becomes empty
        if (arg0.Length == 0)
        {
            return;
        }
        if (_inspectorX.text.Length == 0)
        {
            _inspectorX.text = "0";
        }
        _maze.InspectElement(int.Parse(_inspectorX.text), int.Parse(arg0));
    }

    protected void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                MazeElement hitElement;
                if ((hitElement = hit.transform.parent.GetComponent<MazeElement>()) != null)
                {
                    _inspectorText.text = hitElement.InspectorText;
                    _inspectorX.text = ""+hitElement.X;
                    _inspectorY.text = ""+hitElement.Y;
                    _maze.InspectElement(hitElement.X, hitElement.Y);
                }
            }
        }
    }
    public void SetMazeSize(float size)
    {
        _maze.SetMazeSize((int)size);
    }
    public void SetSpeed(float speed)
    {
        _maze.SetSpeed(speed);
    }
    private void SetPathfindingAlgo(int algoIndex)
    {
        _maze.SetAlgorithm(algos[algoIndex]);
    }

}
