using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MazeGeneration
{
    public abstract void Generate(MazeElement[,] elements, int size);

    protected IEnumerable<Vector2Int> CrossAdjacentWalls(Vector2Int point, MazeElement[,] elements, int size)
    {
        foreach (Vector2Int vector in CrossAdjacents(point, size))
        {
            if (elements[vector.x, vector.y].ElementType == MazeElement.MazeElementType.WALL)
            {
                yield return vector;
            }
        }
    }
    protected IEnumerable<Vector2Int> AdjacentWalls(Vector2Int point, MazeElement[,] elements, int size)
    {
        foreach (Vector2Int vector in Adjacents(point, size))
        {
            if (elements[vector.x, vector.y].ElementType == MazeElement.MazeElementType.WALL)
            {
                yield return vector;
            }
        }
    }
    protected IEnumerable<Vector2Int> CrossAdjacentPaths(Vector2Int point, MazeElement[,] elements, int size)
    {
        foreach (Vector2Int vector in CrossAdjacents(point, size))
        {
            if (elements[vector.x, vector.y].ElementType == MazeElement.MazeElementType.PATH)
            {
                yield return vector;
            }
        }
    }
    // Utility function for getting the neighbouring coordinates for easy iteration
    // Only gives the vertical or horizontal adjacents, no diagonals
    protected IEnumerable<Vector2Int> CrossAdjacents(Vector2Int point, int size)
    {
        // check if point is valid
        Debug.Assert(point.x > -1 && point.x < size && point.y > -1 && point.y < size);
        if (point.x > 0)
        {
            yield return new Vector2Int(point.x - 1, point.y);
        }
        if (point.x < size - 1)
        {
            yield return new Vector2Int(point.x + 1, point.y);
        }
        if (point.y > 0)
        {
            yield return new Vector2Int(point.x, point.y - 1);
        }
        if (point.y < size - 1)
        {
            yield return new Vector2Int(point.x, point.y + 1);
        }
    }
    // Utility function for getting the neighbouring coordinates for easy iteration
    // Also includes diagonals
    protected IEnumerable<Vector2Int> Adjacents(Vector2Int point, int size)
    {
        // check if point is valid
        Debug.Assert(point.x > -1 && point.x < size && point.y > -1 && point.y < size);
        // NorthEast
        if (point.x > 0 && point.y > 0)
        {
            yield return new Vector2Int(point.x - 1, point.y - 1);
        }
        // North
        if (point.y > 0)
        {
            yield return new Vector2Int(point.x, point.y - 1);
        }
        // NorthWest
        if (point.x < size - 1 && point.y > 0)
        {
            yield return new Vector2Int(point.x + 1, point.y - 1);
        }
        // East
        if (point.x > 0)
        {
            yield return new Vector2Int(point.x - 1, point.y);
        }
        // West
        if (point.x < size - 1)
        {
            yield return new Vector2Int(point.x + 1, point.y);
        }
        // SouthEast
        if (point.x > 0 && point.y < size - 1)
        {
            yield return new Vector2Int(point.x - 1, point.y + 1);
        }
        // South
        if (point.y < size - 1)
        {
            yield return new Vector2Int(point.x, point.y + 1);
        }
        // SouthWest
        if (point.x < size - 1 && point.y < size - 1)
        {
            yield return new Vector2Int(point.x + 1, point.y + 1);
        }
    }
}
