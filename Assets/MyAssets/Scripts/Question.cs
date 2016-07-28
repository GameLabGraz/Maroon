using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Answer
{
	private string text;
	private bool correct;

	public string Text { get{ return this.text; } }
	public bool Correct { get{ return this.correct; } }

	public Answer(string text, bool correct)
	{
		this.text = text;
		this.correct = correct;
	}

	public override string ToString ()
	{
		return this.text;
	}
}

public class Question 
{
	public enum QuestionType { MultipleAnswers, SingleAnswer }

	private string text;
	private List<Answer> answers;
	private QuestionType type;

	public string Text { get{ return this.text; } }
	public List<Answer> Answers { get{ return this.answers; }}
	public QuestionType Type { get{ return this.type; } }
	
	public Question(string text, List<Answer> answers, QuestionType type)
	{
		this.text = text;
		this.answers = answers;
		this.type = type;
	}

	public string[] GetAnswerTexts()
	{
		string[] answerTexts = new string[this.answers.Count];
		int i = 0;
		foreach (Answer answer in this.answers) {
			answerTexts[i++] = answer.ToString();
		}
		return answerTexts;
	}
}
