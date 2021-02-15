using UnityEngine;

public class PC_DisablePanel : MonoBehaviour
{
    public void SetActive(bool value)
    {
        gameObject.SetActive(!value);
    }
}
