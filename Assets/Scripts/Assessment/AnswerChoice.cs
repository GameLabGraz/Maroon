//-----------------------------------------------------------------------------
// AnswerChoice.cs
//
// Class for a answer choice, used for multiple choice questions.
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;
using System.Collections;

/// <summary>
///  Class for a answer choice, used for multiple choice questions.
/// </summary>
public class AnswerChoice
{
    /// <summary>
    /// The answer choice text
    /// </summary>
    private string text;

    /// <summary>
    /// The path to the answer choice image
    /// </summary>
	private string image_path;

    /// <summary>
    /// Indicates if the answer choice is correct or wrong
    /// </summary>
	private bool correct;

    /// <summary>
    /// Creates a new AnswerChoice object.
    /// </summary>
    /// <param name="text">The answer choice text</param>
    /// <param name="correct">Is answer choice correct or wrong?</param>
    public AnswerChoice(string text, bool correct)
    {
        this.text = text;
        this.correct = correct;
        this.image_path = null;
    }

    /// <summary>
    /// Returns the correctness of the answer choice.
    /// </summary>
    /// <returns>True if the answer choice is correct, otherwise false.</returns>
    public bool isCorrect()
    {
        return this.correct;
    }

    /// <summary>
    /// Sets the correctness of the answer choice.
    /// </summary>
    /// <param name="correct">Is answer choice correct or wrong?</param>
    public void setCorrect(bool correct)
    {
        this.correct = correct;
    }

    /// <summary>
    /// Gets the answer choice text.
    /// </summary>
    /// <returns>The answer choice text</returns>
    public string getText()
    {
        return this.text;
    }

    /// <summary>
    /// Sets the answer choice text.
    /// </summary>
    /// <param name="text">The answer choice text</param>
    public void setText(string text)
    {
        this.text = text;
    }

    /// <summary>
    /// Gets the answer choice image path
    /// </summary>
    /// <returns>The path to the answer choice image</returns>
    public string getImagePath()
    {
        return this.image_path;
    }

    /// <summary>
    /// Sets the answer choice image path
    /// </summary>
    /// <param name="image_path">The path to the answer choice image</param>
    public void setImagePath(string image_path)
    {
        this.image_path = image_path;
    }
}
