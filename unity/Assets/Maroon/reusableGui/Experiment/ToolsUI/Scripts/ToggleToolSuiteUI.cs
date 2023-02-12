using UnityEngine;

public class ToggleToolSuiteUI : MonoBehaviour
{
    [SerializeField] private GameObject ToolControls;
    [SerializeField] private GameObject ToolContents;

    public void ToggleToolSelection()
    {      
        ToolControls?.SetActive(!ToolControls.activeSelf);
        ToolContents?.SetActive(false);              
    }
}
