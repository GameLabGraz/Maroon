using System.Collections.Generic;

public class Quiz 
{
	private string name;
	private List<Question> questions;
	private float score;

	public string Name { get{ return this.name; } }
	public List<Question> Questions { get{ return this.questions; } }
	public float Score { get{ return this.score; } }

	public Quiz(string name, List<Question> questions)
	{
		this.name = name;
		this.questions = questions;
		this.score = 0f;
	}
}
