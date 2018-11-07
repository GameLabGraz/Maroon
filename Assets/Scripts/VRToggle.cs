using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using VRStandardAssets.Utils;
using UnityEngine.UI;


[RequireComponent(typeof(Toggle))]
[RequireComponent(typeof(VRInteractiveItem))]
public class VRToggle : VRMenuItem
{
	public Toggle togglebutton;
    private GazeOnly_SelectionRadial m_GazeOnly_SelectionRadial = null;

    public void OnEnable()
    {
        m_GazeOnly_SelectionRadial = this.GetComponent<GazeOnly_SelectionRadial>();
        if (m_GazeOnly_SelectionRadial != null)
            m_GazeOnly_SelectionRadial.OnSelectionComplete += onActivate;
    }

    public void OnDisable()
    {
        if (m_GazeOnly_SelectionRadial != null)
            m_GazeOnly_SelectionRadial.OnSelectionComplete -= onActivate;
    }

    protected override void onActivate()
	{
		base.onActivate();
		togglebutton.isOn = !togglebutton.isOn;
	}
}
