using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Evaluation.UnityInterface;
using Evaluation.UnityInterface.EWS;
using Localization;

public class GuiPendulum : MonoBehaviour {

    public float MinimumTimeTextIsVisible = 3f;
    public float SecondsPerWord = 1f;
    private float CurrentMaxVisibleTime;

    [SerializeField]
    private Text InfoText = null;

    [SerializeField]
    private GameObject AssignmentSheet = null;

    private TaskEntryManager TEM = null;

    private static GuiPendulum Instance = null;

    private DateTime lastSet;

    private List<Evaluation.UnityInterface.Feedback> textToSet = new List<Evaluation.UnityInterface.Feedback>();

    private void Awake()
    {
        Instance = this;
        TEM = AssignmentSheet.GetComponent<TaskEntryManager>();

        lastSet = DateTime.Now;

        if (AssignmentSheet == null || TEM == null)
            throw new InvalidProgramException("The given Assignment Sheet is not valid. It must contain a TaskEntryManager component.");

        InfoText.transform.parent.Find("Button").GetComponent<Button>().onClick.AddListener(delegate {
            Instance.defaultText();
        });
    }

    void Start ()
    {
        defaultText();
        lastSet = DateTime.Now;
    }
    private void Update()
    {
        if (textToSet.Count > 0)
        {
            if (lastSet.AddSeconds(MinimumTimeTextIsVisible) < DateTime.Now)
                lock (textToSet)
                {
                    Color clr = new Color(1,1,1);
                    switch(textToSet[0].Item.Code) {
                        case ColorCode.Success:
                            clr = new Color(0, 0.6f, 0);
                            break;
                        case ColorCode.Mistake:
                            clr = new Color(1, 1, 0);
                            break;
                    }

                    CurrentMaxVisibleTime = Math.Max(customText(textToSet[0].Item.Text, clr).Split(' ').Count() * SecondsPerWord, MinimumTimeTextIsVisible);
                    lastSet = DateTime.Now;
                    textToSet.RemoveAt(0);
                }
        } else
            if (lastSet.AddSeconds(CurrentMaxVisibleTime) < DateTime.Now)
            defaultText();
    }
    private void defaultText()
    {
        InfoText.gameObject.transform.parent.gameObject.SetActive(false);
    }

    private string  customText(string text, Color color)
    {
        InfoText.gameObject.transform.parent.gameObject.SetActive(true);

        //InfoText.text = text;
        //Multilang:
        InfoText.text = text.Contains(";") ?
            text.Split(';').Aggregate((prev, curr) => LanguageManager.Instance.GetString(prev.Trim()) + " " + LanguageManager.Instance.GetString(curr.Trim())) :
            LanguageManager.Instance.GetString(text);

        InfoText.color = color;
        return InfoText.text;
    }

    public static void ShowFeedback(FeedbackEntry[] feedback)
    {
        if (feedback.Length == 0)
            return;
        
        
        foreach (var fb in feedback)
        {
            if (fb.Type == FeedbackType.Technical)
            {
                if (fb.Text.ToLower() == "clear")
                {
                    Clear();
                    Clear(); //dunno why we have to do that... Unity...
                }
                else if (fb.Text.ToLower().StartsWith("entersection"))
                    ShowFeedback(Send(
                        GameEventBuilder.EnterSection(fb.Text.Split(':')[1])
                    ));
                else if (fb.Text.ToLower() == "cleartext")
                    Instance.defaultText();
            } else 
                if(fb.IsQuestion)
                {
                    string text = fb.Text.Contains(";") ?
                    fb.Text.Split(';').Aggregate((prev, curr) => LanguageManager.Instance.GetString(prev.Trim()) + " " + LanguageManager.Instance.GetString(curr.Trim())) :
                    LanguageManager.Instance.GetString(fb.Text);

                    var args = new TaskEntryManager.AddElementArguments() {
                        Inputs = fb.Inputs,
                        Text = text,
                        SendHandler = ButtonSendPressed,
                    };
                
                    Instance.TEM.AddElement(args);
                } else
                {
                    ShowText(fb);
                }


        }
    }



    public static void ShowText(Evaluation.UnityInterface.Feedback feedback)
    {
        if (feedback.Item.Text.Length > 0 && Instance != null)
            lock (Instance.textToSet)
            {
                Instance.textToSet.Add(feedback);
            }
    }


    public static void ShowText(FeedbackEntry feedbackEntry)
    {
        var fb = new Evaluation.UnityInterface.Feedback {
            Item = new TextFeedbackContent {
                Text = feedbackEntry.Text,
                Code = feedbackEntry.ColorCode
            }
        };
        ShowText(fb);
    }

    public static void ShowText(string[] text)
    {
        var fb = new Evaluation.UnityInterface.Feedback {
            Item = new TextFeedbackContent {
                Text = String.Join(", ", text).Trim(),
                Code = ColorCode.Hint
            }
        };
        ShowText(fb);
    }

    private static void ButtonBoolPressed(string VariableName, bool value)
    {
        AssessmentManager.Instance.Send(
            GameEventBuilder.EnvironmentVariable("assessment", VariableName, value)
        );
    }

    public static bool isFocused()
    {
        try
        {
            return Instance.TEM.Elements.Where(e => e.InputFields.Count > 0 && e.InputFields.Where(i => i.GUIField.isFocused).FirstOrDefault() != null).FirstOrDefault() != null;
        }catch
        {
            return false;
        }
    }
    
    public static void Clear()
    {
        Instance.defaultText();
        Instance.TEM.Clear();
    }

    private static ColorCode ButtonSendPressed(TaskEntryManager.ButtonPressedEvent evt)
    {
        var msg = GameEventBuilder.AnswerQuestion(evt.Sender.VariableName, GetRealValue(evt.Sender.Text, evt.Sender.VariableType));
        foreach(var inp in evt.ComponentGroup.InputFields)
        {
            var val = GetRealValue(inp.GUIField.text, inp.FBField.VariableType);
            msg.Add(
                GameEventBuilder.AnswerQuestion(inp.FBField.VariableName, val)
            );
        }

        foreach (var inp in evt.ComponentGroup.DropDowns)
        {
            var val = GetRealValue(inp.GUIDropdown.options[inp.GUIDropdown.value].text, inp.FBDropdown.VariableType);
            msg.Add(
                GameEventBuilder.AnswerQuestion(inp.FBDropdown.VariableName, val)
            );
        }

        foreach (var inp in evt.ComponentGroup.Toggles)
        {
            var val = GetRealValue(inp.GUIToggle.isOn.ToString(), inp.FBToggle.VariableType);
            msg.Add(
                GameEventBuilder.AnswerQuestion(inp.FBToggle.VariableName, val)
            );
        }

        var fb = Send(msg);
        ShowFeedback(fb);

        return fb.Any(e => e.ColorCode == ColorCode.Mistake) ? ColorCode.Mistake :
            fb.Any(e => e.ColorCode == ColorCode.Hint) ? ColorCode.Hint : ColorCode.Success;
    }

    private static object GetRealValue(string value, DataType type )
    {
        object ret = value;

        switch (type)
        {
            case DataType.Float:
                double dbl;
                if (!double.TryParse(value, out dbl))
                    Debug.Log(string.Format("Could not parse {0} to double, falling back to string", value.ToString()));
                else
                    ret = dbl;
                break;

            case DataType.Integer:
                int i;
                if (!int.TryParse(value, out i))
                    Debug.Log(string.Format("Could not parse {0} to integer, falling back to string", value.ToString()));
                else
                    ret = i;

                break;

            case DataType.Boolean:
                bool res;
                if (!Boolean.TryParse(value, out res))
                    Debug.Log(string.Format("Could not parse {0} to boolean, falling back to string", value.ToString()));
                else
                    ret = res;
                break;
        }

        return ret;
    }

    private static FeedbackEntry[] Send<T>(string name, T value)
    {
        return Send(GameEventBuilder.AnswerQuestion(name, value));   
    }

    private static FeedbackEntry[] Send(Evaluation.UnityInterface.GameEvent Message)
    {
        return AssessmentManager.Instance.Send(Message).Feedback;        
    }

}
