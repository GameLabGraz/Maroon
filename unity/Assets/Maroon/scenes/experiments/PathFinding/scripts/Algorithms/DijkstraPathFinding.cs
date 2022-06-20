using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraPathFinding : PathFindingAlgorithm
{
    private class Node
    {
        public Vector2Int position;
        public Node parent;
        public int distance;
        public bool discovered;
        public Node(Vector2Int pos)
        {
            position = pos;
            parent = null;
            distance = int.MaxValue;
            discovered = false;
        }
    }
    // list used as priority queue, as there apparently is none in C# whatever unity uses, and performance doesnt matter here
    
    public DijkstraPathFinding()
        : base("Dijkstra")
    { }
    public override List<PathFindingStep> Run(MazeElement[,] layout)
    {
        List<Node> queue = new List<Node>();
        Node currentNode;
        List<PathFindingStep> steps = new List<PathFindingStep>();
        PathFindingStep lastStep;
        Node start = new Node(_playerPosition);
        start.distance = 0;
        queue.Add(start);
        PathFindingStep initialStep = new PathFindingStep();
        initialStep.Layout = new MazeElement.MazeElementType[_mazeSize, _mazeSize];
        initialStep.MazeInfos = new string[_mazeSize, _mazeSize];
        initialStep.Parents = new Vector2Int[_mazeSize, _mazeSize];
        steps.Add(initialStep);
        lastStep = initialStep;
        for (int x = 0; x < _mazeSize; ++x)
        {
            for (int y = 0; y < _mazeSize; ++y)
            {
                initialStep.Layout[x, y] = layout[x, y].ElementType;
                initialStep.MazeInfos[x, y] = FormatNodeText(new Node(new Vector2Int(x, y)));
                initialStep.Parents[x, y] = new Vector2Int(-1, -1);
            }
        }
        while (queue.Count > 0)
        {
            PathFindingStep searchStep = new PathFindingStep(lastStep);

            currentNode = queue[0];
            foreach (Node node in queue)
            {
                if (node.discovered)
                    continue;
                if (node.distance > currentNode.distance)
                {
                    currentNode = node;
                }
            }
            currentNode.discovered = true;
            searchStep.Layout[currentNode.position.x, currentNode.position.y] = MazeElement.MazeElementType.IGNORED;
            searchStep.MazeInfos[currentNode.position.x, currentNode.position.y] = FormatNodeText(currentNode);
            if(currentNode.parent != null)
            {
                searchStep.Parents[currentNode.position.x, currentNode.position.y] = currentNode.parent.position;
            }
            searchStep.NextStepDelay = 1.0f;
            steps.Add(searchStep);
            lastStep = searchStep;
            foreach (Vector2Int pos in AdjacentPaths(currentNode.position, searchStep.Layout, _mazeSize))
            {
                PathFindingStep updateStep = new PathFindingStep(lastStep);
                // The element is still a normal path, create a new node
                if (searchStep.Layout[pos.x, pos.y] == MazeElement.MazeElementType.PATH)
                {
                    Node node = new Node(pos);
                    node.distance = currentNode.distance + 1;
                    node.parent = currentNode;
                    queue.Add(node);
                    updateStep.Layout[pos.x, pos.y] = MazeElement.MazeElementType.WALKED;
                    updateStep.MazeInfos[pos.x, pos.y] = FormatNodeText(node);
                    updateStep.Parents[pos.x, pos.y] = currentNode.position;
                    steps.Add(updateStep);
                    lastStep = updateStep;
                    if (pos == _goalPosition)
                    {
                        MarkCorrect(updateStep, node);
                        updateStep.NextStepDelay = -1.0f;
                        updateStep.Complete = true;
                        return steps;
                    }
                }
                // The element is already part of _queue
                else
                {
                    foreach (var node in queue)
                    {
                        if (node.position == pos && node.distance > currentNode.distance + 1)
                        {
                            // we have updated this elements distance with a shorter one
                            node.distance = currentNode.distance + 1;
                            node.parent = currentNode;
                            updateStep.MazeInfos[pos.x, pos.y] = FormatNodeText(node);
                            updateStep.Parents[pos.x, pos.y] = currentNode.position;
                            steps.Add(updateStep);
                            lastStep = updateStep;
                        }
                    }
                }
            }
        }
        return steps;
    }
    private void MarkCorrect(PathFindingStep step, Node node)
    {
        step.Layout[node.position.x, node.position.y] = MazeElement.MazeElementType.CORRECT;
        if (node.parent != null)
        {
            MarkCorrect(step, node.parent);
        }
    }

    private static string FormatNodeText(Node node)
    {
        return string.Format("Distance: {0}\nDiscovered: {1}", node.distance == int.MaxValue ? "?" : ""+node.distance, node.discovered);
    }
}
