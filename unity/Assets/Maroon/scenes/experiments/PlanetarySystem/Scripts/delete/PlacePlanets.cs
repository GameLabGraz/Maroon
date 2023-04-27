using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacePlanets : MonoBehaviour
{
    public GameObject boxPrefab;
    public List<GameObject> planetPrefabs;
    public int planetCount = 10;

    private void Start()
    {
        SpawnPlanets();
    }

    private void SpawnPlanets()
    {
        List<Vector3> cutoutPositions = GetCutoutPositions(boxPrefab);

        if (cutoutPositions.Count < planetCount)
        {
            Debug.LogError("Not enough cutouts for the desired number of planets.");
            return;
        }

        for (int i = 0; i < planetCount; i++)
        {
            int randomCutoutIndex = Random.Range(0, cutoutPositions.Count);
            Vector3 spawnPosition = cutoutPositions[randomCutoutIndex];
            cutoutPositions.RemoveAt(randomCutoutIndex);

            GameObject planetPrefab = planetPrefabs[Random.Range(0, planetPrefabs.Count)];
            GameObject planetInstance = Instantiate(planetPrefab, spawnPosition, Quaternion.identity);
            planetInstance.transform.SetParent(transform);
        }
    }

    private List<Vector3> GetCutoutPositions(GameObject box)
    {
        List<Vector3> positions = new List<Vector3>();

        foreach (Transform child in box.transform)
        {
            if (child.CompareTag("Cutout"))
            {
                positions.Add(child.position);
            }
        }

        return positions;
    }
}
