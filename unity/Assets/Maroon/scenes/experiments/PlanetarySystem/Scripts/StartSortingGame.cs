using UnityEngine;

public class StartSortingGame : MonoBehaviour
{                                      
    void OnMouseDown()
    {
        PlanetaryController.Instance.StartSortingGameOnInput();
    }
}
