using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using VRStandardAssets.Utils;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
[RequireComponent(typeof(VRInteractiveItem))]
public class VRButton : VRMenuItem
{
    [SerializeField] private GazeOnly_SelectionRadial m_GazeOnly_SelectionRadial;

    protected override void Start()
    {
        if (m_GazeOnly_SelectionRadial != null)
            m_GazeOnly_SelectionRadial.OnSelectionComplete += onActivate;
        base.Start();
    }

    private void OnDestroy()
    {
        if (m_GazeOnly_SelectionRadial != null)
            m_GazeOnly_SelectionRadial.OnSelectionComplete -= onActivate;
    }
    protected override void onActivate()
    {
        base.onActivate();
        GetComponent<Button>().onClick.Invoke();
    }
}