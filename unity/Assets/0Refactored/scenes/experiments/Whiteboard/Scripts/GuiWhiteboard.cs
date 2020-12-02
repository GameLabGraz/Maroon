using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GuiWhiteboard : MonoBehaviour 
{
	private WhiteboardController whiteboardController;
	private GUIStyle textStyle;
	private Vector2 scrollViewVector = Vector2.zero;
	private int selectedLectureIndex;
    private bool showMenu = true;

    private string[] LectureNames => whiteboardController.Lectures.Select(lecture => lecture.Name).ToArray();

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
			GUI.Label (new Rect (Screen.width / 2f - 50, Screen.height - 60f, 100f, 50f),
                $"{whiteboardController.CurrentContentIndex + 1}/{whiteboardController.SelectedLecture.Contents.Count}", textStyle);
		}

		// Show lecture menu when activated
		if (showMenu)
		{
			// Begin the ScrollView
			scrollViewVector = GUI.BeginScrollView (
                new Rect (Screen.width / 2f + 320, 50f, 280, Screen.height - 125f), 
                scrollViewVector, 
                new Rect (0f, 0f, 260, 1000));

			// Put something inside the ScrollView
			GUILayout.BeginArea (new Rect (0, 0, 250, 1000f));
			GUILayout.Box ("PLEASE SELECT A LECTURE");
			selectedLectureIndex = GUILayout.SelectionGrid (selectedLectureIndex, LectureNames, 1);
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
