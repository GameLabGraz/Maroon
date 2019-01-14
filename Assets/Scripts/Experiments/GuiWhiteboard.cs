using UnityEngine;
using System.Collections.Generic;

public class GuiWhiteboard : MonoBehaviour 
{
	private WhiteboardController whiteboardController;
	private GUIStyle textStyle;
	private Vector2 scrollViewVector = Vector2.zero;
	private int selectedLectureIndex = 0;
	private string[] lectureNames;
	private bool showMenu = true;

    // HOTFIX for NullReference to this.whiteboardController.Lectures.Count
    private int lectureCount = 4; //static, only 4 lectures atm
    private List<string> myLectureNames = new List<string>();

    public void Start () 
	{
	    Cursor.lockState = CursorLockMode.None;
	    // Define GUI style
	    textStyle = new GUIStyle("label") {alignment = TextAnchor.MiddleCenter};

	    // Find Whiteboard GameObject in the scene
	    var whiteboard = GameObject.FindGameObjectWithTag("WhiteboardPlane");
	    if (null == whiteboard)
	    {
	        throw new System.ArgumentNullException("No WhiteboardPlane GameObject found");
	    }
	    whiteboardController = whiteboard.GetComponent<WhiteboardController>();
	    if (null == whiteboardController)
	    {
	        throw new System.ArgumentNullException("No WhiteboardController script attached to WhiteboardPlane GameObject");
	    }

        // Get Lecture names
        lectureNames = new string[whiteboardController.Lectures.Count];
	    var i = 0;
	    foreach (var lecture in whiteboardController.Lectures)
	        lectureNames[i++] = lecture.Name;
    }

    public void ShowMenu()
    {
        showMenu = !showMenu;
    }

    public void OnGUI()
	{
		// Show navigation information on the middle lower screen
		if (null != whiteboardController && 
		    null != whiteboardController.SelectedLecture &&
		    null != whiteboardController.SelectedLecture.Contents)
		{
			GUI.Label (new Rect (Screen.width / 2f - 50f, Screen.height - 50f, 100f, 50f), string.Format ("{0}/{1}", whiteboardController.CurrentContentIndex + 1, whiteboardController.SelectedLecture.Contents.Count), textStyle);
		}

		// Show lecture menu when activated
		if (showMenu)
		{
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

		if (GUI.changed)
		{
			Debug.Log("Selected Lecture Index: " + selectedLectureIndex);
			if(null != whiteboardController)
			{
				showMenu = false;
				whiteboardController.SelectLecture(selectedLectureIndex);
				whiteboardController.Refresh();
			}
		}

	}
}
