using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InfoStand : MonoBehaviour
{
	private Text _text;
	private ButtonSceneLoad _buttonSceneLoad;

	public string ShownName;
	public string SceneName;

	private void Awake()
	{
		var textFields = GetComponentsInChildren<Text>();
		_text = textFields.First();
		_buttonSceneLoad = GetComponentInChildren<ButtonSceneLoad>();
	}

	private void Start()
	{
		_text.text = ShownName;
		_buttonSceneLoad.SceneName = SceneName;
	}
}
