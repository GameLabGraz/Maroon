//-----------------------------------------------------------------------------
// Question.cs
//
// Base class for questions
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;
using System.Collections;

namespace TealUnity
{

  public abstract class Question
  {
    /// <summary>
    /// The question title
    /// </summary>
    protected string title;

    /// <summary>
    /// The question text
    /// </summary>
    protected string text;

    /// <summary>
    /// The path to the question image
    /// </summary>
    protected string image_path;

    /// <summary>
    /// Creates a new Question object.
    /// </summary>
    /// <param name="title">The question title</param>
    /// <param name="text">The question text</param>
    public Question(string title, string text)
    {
      this.title = title;
      this.text = text;
      this.image_path = null;
    }

    /// <summary>
    /// Gets the question title.
    /// </summary>
    /// <returns>The question title</returns>
    public string getTitle()
    {
      return this.title;
    }

    /// <summary>
    /// Sets the question title
    /// </summary>
    /// <param name="title">The question title.</param>
    public void setTitle(string title)
    {
      this.title = title;
    }

    /// <summary>
    /// Gets the question text
    /// </summary>
    /// <returns>The question text</returns>
    public string getText()
    {
      return this.text;
    }

    /// <summary>
    /// Sets the question text
    /// </summary>
    /// <param name="text">The question text.</param>
    public void setText(string text)
    {
      this.text = text;
    }

    /// <summary>
    /// Gets the image path
    /// </summary>
    /// <returns>The image path</returns>
    public string getImagePath()
    {
      return this.image_path;
    }

    /// <summary>
    /// Sets the image path
    /// </summary>
    /// <param name="image_path">The path to the image</param>
    public void setImagePath(string image_path)
    {
      this.image_path = image_path;
    }
  }
}
