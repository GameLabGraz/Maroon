using System.Collections.Generic;
using UnityEngine;

public class SortingPlanetSpawner : MonoBehaviour
{
    public GameObject placeholder_slot;
    private GameObject[] sortingPlanets; 

    private List<int> availablePositions = new List<int>();


    /*
     * 
     */
    void Start()
    {
        // Initialize the availablePositions list with all possible positions
        for (int i = 0; i < 24; i++)
        {
            availablePositions.Add(i);
        }

        sortingPlanets = GameObject.FindGameObjectsWithTag("sortablePlanet");
        if (sortingPlanets.Length < 1)
            Debug.Log("no sortable planets found");

        SpawnSortingPlanets();
    }


   /*
    * 
    */ 
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
        }
    }
}
