using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthFirstSeach : PathFindingAlgorithm
{
    private class Node
    {
        public Vector2Int position;
        public Node parent;
        public int distance;
        public Node(Vector2Int pos)
        {
            position = pos;
            parent = null;
            distance = 0;
        }
    }
    private List<Node> _nodes;
    private Dictionary<int, PathFindingStep> _steps;
    private MazeElement.MazeElementType[,] layout;
    private int _stepNum = 0;
    private bool done;
    public DepthFirstSeach()
        : base("DepthFirstSeach")
    {
    }

    public override List<PathFindingStep> Run(MazeElement[,] elements)
    {
        _nodes = new List<Node>();
        _steps = new Dictionary<int, PathFindingStep>();
        _stepNum = 0;
        PathFindingStep initialStep = new PathFindingStep();
        initialStep.MazeInfos = new string[_mazeSize, _mazeSize];
        done = false;
        layout = new MazeElement.MazeElementType[_mazeSize, _mazeSize];
        for (int x = 0; x < _mazeSize; ++x)
        {
            for (int y = 0; y < _mazeSize; ++y)
            {
                layout[x, y] = elements[x, y].ElementType;
                initialStep.MazeInfos[x, y] = "No parent";
            }
        }
        layout[_playerPosition.x, _playerPosition.y] = MazeElement.MazeElementType.WALKED;

        initialStep.Layout = (MazeElement.MazeElementType[,])layout.Clone();
        initialStep.StepID = 0;
        Node initialNode = new Node(_playerPosition);
        _nodes.Add(initialNode);
        _steps[_stepNum++] = initialStep;
        Search(initialStep, initialNode);
        
        return new List<PathFindingStep>(_steps.Values);
    }
    private void Search(PathFindingStep previous, Node prevNode)
    {
        foreach(Vector2Int n in AdjacentPaths(prevNode.position, layout, _mazeSize))
        {
            if (done)
                return;
            PathFindingStep result = new PathFindingStep();
            result.MazeInfos = (string[,])previous.MazeInfos.Clone();
            result.StepID = _stepNum;
            result.NextStepDelay = 1.0f;
            if(layout[n.x, n.y] == MazeElement.MazeElementType.PATH)
            {
                Node node = new Node(n);
                node.parent = prevNode;
                node.distance = prevNode.distance + 1;
                _nodes.Add(node);
                layout[n.x, n.y] = MazeElement.MazeElementType.WALKED;
                result.Layout = (MazeElement.MazeElementType[,])layout.Clone();
                if (n == _goalPosition)
                {
                    MarkCorrect(result, node);
                    result.Complete = true;
                    done = true;
                    _steps[_stepNum++] = result;
                    return;
                }
                _steps[_stepNum++] = result;
                Search(result, node);
            }
            else
            {
                foreach(Node node in _nodes)
                {
                    if(node.position ==  n)
                    {
                        if(node.distance > prevNode.distance + 1)
                        {
                            node.distance = prevNode.distance + 1;
                            node.parent = prevNode;
                            result.MazeInfos[n.x, n.y] = FormatNodeString(node);
                        }
                    }
                }
            }
        }
    }
    private void MarkCorrect(PathFindingStep step, Node node)
    {
        step.Layout[node.position.x, node.position.y] = MazeElement.MazeElementType.CORRECT;
        if (node.parent != null)
        {
            MarkCorrect(step, node.parent);
        }
    }
    private string FormatNodeString(Node node)
    {
        if(node.parent != null)
        {
            return string.Format("parent: [{0}, {1}]", node.parent.position.x, node.parent.position.y);
        }
        return string.Format("No parent");
    }
}
