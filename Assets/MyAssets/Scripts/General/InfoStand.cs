using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InfoStand : MonoBehaviour
{
	private Text _text;

	public string Name;

	private void Awake()
	{
		var textFields = GetComponentsInChildren<Text>();
		_text = textFields.First();
	}

	private void Start()
	{
		_text.text = Name;
	}
}
