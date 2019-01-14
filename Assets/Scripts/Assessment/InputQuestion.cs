//-----------------------------------------------------------------------------
// InputQuestion.cs
//
// Generic class for a input question type
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TealUnity;

/// <summary>
/// Generic class for a input question type
/// </summary>
/// <typeparam name="T">answer type like float, string</typeparam>
public class InputQuestion<T> : TealUnity.Question
{
    /// <summary>
    /// The answer of the question
    /// </summary>
    T answer;

    /// <summary>
    /// Creates a new InputQuestion object.
    /// </summary>
    /// <param name="title">The question title</param>
    /// <param name="text">The question text</param>
    public InputQuestion(string title, string text)
        : base(title, text)
    {
    }

    /// <summary>
    /// Sets the question answer.
    /// </summary>
    /// <param name="answer">The question answer</param>
    public void setAnswer(T answer)
    {
        this.answer = answer;
    }

    /// <summary>
    /// Gets the question answer
    /// </summary>
    /// <returns>The question answer</returns>
    public T getAnswer()
    {
        return this.answer;
    }

    /// <summary>
    /// Checks the correctness of the entered answer.
    /// </summary>
    /// <param name="answer">The answer to be checked.</param>
    /// <returns>True if the entered answer is correct, otherwise false.</returns>
    public bool checkCorrectness(T answer)
    {
        return EqualityComparer<T>.Default.Equals(this.answer, answer);
    }
}
