using UnityEngine;

public class ToggleToolUI : MonoBehaviour
{
    [SerializeField] private GameObject ToolControls;
    [SerializeField] private GameObject ToolContents;

    public void ToggleToolSelection()
    {      
        ToolControls?.SetActive(!ToolControls.activeSelf);
        ToolContents?.SetActive(false);              
    }
}
