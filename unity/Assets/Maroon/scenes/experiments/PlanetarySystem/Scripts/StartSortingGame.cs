using UnityEngine;

public class StartSortingGame : MonoBehaviour
{       
    /*
     * starts SortingGame when the screen is clicked
     */
    void OnMouseDown()
    {
        PlanetaryController.Instance.StartSortingGameOnInput();
    }
}
