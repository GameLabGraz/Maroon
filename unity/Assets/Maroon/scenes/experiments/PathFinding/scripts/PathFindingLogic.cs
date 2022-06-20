using GEAR.Localization.Text;
using Maroon.Physics;
using Maroon.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PathFindingLogic : MonoBehaviour
{
    [SerializeField] Maze _maze;
    [SerializeField] Slider _mazeSizeSlider;
    [SerializeField] Slider _speedSlider;
    [SerializeField] Dropdown _algoDropdown;
    [SerializeField] LocalizedTMP _algoDescription;

    private PathFindingAlgorithm[] algos =
    {
        new BreadthFirstSearch(),
        new DepthFirstSeach(),
        new AStarPathFinding(),
        new DijkstraPathFinding()
    };

    protected void Start()
    {
        _mazeSizeSlider.onValueChanged.AddListener(SetMazeSize);
        _speedSlider.onValueChanged.AddListener(SetSpeed);
        _algoDropdown.onValueChanged.AddListener(SetPathfindingAlgo);
        _maze.Init((int)_mazeSizeSlider.value, algos[0]);
    }

    protected void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                MazeElement hitElement;
                if ((hitElement = hit.transform.parent.GetComponent<MazeElement>()) != null)
                {
                    _maze.InspectElement(hitElement.X, hitElement.Y);
                }
                else
                {
                    _maze.InspectElement(-1, -1);
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
        _algoDescription.Key = algos[algoIndex].Name;
    }
}
