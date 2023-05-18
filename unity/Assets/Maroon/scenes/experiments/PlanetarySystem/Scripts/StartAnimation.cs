using UnityEngine;

public class StartAnimation : MonoBehaviour
{
    /*
     * starts Animation when the screens are clicked
     */
    void OnMouseDown()
    {
        PlanetaryController.Instance.StartAnimationOnInput();
    }
}