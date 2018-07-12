using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GuiWhiteboard : MonoBehaviour 
{
	WhiteboardController whiteboardController;
	GUIStyle textStyle;
	Vector2 scrollViewVector = Vector2.zero;
	int selectedLectureIndex = 0;
	string[] lectureNames;
	bool showMenu = true;

	public void Start () 
	{
		// Define GUI style
		textStyle = new GUIStyle("label");
		textStyle.alignment = TextAnchor.MiddleCenter;

		// Find Whiteboard GameObject in the scene
		GameObject whiteboard = GameObject.FindGameObjectWithTag ("WhiteboardPlane");
		if (null == whiteboard) {
			throw new System.ArgumentNullException("No WhiteboardPlane GameObject found");
		}
		whiteboardController = whiteboard.GetComponent<WhiteboardController> ();
		if (null == whiteboardController) {
			throw new System.ArgumentNullException("No WhiteboardController script attached to WhiteboardPlane GameObject");
		}

		// Get Lecture names
		lectureNames = new string[whiteboardController.lectures.Count];
		int i = 0;
		foreach (Lecture lecture in whiteboardController.lectures) {
			lectureNames[i++] = lecture.Name;
		}
	}
	
	public void Update()
	{
		// Check if [ESC] was pressed
		if (Input.GetKeyDown (KeyCode.Escape))
        {
            StartCoroutine(VRSceneLoader.loadScene("Laboratory"));
        }
		// Check if [<-] or [A] was pressed
		if (Input.GetKeyDown (KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) 
		{
			// Move back in slide show
			if(null != whiteboardController) {
				whiteboardController.Previous();
			}

		}
		// Check if [->] or [D] was pressed
		if (Input.GetKeyDown (KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) 
		{
			// Move forward in slide show
			if(null != whiteboardController) {
				whiteboardController.Next();
			}
		}
		// Check if [TAB] was pressed
		if (Input.GetKeyDown (KeyCode.Tab)) {
			// Show menu to select the lecture of interest
			showMenu = !showMenu;
		}
	}

	public void OnGUI()
	{
		// Show control messages on top left corner
		GUI.Label (new Rect (10f, 10f, 300f, 200f), string.Format("[ESC] - Leave\r\n[TAB] - {0} Lecture Menu\r\n[<-] or [A] - Back\r\n[->] or [D] - Forward", showMenu ? "Hide" : "Show"));
		// Show navigation information on the middle lower screen
		if (null != whiteboardController && 
		    null != whiteboardController.selectedLecture &&
		    null != whiteboardController.selectedLecture.WebContents) {
			GUI.Label (new Rect (Screen.width / 2f - 50f, Screen.height - 50f, 100f, 50f), string.Format ("{0}/{1}", whiteboardController.currentSlideIndex + 1, whiteboardController.selectedLecture.WebContents.Count), textStyle);
		}

		// Show lecture menu when activated
		if (showMenu) {
			// Begin the ScrollView
			scrollViewVector = GUI.BeginScrollView (new Rect (Screen.width / 2f - 225f, 50f, 450f, Screen.height - 125f), scrollViewVector, new Rect (0f, 0f, 430f, 2000f));

			// Put something inside the ScrollView
			GUILayout.BeginArea (new Rect (0, 0, 430f, 2000f));
			GUILayout.Box ("PLEASE SELECT A LECTURE");
			selectedLectureIndex = GUILayout.SelectionGrid (selectedLectureIndex, lectureNames, 1);
			GUILayout.EndArea ();

			// End the ScrollView
			GUI.EndScrollView ();
		}

		if (GUI.changed) {
			Debug.Log("Selected Lecture Index: " + selectedLectureIndex);
			if(null != whiteboardController) {
				showMenu = false;
				whiteboardController.SelectLecture(selectedLectureIndex);
				whiteboardController.Refresh();
			}
		}

	}
}
