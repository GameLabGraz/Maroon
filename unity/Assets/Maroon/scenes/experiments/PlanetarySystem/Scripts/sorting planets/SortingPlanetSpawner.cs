using System.Collections.Generic;
using UnityEngine;

public class SortingPlanetSpawner : MonoBehaviour
{
    public GameObject placeholder_slot;
    private GameObject[] sortingPlanets; // Array to store the prefabs of Sun, Moon, and planets

    private List<int> availablePositions = new List<int>(); // List to store available positions

    void Start()
    {
        // Initialize the availablePositions list with all possible positions (0 to 19)
        for (int i = 0; i < 20; i++)
        {
            availablePositions.Add(i);
        }

        sortingPlanets = GameObject.FindGameObjectsWithTag("sortablePlanet");
        if (sortingPlanets.Length < 1)
            Debug.Log("no sortable planets found");

        // Spawn celestial objects
        SpawnSortingPlanets();
    }

    void SpawnSortingPlanets()
    {
        for (int i = 0; i < sortingPlanets.Length; i++)
        {
            // Randomly select a position index from the availablePositions list
            int randomIndex = Random.Range(0, availablePositions.Count);

            // Get the selected position index and remove it from the list to avoid duplicate positions
            int positionIndex = availablePositions[randomIndex];
            availablePositions.RemoveAt(randomIndex);

            // Get the child object of the placeholder GameObject at the selected position index
            Transform spawnPosition = placeholder_slot.transform.GetChild(positionIndex);
            sortingPlanets[i].transform.SetParent(spawnPosition);
            sortingPlanets[i].transform.position = spawnPosition.position;

            // Instantiate the celestial object at the spawn position and set the parent to the placeholder GameObject
            //GameObject spawnedObject = Instantiate(sortingPlanets[i], spawnPosition.position, Quaternion.identity);

            // Scale the spawned object to the desired size
            // spawnedObject.transform.localScale = new Vector3(0.26f, 0.26f, 0.26f);

            // Set the parent of the spawned object to the placeholder GameObject
            //spawnedObject.transform.SetParent(placeholder_slot.transform);
        }
    }
}
