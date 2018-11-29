using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuizController : MonoBehaviour 
{
	private List<Quiz> quizzes;
	private Quiz selectedQuiz;
	private int currentQuestionIndex;
	private Object lockObject = new Object();

	public List<Quiz> Quizzes { get{ return this.quizzes; }}
	public Quiz SelectedQuiz { get{ return this.selectedQuiz; } }
	public int CurrentQuestionIndex { get{ return this.currentQuestionIndex; } }

	public void Start () {
		this.currentQuestionIndex = -1;

		// ---- REMOVE WHEN DONE
		// Create some dummy quizzes - TO BE REPLACED with dynamic loading from XML
		this.quizzes = new List<Quiz> ();
		List<Question> questionsQuiz1 = new List<Question> ();
		List<Answer> answersQuiz1Question1 = new List<Answer>();
		answersQuiz1Question1.Add (new Answer ("Positive", true));
		answersQuiz1Question1.Add (new Answer ("Negative", false));
		answersQuiz1Question1.Add (new Answer ("None", false));
		questionsQuiz1.Add(new Question ("What kind of charge does the Van de Graaff Generator accumulate in its sphere?", answersQuiz1Question1, Question.QuestionType.SingleAnswer));
		List<Answer> answersQuiz1Question2 = new List<Answer>();
		answersQuiz1Question2.Add (new Answer ("5V/cm", false));
		answersQuiz1Question2.Add (new Answer ("30kV/cm", true));
		answersQuiz1Question2.Add (new Answer ("10MV/cm", false));
		answersQuiz1Question2.Add (new Answer ("1TV/cm", false));
		questionsQuiz1.Add (new Question ("What is the value of the breakdown voltage of air under standard conditions for temperature and pressure?", answersQuiz1Question2, Question.QuestionType.SingleAnswer));
		this.quizzes.Add(new Quiz ("Van de Graaff Experiment 1 Quiz", questionsQuiz1));

		List<Question> questionsQuiz2 = new List<Question> ();
		List<Answer> answersQuiz2Question1 = new List<Answer>();
		answersQuiz2Question1.Add (new Answer ("Conductive", true));
		answersQuiz2Question1.Add (new Answer ("Non-Conductive", false));
		questionsQuiz2.Add(new Question ("Is the balloon surface conductive or non-conductive?", answersQuiz2Question1, Question.QuestionType.SingleAnswer));
		this.quizzes.Add(new Quiz ("Van de Graaff Experiment 2 Quiz", questionsQuiz2));

		this.selectedQuiz = this.quizzes[0];
		// ---------------------
	}

	public void Update () {
	
	}

	public Quiz SelectQuiz(int quizIndex)
	{
		if (quizIndex < 0 || quizIndex >= this.quizzes.Count) {
			throw new  System.IndexOutOfRangeException(string.Format("Selected Quiz Index {0} out of range", quizIndex));
		}
		this.selectedQuiz = this.quizzes [quizIndex];
		this.currentQuestionIndex = 0;
		return this.selectedQuiz;
	}

	public Question GetCurrentQuestion()
	{
		if (null == this.selectedQuiz.Questions)
			throw new System.NullReferenceException("Questions list in selected quizz is null");
		lock (this.lockObject) {
			if (this.currentQuestionIndex >= 0 && this.selectedQuiz.Questions.Count > this.currentQuestionIndex) {
				return this.selectedQuiz.Questions[this.currentQuestionIndex];
			}
		}
		return null;
	}

	public Question GetNextQuestion()
	{
		if (null == this.selectedQuiz.Questions)
			throw new System.NullReferenceException("Questions list in selected quizz is null");
		lock (this.lockObject) {
			if (this.currentQuestionIndex >= 0 && this.selectedQuiz.Questions.Count > this.currentQuestionIndex + 1) {
				return this.selectedQuiz.Questions[++this.currentQuestionIndex];
			}
		}
		return null;
	}
}
