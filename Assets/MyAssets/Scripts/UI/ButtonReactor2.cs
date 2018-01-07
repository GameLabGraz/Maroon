using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class ButtonReactor2 : MonoBehaviour
{
	public GameObject InvokeObject;
	public string MethodName;
	public bool IsToggle;
	public bool ToggleStartValue = false;
	private bool _toggleValue = false;
	private VRTK_Button _button;
	private Renderer _renderer;

	private void Start()
	{
		_button.Pushed += ButtonOnPushed;
		_toggleValue = ToggleStartValue;
	}

	private void ButtonOnPushed(object sender, Control3DEventArgs control3DEventArgs)
	{
		InvokeObject.SetActive(true);

		if(IsToggle)
		{
			_toggleValue = !_toggleValue;
			InvokeObject.SendMessage(MethodName, _toggleValue);

			_renderer.material.color = _toggleValue ? Color.green : Color.red;
		}
		else
		{
			InvokeObject.SendMessage(MethodName);
		}
	}

	private void Awake()
	{
		_button = GetComponentInChildren<VRTK_Button>();
		_renderer = GetComponent<Renderer>();
	}
}