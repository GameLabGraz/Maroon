//-----------------------------------------------------------------------------
// McQuestion.cs
//
// Class for a multiple choice question type
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;
using System.Collections;
using System;

public class MCQuestion : TealUnity.Question
{
    /// <summary>
    /// all answer choices
    /// </summary>
	private AnswerChoice[] answer_choices;

    /// <summary>
    /// Creates a new MCQuestion object.
    /// </summary>
    /// <param name="title">The question title</param>
    /// <param name="text">The question text</param>
    public MCQuestion(string title, string text)
        : base(title, text)
    {
        // exactly 4 answer choices
        answer_choices = new AnswerChoice[4];
    }

    /// <summary>
    /// Gets all answer choices.
    /// </summary>
    /// <returns>The answer choices</returns>
    public AnswerChoice[] getAnswerChoices()
    {
        return this.answer_choices;
    }

    /// <summary>
    /// Sets the answer choices.
    /// </summary>
    /// <param name="answer_choices">The answer choices.
    /// Length should be exactly 4.</param>
    public void setAnsweChoices(AnswerChoice[] answer_choices)
    {
        if (answer_choices.Length == 4)
            this.answer_choices = answer_choices;
    }

    /// <summary>
    /// Sets an answer choice at the given index.
    /// </summary>
    /// <param name="answer_choice">The answer choice</param>
    /// <param name="index">The index where the answer choice  is to be inserted.
    /// The index should be between 0 and 3.</param>
    public void setAnswerChoise(AnswerChoice answer_choice, int index)
    {
        if (index >= 0 && index <= 3)
            this.answer_choices[index] = answer_choice;
    }

    /// <summary>
    /// Checks if all answer choices have a image.
    /// </summary>
    /// <returns>True if all answer choices have a image, otherwise false.</returns>
    public bool checkAllImages()
    {
        bool check = true;
        foreach (AnswerChoice answer_choice in answer_choices)
        {
            if (answer_choice.getImagePath() == null)
                check = false;
        }
        return check;
    }

    /// <summary>
    /// Checks the correctness of the entered answers.
    /// </summary>
    /// <param name="answers">The answers to be checked.</param>
    /// <returns>True if the entered answers are correct, otherwise false.</returns>
    public bool checkCorrectness(AnswerChoice[] answers)
    {
        if (answers.Length != answer_choices.Length)
            throw new Exception("answers.Length != " + answer_choices.Length.ToString());

        for (int i = 0; i < answer_choices.Length; i++)
        {
            if (answers[i].isCorrect() != answer_choices[i].isCorrect())
                return false;
        }
        return true;
    }
}