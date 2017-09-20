using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GuiTriboelectricExperiment : MonoBehaviour {
	
	private GUIStyle textStyle;
	
	public void Start () {
		// define GUI style
		this.textStyle = new GUIStyle("label");
		this.textStyle.alignment = TextAnchor.MiddleCenter;
	}
	
	public void Update()
	{
		// check if [Space] was pressed
		if (Input.GetKeyDown (KeyCode.Space)) 
		{
      SceneManager.LoadScene("Laboratory");
		}
	}
	
	// OnGUI is called once per frame
	public void OnGUI()
	{
		GUI.Label (new Rect (Screen.width / 2 - 200f, Screen.height / 3, 400f, 100f), "Sorry, this experiment is not ready yet.", this.textStyle);
		// show control messages on top left corner
		GUI.Label (new Rect (10f, 10f, 300f, 200f), string.Format("[ESC] - Leave"));
	}
}
