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

	protected override void onActivate()
	{
		base.onActivate();
		togglebutton.isOn = !togglebutton.isOn;
	}
}
