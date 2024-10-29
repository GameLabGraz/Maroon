using System.Collections.Generic;
using UnityEngine;

public class ToolController : MonoBehaviour
{
    [SerializeField] private bool ActivateAdvancedTools;
    [SerializeField] private List<GameObject> AdvancedTools;
    [SerializeField] private List<GameObject> BasicTools;

    private void Start()
    {
        // Only activate advanced tool if experiment requires them
        foreach (var advancedTool in AdvancedTools)
        {
            advancedTool.SetActive(ActivateAdvancedTools);
        }

        // Always activate basic tools
        foreach (var basicTool in BasicTools)
        {
            basicTool.SetActive(true);
        }
    }
}
