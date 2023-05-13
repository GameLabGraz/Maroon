using System.Collections.Generic;
using UnityEngine;

public class SortingPlanetSpawner : MonoBehaviour
{
    public GameObject sortingGamePlanetPlaceholderSlots;
    private GameObject[] sortingPlanets;

    private List<int> sortingGameAvailableSlotPositions = new List<int>();


    /*
     *
     */
    void Awake()
    {
        Debug.Log("SortingPlanetSpawner: Awake(): ");

        //SortingGame
        Debug.Log("SortingPlanetSpawner: Awake(): InitializeAvailableSortingGameSlotPositions()");
        InitializeAvailableSortingGameSlotPositions();
        Debug.Log("SortingPlanetSpawner: Awake(): SpawnSortingPlanets()");
        SpawnSortingPlanets();
    }


    /*
     * handles the SortingGame
     */
    #region SortingGameSpawner
    /*
     * Initialize the availablePositions list with all possible sorting positions
     */
    void InitializeAvailableSortingGameSlotPositions()
    {
        int boxSize = 24;
        for (int i = 0; i < boxSize; i++)
        {
            sortingGameAvailableSlotPositions.Add(i);
        }

        sortingPlanets = GameObject.FindGameObjectsWithTag("SortingGamePlanet");
        if (sortingPlanets.Length < 1)
            Debug.Log("SortingPlanetSpawner: InitializeAvailableSortingGameSlotPositions(): no sortingPlanets found");
    }


    /*
     * Spawn the SortingPlanets on a random sortingGameAvailableSlotPositions
     */
    void SpawnSortingPlanets()
    {
        for (int i = 0; i < sortingPlanets.Length; i++)
        {
            // Randomly select a position index from the sortingGameAvailableSlotPositions list
            int randomIndex = Random.Range(0, sortingGameAvailableSlotPositions.Count);

            // Get the selected position index and remove it from the list to avoid duplicate positions
            int positionIndex = sortingGameAvailableSlotPositions[randomIndex];
            sortingGameAvailableSlotPositions.RemoveAt(randomIndex);

            // Get the child object of the placeholder GameObject at the selected position index
            Transform spawnPosition = sortingGamePlanetPlaceholderSlots.transform.GetChild(positionIndex);
            sortingPlanets[i].transform.SetParent(spawnPosition);
            sortingPlanets[i].transform.position = spawnPosition.position;
        }
    }
    #endregion SortingGameSpawner
}