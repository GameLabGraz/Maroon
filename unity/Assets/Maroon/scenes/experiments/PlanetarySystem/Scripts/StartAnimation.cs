using UnityEngine;

public class StartAnimation : MonoBehaviour
{
    void OnMouseDown()
    {
        PlanetaryController.Instance.StartAnimationOnInput();
    }
}