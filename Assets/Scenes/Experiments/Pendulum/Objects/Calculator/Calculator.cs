
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Calculator : MonoBehaviour
{

    public class CalculatorButtonPressedEvent : EventArgs
    {
        public string Name { get; private set; }
        public List<KeyCode> Keycodes { get; private set; }
        public DateTime SystemTime { get; private set; }
        public Time GameTime { get; private set; }
        public double CurrentNumber { get; private set; }

        public CalculatorButtonPressedEvent(string  name, List<KeyCode> keycodes, double currentNumber)
        {
            Name = name;
            Keycodes = keycodes;
            SystemTime = DateTime.Now;
            GameTime = new Time();
            CurrentNumber = currentNumber;
        }
    }

    public delegate void ButtonPressed(CalculatorButtonPressedEvent ev);
    public static event ButtonPressed OnButtonPressed;

    public bool HasFocus { get; private set; }

    private static readonly int visibleDigits = 15;

    private Dictionary<string, Button> buttons = new Dictionary<string, Button>();
    private string leftNumber;
    private string rightNumber;
    private string op;
    private bool dot = false;
    private bool error = false;
    private bool clear = false;

    private Transform mainDisplay;
    private Transform sideDisplay;

    private Dictionary<string, Func<string, bool>> actions = new Dictionary<string, Func<string, bool>>();

	// Use this for initialization
	private void Start ()
    {

        HasFocus = false;

        //numberButtons
        const int start = (int)KeyCode.Keypad0;
        
        for (var i = start; i <= (int)KeyCode.Keypad9; i++)
        {
            var btn = AddButton((i - start).ToString(), (KeyCode)i, calcNumber);
            btn.keycodes.Add((KeyCode)(i - start + (int)KeyCode.Alpha0));
        }

        Func<string, bool> act = (string x) => {
            reduce();
            leftNumber = rightNumber;
            if (string.IsNullOrEmpty(leftNumber))
                leftNumber = "0";
            rightNumber = "";
            op = x;


            return true;
        };

        AddButton("/", KeyCode.KeypadDivide, act);
        AddButton("*", KeyCode.KeypadMultiply, act);
        AddButton("+", KeyCode.KeypadPlus, act);
        AddButton("-", KeyCode.KeypadMinus, act);

        var btnPeriod = AddButton(".", KeyCode.KeypadPeriod, calcNumber);
        btnPeriod.keycodes.Add(KeyCode.Period);
        btnPeriod.keycodes.Add(KeyCode.Comma);

        var btnEnter = AddButton("enter", KeyCode.KeypadEnter, ((string x) => {
            reduce();

            clear = true;
            displayNumber();
            return true;
        }));
        btnEnter.keycodes.Add(KeyCode.Return);

        AddButton("clear", KeyCode.Backspace, (string x) => { leftNumber = ""; rightNumber = ""; op = ""; error = clear = dot = false; return true; });

        AddButton("sin", KeyCode.S, (string x) => { rightNumber = Math.Sin(toDouble(rightNumber)).ToString(); return true; });
        AddButton("cos", KeyCode.C, (string x) => { rightNumber = Math.Cos(toDouble(rightNumber)).ToString(); return true; });


        foreach (Transform child in GetComponentInChildren<Transform>())
            if (child.name.ToLower() == "maindisplay")
                mainDisplay = child;
            else if (child.name.ToLower() == "sidedisplay")
                sideDisplay = child;
        
        if (mainDisplay == null)
            throw new Exception("Calculator: Internal Error. Could not find an object named 'Display'");
	}

    private void Update()
    {

        Button buttonPressed;
        if (!(getButtonFromKeyboard(out buttonPressed)
            || getButtonfromMouse(out buttonPressed)))
            return;

        if(clear)
        {
            clear = false;
            buttons["clear"].action.Invoke("clear");
        }

        Debug.Log($"Button '{buttonPressed.name}' pressed, method name '{buttonPressed.action.Method.Name}'");
        buttonPressed.action.Invoke(buttonPressed.name);
        OnButtonPressed?.Invoke(new CalculatorButtonPressedEvent(buttonPressed.name, buttonPressed.keycodes, toDouble(rightNumber)));

        displayNumber();
    }



    protected double toDouble(string s)
    {
        if (s == "" || s == ".")
            return 0;

        try
        {
            return double.Parse(s, NumberFormatInfo.InvariantInfo);
        }
        catch
        {
            return 0;
        }
    }

    protected void reduce()
    {
        switch (op)
        {
            case "/":
                if (rightNumber == "0" || rightNumber == "")
                {
                    rightNumber = "";
                    leftNumber = "";
                    error = true;
                    op = "";
                }
                else
                    rightNumber = (toDouble(leftNumber) / toDouble(rightNumber)).ToString(CultureInfo.InvariantCulture);
                break;

            case "+":
                rightNumber = (toDouble(leftNumber) + toDouble(rightNumber)).ToString(CultureInfo.InvariantCulture);
                break;

            case "-":
                rightNumber = (toDouble(leftNumber) - toDouble(rightNumber)).ToString(CultureInfo.InvariantCulture);
                break;

            case "*":
                rightNumber = (toDouble(leftNumber) * toDouble(rightNumber)).ToString(CultureInfo.InvariantCulture);
                break;                
        }

        op = "";
        dot = false;

    }

    protected bool calcNumber(string x)
    {
        if (x == "." || x == ",")
        {
            if (dot)
                return false;
            else
                dot = true;

            rightNumber += ".";
            return true;
        }
        
        rightNumber += x;
        return true;
    }

    protected void displayNumber()
    {
        if (error)
        {
            mainDisplay.GetComponent<TextMesh>().text = "Error: div/0";
            sideDisplay.GetComponent<TextMesh>().text = "";
        }
        else
        {
            if (rightNumber == "")
                mainDisplay.GetComponent<TextMesh>().text = "0.";
            else if (rightNumber.Contains("."))
            {
                mainDisplay.GetComponent<TextMesh>().text = rightNumber.Substring(0, Math.Min(visibleDigits,rightNumber.Length));
            } else
            {
                mainDisplay.GetComponent<TextMesh>().text = rightNumber.Substring(Math.Max(0, rightNumber.Length - visibleDigits), Math.Min(rightNumber.Length, 15)) + ".";
            }

            if (op != "")
                sideDisplay.GetComponent<TextMesh>().text = leftNumber + " " + op;
            else
                sideDisplay.GetComponent<TextMesh>().text = "";
        }
    }

    // Update is called once per frame
 
    protected bool getButtonfromMouse(out Button btn)
    {
        btn = null;
        if (!Input.GetMouseButtonDown(0))
            return false;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            if (buttons.ContainsKey(hit.transform.name))
            {
                HasFocus = true;
                btn = buttons[hit.transform.name];
                return true;
            } else if (hit.transform.parent != null && hit.transform.parent.name == this.name)
                HasFocus = true;
            else
                HasFocus = false;
        } else
            HasFocus = false;

        return false;
    }

    protected bool getButtonFromKeyboard(out Button btn)
    {
        btn = null;

        if (!HasFocus)
            return false;

        foreach (var button in buttons.Values)
        {
            foreach(var code in button.keycodes)
                if (Input.GetKeyDown(code))
                { 
                    btn = button;
                    return true;
                }                
        }
        return false;
    }

    private Button AddButton(string name, KeyCode code, Func<string, bool> action)
    {
        var btn = new Button();
        btn.name = name;
        btn.keycodes.Add(code);
        btn.action = action;
        buttons.Add(name, btn);
        return btn;
    }

    protected class Button
    {
        public string name;
        public List<KeyCode> keycodes = new List<KeyCode>();
        public Func<string, bool> action;

        public override string ToString()
        {
            var kc = string.Join(", ", keycodes);
            return $"{name} - {kc}";
        }
    }
}
