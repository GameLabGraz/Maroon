
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculator : MonoBehaviour {

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

    public Boolean HasFocus { get; private set; }

    private static int visibleDigits = 15;

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
	void Start () {

        HasFocus = false;

        //numberButtons
        var start = (int)KeyCode.Keypad0;
        
        for (int i = start; i <= (int)KeyCode.Keypad9; i++)
        {
            Button btn = AddButton((i - start).ToString(), (KeyCode)i, calcNumber);
            btn.keycodes.Add((KeyCode)(i - start + (int)KeyCode.Alpha0));
        }

        Func<string, bool> act = (string x) => {
            reduce();
            leftNumber = rightNumber;
            if (leftNumber == "" || leftNumber == null)
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

    void Update()
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

        Debug.Log(String.Format("Button '{0}' pressed, method name '{1}'", buttonPressed.name, buttonPressed.action.Method.Name));
        buttonPressed.action.Invoke(buttonPressed.name);
        OnButtonPressed(new CalculatorButtonPressedEvent(buttonPressed.name, buttonPressed.keycodes, toDouble(rightNumber)));

        displayNumber();
    }



    protected double toDouble(string s)
    {
        if (s == "" || s == ".")
            return 0;

        double res;
        if (!double.TryParse(s, out res))
            return 0;

        return res;
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
                    rightNumber = (toDouble(leftNumber) / toDouble(rightNumber)).ToString();
                break;

            case "+":
                rightNumber = (toDouble(leftNumber) + toDouble(rightNumber)).ToString();
                break;

            case "-":
                rightNumber = (toDouble(leftNumber) - toDouble(rightNumber)).ToString();
                break;

            case "*":
                rightNumber = (toDouble(leftNumber) * toDouble(rightNumber)).ToString();
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
        } else
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

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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

        foreach (Button button in buttons.Values)
        {
            foreach(KeyCode code in button.keycodes)
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
        Button btn = new Button();
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
    }
}
