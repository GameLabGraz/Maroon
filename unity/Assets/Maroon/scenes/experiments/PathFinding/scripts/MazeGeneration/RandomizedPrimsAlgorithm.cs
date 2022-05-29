using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizedPrimsAlgorithm : MazeGeneration
{
    public override void Generate(MazeElement[,] elements, int size)
    {
        Vector2Int startingPoint = new Vector2Int(Random.Range(0, size), Random.Range(0, size));
        elements[startingPoint.x, startingPoint.y].MakePath();
        List<Vector2Int> walls = new List<Vector2Int>(CrossAdjacents(startingPoint, size));
        while(walls.Count > 0)
        {
            Vector2Int chosenWall = walls[Random.Range(0, walls.Count)];
            Debug.Assert(elements[chosenWall.x, chosenWall.y].ElementType == MazeElement.MazeElementType.WALL);
            List<Vector2Int> adjacents = new List<Vector2Int>(CrossAdjacents(chosenWall, size));
            List<Vector2Int> candidates = new List<Vector2Int>(CrossAdjacentWalls(chosenWall, elements, size));

            if(candidates.Count == adjacents.Count - 1)
            {
                Vector2Int selectedPath = candidates[Random.Range(0, candidates.Count)];
                elements[chosenWall.x, chosenWall.y].MakePath();
                elements[selectedPath.x, selectedPath.y].MakePath();
                walls.AddRange(CrossAdjacentWalls(selectedPath, elements, size));
            }
            walls.Remove(chosenWall);
        }
    }
}
