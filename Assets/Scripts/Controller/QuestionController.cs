//-----------------------------------------------------------------------------
// QuestionController.cs
//
// Handles loading and displaying the questions
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using TealUnity;

/// <summary>
/// Handles loading and displaying the questions
/// </summary>
public class QuestionController : MonoBehaviour
{
    /// <summary>
    /// path to xml questions
    /// </summary>
    public string xmlQuestions;

    /// <summary>
    /// List of the questions
    /// </summary>
    private List<TealUnity.Question> questions;

    /// <summary>
    /// List of the results
    /// </summary>
    private List<bool> results;

    /// <summary>
    /// Index of current question
    /// </summary>
    private int questionIndex = 0;

    /// <summary>
    /// Multiple choice panel text
    /// </summary>
    private GameObject mcPanelText;

    /// <summary>
    /// Multiple choice panel image
    /// </summary>
    private GameObject mcPanelImage;

    /// <summary>
    /// The input panel
    /// </summary>
    private GameObject inputPanel;

    /// <summary>
    /// The title of the question
    /// </summary>
    private Text questionTitle;

    /// <summary>
    /// The text of the question
    /// </summary>
    private Text questionText;

    /// <summary>
    /// The image of the question
    /// </summary>
    private Image questionImage;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
    {
        questions = new List<TealUnity.Question>();
        results = new List<bool>();
        parseXmlDocument();

        mcPanelText = GameObject.Find("MCPanelText");
        mcPanelImage = GameObject.Find("MCPanelImage");
        inputPanel = GameObject.Find("InputPanel");

        disableAnswerPanels();

        questionTitle = GameObject.Find("QuestionTitle").GetComponent<Text>();
        questionText = GameObject.Find("QuestionText").GetComponent<Text>();
        questionImage = GameObject.Find("QuestionImage").GetComponent<Image>();

        loadNextQuestion();
    }

    /// <summary>
    /// Disables all answer panels.
    /// </summary>
    private void disableAnswerPanels()
    {
        mcPanelText.SetActive(false);
        mcPanelImage.SetActive(false);
        inputPanel.SetActive(false);
    }

    /// <summary>
    /// Loads the next question
    /// </summary>
    public void loadNextQuestion()
    {
        // Check previous question for correctness
        if (questionIndex > 0 && questionIndex <= questions.Count)
            results.Add(checkQuestion(questions[questionIndex - 1]));
        //Debug.Log("Question" + (question_index - 1) + ": " + checkQuestion(questions[question_index - 1]));

        // There are no questions, therefore no question panel
        if (questions.Count <= 0)
        {
            GameObject.Find("PanelQuestion").SetActive(false);
            return;
        }

        // Show results
        if (questionIndex == questions.Count)
        {
            disableAnswerPanels();
            questionImage.gameObject.SetActive(false);

            questionTitle.text = "Result";

            questionText.text = "";
            for (int i = 0; i < results.Count; i++)
                questionText.text += "Question " + i.ToString() + ": " + results[i].ToString() + "\n";

            GameObject.Find("BtnNext").GetComponentInChildren<Text>().text = "Close";
            questionIndex++;
            return;
        }

        // Close question panel
        else if (questionIndex > questions.Count)
        {
            GameObject.Find("PanelQuestion").SetActive(false);
            CameraController controller = GameObject.Find("MainCamera").GetComponent<CameraController>();
            controller.setMouseOverPanel(false);
            return;
        }

        // load question title
        questionTitle.text = questions[questionIndex].getTitle();

        // load question text
        questionText.text = questions[questionIndex].getText();

        // load question image 
        questionImage.gameObject.SetActive(false);
        if (questions[questionIndex].getImagePath() != null)
        {
            Sprite image = Resources.Load<Sprite>(questions[questionIndex].getImagePath());
            if (image != null)
            {
                questionImage.gameObject.SetActive(true);
                questionImage.overrideSprite = image;
            }
            else
                Debug.LogError("Sprite not found.");
        }

        // load answer panel
        disableAnswerPanels();
        if (questions[questionIndex].GetType() == typeof(MCQuestion))
        {
            MCQuestion question = (MCQuestion)questions[questionIndex];
            string[] answer_choices = { "AnswerChoice1", "AnswerChoice2", "AnswerChoice3", "AnswerChoice4" };

            if (question.checkAllImages())
            {
                mcPanelImage.SetActive(true);
                for (int i = 0; i < answer_choices.Length; i++)
                {
                    string imagePath = question.getAnswerChoices()[i].getImagePath();
                    Texture2D imageTexture = Resources.Load<Texture2D>(imagePath) as Texture2D;

                    //Sprite image = Resources.Load<Sprite>(question.getAnswerChoices()[i].getImagePath());
                    Sprite image = Sprite.Create(imageTexture,
                        new Rect(0, 0, imageTexture.width, imageTexture.height),
                        new Vector2(0.5f, 0.5f));
                    if (image != null)
                    {
                        GameObject.Find(answer_choices[i]).transform.GetChild(0).GetComponent<Image>().overrideSprite = image;
                        GameObject.Find(answer_choices[i]).transform.GetChild(1).GetComponentInChildren<Text>().text = question.getAnswerChoices()[i].getText();
                    }
                    else
                        Debug.LogError("Image not found.");
                }
            }
            else
            {
                mcPanelText.SetActive(true);
                for (int i = 0; i < answer_choices.Length; i++)
                    GameObject.Find(answer_choices[i]).GetComponentInChildren<Text>().text = question.getAnswerChoices()[i].getText();
            }
        }
        else if (questions[questionIndex].GetType().GetGenericTypeDefinition() == typeof(InputQuestion<>))
        {
            inputPanel.SetActive(true);
            GameObject.Find("AnswerField").GetComponent<InputField>().text = "";
        }
        questionIndex++;
    }

    /// <summary>
    /// Checks question correctness
    /// </summary>
    /// <param name="question">a question object</param>
    /// <returns>true or false depending on the answer</returns>
    private bool checkQuestion(TealUnity.Question question)
    {
        if (question.GetType() == typeof(MCQuestion))
        {
            string[] answer_choices = { "AnswerChoice1", "AnswerChoice2", "AnswerChoice3", "AnswerChoice4" };
            AnswerChoice[] answers = new AnswerChoice[4];
            for (int i = 0; i < answers.Length; i++)
            {
                bool toggle = GameObject.Find(answer_choices[i]).GetComponentInChildren<Toggle>().isOn;
                AnswerChoice answer = new AnswerChoice("Answer", toggle);
                answers[i] = answer;
            }
            return ((MCQuestion)question).checkCorrectness(answers);
        }
        else if (question.GetType() == typeof(InputQuestion<string>))
        {
            string answer = GameObject.Find("AnswerField").GetComponent<InputField>().text;
            return ((InputQuestion<string>)question).checkCorrectness(answer);
        }
        else if (question.GetType() == typeof(InputQuestion<float>))
        {
            try
            {
                float answer = float.Parse(GameObject.Find("AnswerField").GetComponent<InputField>().text);
                return ((InputQuestion<float>)question).checkCorrectness(answer);
            }
            catch (FormatException)
            {
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Parse Xml document and generate questions
    /// </summary>
    private void parseXmlDocument()
    {
        XmlDocument doc = LoadDocumentWithSchemaValidation();
        if (doc == null)
            return;

        XmlNodeList quiz_list = doc.SelectNodes("/quiz/question");
        foreach (XmlNode question_node in quiz_list)
        {
            TealUnity.Question question;

            string type = question_node.Attributes["type"].Value;
            string title = question_node["title"].InnerText;
            string text = question_node["text"].InnerText;

            switch (type)
            {
                case "input-number":
                    {
                        question = new InputQuestion<float>(title, text);
                        float answer = 0;
                        try
                        {
                            answer = float.Parse(question_node["answer"].InnerText);
                        }
                        catch (FormatException e)
                        {
                            Debug.LogError(e.Message);
                        }
                    ((InputQuestion<float>)question).setAnswer(answer);
                        break;
                    }
                case "input-text":
                    {
                        question = new InputQuestion<string>(title, text);
                        string answer = question_node["answer"].InnerText;
                        ((InputQuestion<string>)question).setAnswer(answer);
                        break;
                    }
                case "multiple-choice":
                    {
                        question = new MCQuestion(title, text);
                        XmlNodeList answerchoice_list = question_node.SelectNodes("answerchoice");
                        int i = 0;
                        foreach (XmlNode anserchoice_node in answerchoice_list)
                        {
                            bool correct = bool.Parse(anserchoice_node.Attributes["correct"].Value);
                            string answer_text = anserchoice_node["text"].InnerText;

                            AnswerChoice answerchoice = new AnswerChoice(answer_text, correct);

                            if (anserchoice_node["image"] != null)
                                answerchoice.setImagePath(anserchoice_node["image"].InnerText);
                            ((MCQuestion)question).setAnswerChoise(answerchoice, i);
                            i++;
                        }
                        break;
                    }
                default:
                    continue;
            }

            if (question_node["image"] != null)
                question.setImagePath(question_node["image"].InnerText);

            questions.Add(question);
        }
    }

    /// <summary>
    /// Associate the schema with XML. Then, load the XML and validate it against 
    /// the schema. 
    /// </summary>
    /// <returns></returns>
    private XmlDocument LoadDocumentWithSchemaValidation()
    {
        TextAsset schemaText = Resources.Load("Quiz/questions_schema") as TextAsset;
        System.Text.Encoding encode = System.Text.Encoding.UTF8;
        MemoryStream ms = new MemoryStream(encode.GetBytes(schemaText.text));
        XmlSchema schema = XmlSchema.Read(ms, settings_ValidationEventHandler);

        TextAsset xmlText = Resources.Load(xmlQuestions) as TextAsset;
        XmlDocument doc = new XmlDocument();
        doc.PreserveWhitespace = true;
        doc.LoadXml(xmlText.text);
        doc.Schemas.Add(schema);
        doc.Schemas.Compile();
        doc.Validate(settings_ValidationEventHandler);

        ms.Close();

        return doc;
    }

    /// <summary>
    /// Gets the schema. 
    /// </summary>
    /// <returns>the xml schema</returns>
    private XmlSchema getSchema()
    {
        XmlSchemaSet xs = new XmlSchemaSet();
        XmlSchema schema;
        try
        {
            schema = xs.Add(null, "Assets/Scripts/Quiz/questions_schema.xsd");
        }
        catch (System.IO.FileNotFoundException)
        {
            return null;
        }
        return schema;
    }

    /// <summary>
    /// Event handler that is raised when XML doesn't validate against the schema. 
    /// </summary>
    /// <param name="sender">sender of the event</param>
    /// <param name="e">event arguments</param>
    void settings_ValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs e)
    {
        if (e.Severity == XmlSeverityType.Warning)
        {
            Debug.LogWarning("The following validation warning occurred: " + e.Message);
        }
        else if (e.Severity == XmlSeverityType.Error)
        {
            Debug.LogError("The following critical validation errors occurred: " + e.Message);
        }
    }
}