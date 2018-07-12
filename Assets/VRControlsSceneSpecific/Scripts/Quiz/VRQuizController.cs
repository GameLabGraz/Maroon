using UnityEngine;
using System.Collections;

public class VRQuizController : MonoBehaviour
{
    public static VRQuizController Instance = null;
    public QuizController quizController;
    public VRQuizSelector quizSelector;
    public VRQuizDrawer quizDrawer;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("Multiple VRQuizControllers in scene!");
    }

	void Start ()
    {
        openQuizSelection();
	}

    public void openQuiz(Quiz quiz)
    {
        quizController.SelectQuiz(quiz);
        quizSelector.close();
        quizDrawer.open();
    }

    public void openQuizSelection()
    {
        quizSelector.open();
        quizDrawer.close();
    }
}
