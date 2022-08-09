using System.Collections.Generic;
using UnityEngine;

public abstract class PathFindingAlgorithm
{
    protected int _mazeSize;
    protected Vector2Int _playerPosition;
    protected Vector2Int _goalPosition;
    private string _name;

    public abstract List<string> PseudoCode { get; }

    public string Name
    {
        get { return _name; }
    }


    public PathFindingAlgorithm(string name)
    {
        _name = name;
    }

    public virtual void Init(int mazeSize, Vector2Int playerPosition, Vector2Int goalPosition)
    {
        _playerPosition = playerPosition;
        _goalPosition = goalPosition;
        _mazeSize = mazeSize;
    }

    public abstract List<PathFindingStep> Run(MazeElement[,] layout);

    protected IEnumerable<Vector2Int> AdjacentPaths(Vector2Int point, MazeElement.MazeElementType[,] elements, int size)
    {
        foreach (Vector2Int vector in Adjacents(point, size))
        {
            if (elements[vector.x, vector.y] != MazeElement.MazeElementType.WALL)
            {
                yield return vector;
            }
        }
    }
    // Utility function for getting the neighbouring coordinates for easy iteration
    protected IEnumerable<Vector2Int> Adjacents(Vector2Int point, int size)
    {
        // check if point is valid
        Debug.Assert(point.x > -1 && point.x < size && point.y > -1 && point.y < size);
        if (point.y > 0)
        {
            yield return new Vector2Int(point.x, point.y - 1);
        }
        if (point.x < size - 1)
        {
            yield return new Vector2Int(point.x + 1, point.y);
        }
        if (point.y < size - 1)
        {
            yield return new Vector2Int(point.x, point.y + 1);
        }
        if (point.x > 0)
        {
            yield return new Vector2Int(point.x - 1, point.y);
        }
    }
}
