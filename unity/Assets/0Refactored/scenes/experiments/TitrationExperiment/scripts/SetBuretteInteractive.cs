using UnityEngine;

public class SetBuretteInteractive : MonoBehaviour 
{
    [SerializeField] private OpenBurette burette;

    public void EnableBuretteTap()
    {
        burette.interactable = true;
    }

    public void DisableBuretteTap()
    {
        burette.interactable = false;
    }
}
