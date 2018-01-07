using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;
using VRTK;

public class ButtonReactorText : MonoBehaviour
{
	public Text Text;
	public string[] TextStrings;
	private VRTK_Button _button;
	private int _crtText = -1;

	private void Awake()
	{
		_button = GetComponentInChildren<VRTK_Button>();
	}
	
	private void Start()
	{
		TextStrings = TextStrings.Select(t => t.Replace("\\n", "\n")).ToArray();
		_button.Pushed += ButtonOnPushed;
		SetNextText();
	}

	private void ButtonOnPushed(object sender, Control3DEventArgs control3DEventArgs)
	{
		SetNextText();
	}

	private void SetNextText()
	{
		_crtText++;
		_crtText %= TextStrings.Length;
		Text.text = TextStrings[_crtText];
	}
}
