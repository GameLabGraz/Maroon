using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GuiWorkstation : MonoBehaviour 
{
	QuizController quizController;
#pragma warning disable 414
    bool showQuizSelectionMenu = true;
#pragma warning restore 414
    Vector2 scrollViewVector = Vector2.zero;
	int selectedQuizIndex = 0;
	int selectedAnswerIndex = 0;
	string[] quizNames;
	GUIStyle textStyle;
	
	public void Start () {
		// Define GUI style
		textStyle = new GUIStyle("label");
		textStyle.alignment = TextAnchor.MiddleCenter;

		// Find Workstation GameObject in the scene
		GameObject workstation = GameObject.FindGameObjectWithTag ("Workstation");
		if (null == workstation) {
			throw new System.ArgumentNullException("No Workstation GameObject found");
		}
		quizController = workstation.GetComponent<QuizController> ();
		if (null == quizController) {
			throw new System.ArgumentNullException("No QuizController script attached to Workstation GameObject");
		}

		// Get names of quizzes
		quizNames = new string[quizController.Quizzes.Count];
		int i = 0;
		foreach (Quiz quiz in quizController.Quizzes) {
			quizNames[i++] = quiz.Name;
		}
	}

	public void Update () {
		// Check if [ESC] was pressed
		if (Input.GetKeyDown (KeyCode.Escape))
        {
            StartCoroutine(VRSceneLoader.loadScene("Laboratory"));
        }
	}
    /*
	public void OnGUI()
	{
		// Show control messages on top left corner
		GUI.Label (new Rect (10f, 10f, 300f, 200f), string.Format("[ESC] - Leave"));

		// Show quiz selection menu when activated
		if (showQuizSelectionMenu) {
			ShowQuizSelectionMenu();
		} 
		else {
			// Show selected quiz question
			ShowSelectedQuizQuestion(quizController.GetCurrentQuestion());
		}

		if (GUI.changed) {
			if(showQuizSelectionMenu) {
				Debug.Log("Selected Quiz Index: " + selectedQuizIndex);
				if(null != quizController) {
					showQuizSelectionMenu = false;
					quizController.SelectQuiz(selectedQuizIndex);
					quizController.GetCurrentQuestion();
				}
			}
			else {
				// Handle question answer of selected quiz
				Debug.Log("Selected Answer Index: " + selectedAnswerIndex);
				if(null != quizController) {
					showQuizSelectionMenu = (null == quizController.GetNextQuestion());
				}

			}
		}
	}
    */
	void ShowQuizSelectionMenu()
	{
		// Begin the ScrollView
		scrollViewVector = GUI.BeginScrollView (new Rect (Screen.width / 2f - 225f, 50f, 450f, Screen.height - 280f), scrollViewVector, new Rect (0f, 0f, 430f, 2000f));
		
		// Put something inside the ScrollView
		GUILayout.BeginArea (new Rect (0, 0, 430f, 2000f));
		GUILayout.Box ("PLEASE SELECT A QUIZ");
		selectedQuizIndex = GUILayout.SelectionGrid (selectedQuizIndex, quizNames, 1);
		GUILayout.EndArea ();
		
		// End the ScrollView
		GUI.EndScrollView ();
	}

	void ShowSelectedQuizQuestion(Question question)
	{
		if (null == question)
			return;

		GUILayout.BeginArea (new Rect (Screen.width / 2f - 350f, 200f, 700f, Screen.height - 280f));
		GUILayout.Box (question.Text);
		selectedAnswerIndex = GUILayout.SelectionGrid (selectedAnswerIndex, question.GetAnswerTexts (), 2);
		GUILayout.EndArea ();
	}
}
