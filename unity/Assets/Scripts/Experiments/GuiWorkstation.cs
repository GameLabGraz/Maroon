using UnityEngine;

public class GuiWorkstation : MonoBehaviour 
{
	private QuizController quizController;
	private bool showQuizSelectionMenu = true;
	private Vector2 scrollViewVector = Vector2.zero;
	private int selectedQuizIndex = 0;
	private int selectedAnswerIndex = 0;
	private string[] quizNames;
	private GUIStyle textStyle;
	
	public void Start () {
		// Define GUI style
		this.textStyle = new GUIStyle("label");
		this.textStyle.alignment = TextAnchor.MiddleCenter;

		// Find Workstation GameObject in the scene
		GameObject workstation = GameObject.FindGameObjectWithTag ("Workstation");
		if (null == workstation) {
			throw new System.ArgumentNullException("No Workstation GameObject found");
		}
		this.quizController = workstation.GetComponent<QuizController> ();
		if (null == this.quizController) {
			throw new System.ArgumentNullException("No QuizController script attached to Workstation GameObject");
		}

		// Get names of quizzes
		this.quizNames = new string[this.quizController.Quizzes.Count];
		int i = 0;
		foreach (Quiz quiz in this.quizController.Quizzes) {
			this.quizNames[i++] = quiz.Name;
		}
	}

	public void Update () {
		// Check if [ESC] was pressed
		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			Application.LoadLevel("Laboratory");
		}
	}

	public void OnGUI()
	{
		// Show control messages on top left corner
		GUI.Label (new Rect (10f, 10f, 300f, 200f), string.Format("[ESC] - Leave"));

		// Show quiz selection menu when activated
		if (this.showQuizSelectionMenu) {
			this.ShowQuizSelectionMenu();
		} 
		else {
			// Show selected quiz question
			this.ShowSelectedQuizQuestion(this.quizController.GetCurrentQuestion());
		}

		if (GUI.changed) {
			if(this.showQuizSelectionMenu) {
				Debug.Log("Selected Quiz Index: " + this.selectedQuizIndex);
				if(null != this.quizController) {
					this.showQuizSelectionMenu = false;
					this.quizController.SelectQuiz(this.selectedQuizIndex);
					this.quizController.GetCurrentQuestion();
				}
			}
			else {
				// Handle question answer of selected quiz
				Debug.Log("Selected Answer Index: " + this.selectedAnswerIndex);
				if(null != this.quizController) {
					this.showQuizSelectionMenu = (null == this.quizController.GetNextQuestion());
				}

			}
		}
	}

	private void ShowQuizSelectionMenu()
	{
		// Begin the ScrollView
		this.scrollViewVector = GUI.BeginScrollView (new Rect (Screen.width / 2f - 225f, 50f, 450f, Screen.height - 280f), this.scrollViewVector, new Rect (0f, 0f, 430f, 2000f));
		
		// Put something inside the ScrollView
		GUILayout.BeginArea (new Rect (0, 0, 430f, 2000f));
		GUILayout.Box ("PLEASE SELECT A QUIZ");
		this.selectedQuizIndex = GUILayout.SelectionGrid (this.selectedQuizIndex, this.quizNames, 1);
		GUILayout.EndArea ();
		
		// End the ScrollView
		GUI.EndScrollView ();
	}

	private void ShowSelectedQuizQuestion(Question question)
	{
		if (null == question)
			return;

		GUILayout.BeginArea (new Rect (Screen.width / 2f - 350f, 200f, 700f, Screen.height - 280f));
		GUILayout.Box (question.Text);
		this.selectedAnswerIndex = GUILayout.SelectionGrid (this.selectedAnswerIndex, question.GetAnswerTexts (), 2);
		GUILayout.EndArea ();
	}
}
