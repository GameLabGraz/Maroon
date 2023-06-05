using System.Collections.Generic;
using UnityEngine;

public class BreadthFirstSearch : PathFindingAlgorithm
{
    public override List<string> PseudoCode => new List<string>()
    {
        "<style=\"sortingTitle\">Breadth First Search:</style>",
        "Nodes = <style=\"sortingKeyword\">List</style>(start)",
        "<style=\"sortingKeyword\">foreach</style> node in <style=\"sortingNumber\">Nodes</style>:",
        "    <style=\"sortingKeyword\">foreach</style> n in <style=\"sortingFunction\">Neighbours(node)</style>:",
        "        <style=\"sortingKeyword\">if</style> n == goal:",
        "            <style=\"sortingKeyword\">return</style> n",
        "        Nodes.<style=\"sortingFunction\">Add</style>(<style=\"sortingFunction\">Neighbours(node)</style>)",
    };

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
    private List<Node> _neighbors;
    private List<Node> _nodes;
    private List<PathFindingStep> _steps;
    private MazeElement.MazeElementType[,] layout;
    private PathFindingStep _lastStep;
    public BreadthFirstSearch()
        : base("BreadthFirstSeach")
    {
    }
    public override List<PathFindingStep> Run(MazeElement[,] elements)
    {
        _neighbors = new List<Node>();
        _nodes = new List<Node>();
        _steps = new List<PathFindingStep>();
        PathFindingStep initialStep = new PathFindingStep();
        initialStep.MazeInfos = new string[_mazeSize, _mazeSize];
        layout = new MazeElement.MazeElementType[_mazeSize, _mazeSize];
        initialStep.Parents = new Vector2Int[_mazeSize, _mazeSize];
        for (int x = 0; x < _mazeSize; ++x)
        {
            for (int y = 0; y < _mazeSize; ++y)
            {
                layout[x, y] = elements[x, y].ElementType;
                initialStep.MazeInfos[x, y] = "";
                initialStep.Parents[x, y] = new Vector2Int(-1, -1);
            }
        }
        layout[_playerPosition.x, _playerPosition.y] = MazeElement.MazeElementType.WALKED;

        initialStep.Layout = (MazeElement.MazeElementType[,])layout.Clone();
        initialStep.StepID = 0;
        Node initialNode = new Node(_playerPosition);
        _nodes.Add(initialNode);
        _steps.Add(initialStep);
        _lastStep = initialStep;
        Search();

        return _steps;
    }
    private void Search()
    {
        while (true)
        {
            foreach (Node node in _nodes)
            {
                foreach (Vector2Int n in AdjacentPaths(node.position, _lastStep.Layout, _mazeSize))
                {
                    if (_lastStep.Layout[n.x, n.y] == MazeElement.MazeElementType.PATH)
                    {
                        PathFindingStep result = new PathFindingStep(_lastStep);
                        result.NextStepDelay = 1.0f;
                        result.Layout[n.x, n.y] = MazeElement.MazeElementType.IGNORED;
                        Node neighbor = new Node(n);
                        neighbor.distance = node.distance + 1;
                        neighbor.parent = node;
                        _neighbors.Add(neighbor);
                        result.Parents[n.x, n.y] = node.position;
                        result.PseudoCodeLine = 4;
                        _steps.Add(result);
                        _lastStep = result;
                        if (n == _goalPosition)
                        {
                            MarkCorrect(result, neighbor);
                            result.NextStepDelay = -1.0f;
                            result.Complete = true;
                            result.PseudoCodeLine = 5;
                            return;
                        }
                    }
                }
            }
            _nodes.Clear();
            PathFindingStep neighborStep = new PathFindingStep(_lastStep);
            foreach (Node n in _neighbors)
            {
                neighborStep.Layout[n.position.x, n.position.y] = MazeElement.MazeElementType.WALKED;
                _nodes.Add(n);
            }
            neighborStep.NextStepDelay = 1.0f;
            _steps.Add(neighborStep);
            _lastStep = neighborStep;
            _neighbors.Clear();
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
}
