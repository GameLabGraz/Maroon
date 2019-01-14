using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Evaluation.UnityInterface.EWS;
using Evaluation.UnityInterface;
using System.Linq;
using Localization;

public class TaskEntryManager : MonoBehaviour {

    private GameObject TaskEntryPrefab_text;
    private GameObject TaskEntryPrefab_inp;
    private GameObject TaskEntryPrefab_btn;
    private GameObject TaskEntryPrefab_tgl;
    private GameObject TaskEntryPrefab_dd;

    private GameObject Trash;

    private bool Active;

    private bool Finished;
    
    private RectTransform Canvas;
    private RectTransform Content;
    private RectTransform MainPanel;
    private RectTransform RowPanel;

    private float visibleY;

    private static Color SuccessColor = new Color(0, 0.6f, 0);
    private static Color HintColor = new Color(0, 0, 0);
    private static Color MistakeColor = new Color(1, 1, 0);



    public List<ElementGroup> Elements = new List<ElementGroup>();

    private void Awake()
    {
        Canvas = transform as RectTransform;
        Content = Canvas.Find("Scroll View").Find("Viewport").Find("Content") as RectTransform;
        MainPanel = Content.Find("Panel") as RectTransform;
        Trash = Content.Find("Trash").gameObject;

        TaskEntryPrefab_text = Content.Find("TaskEntryPrefab").Find("Text").gameObject;
        TaskEntryPrefab_inp = Content.Find("TaskEntryPrefab").Find("InputField").gameObject;
        TaskEntryPrefab_btn = Content.Find("TaskEntryPrefab").Find("Button").gameObject;
        TaskEntryPrefab_tgl = Content.Find("TaskEntryPrefab").Find("Toggle").gameObject;
        TaskEntryPrefab_dd = Content.Find("TaskEntryPrefab").Find("Dropdown").gameObject;
        RowPanel = Content.Find("TaskEntryPrefab").Find("Panel") as RectTransform;


        if (TaskEntryPrefab_text == null 
            || TaskEntryPrefab_inp == null 
            || TaskEntryPrefab_btn == null 
            || TaskEntryPrefab_dd == null 
            || TaskEntryPrefab_tgl == null)
            throw new Exception("Task Entry Prefab must be provided to ensure full functionallity of the Assignment Sheet");
    }

    // Use this for initialization
    void Start() {
        updateHeight();
        visibleY = transform.localPosition.y;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    
    private void updateHeight()
    {
        //init the contentHeight or set at least 
        float contentHeight = 0;

        var vlg = MainPanel.GetComponent<VerticalLayoutGroup>();
        MainPanel.GetComponent<ContentSizeFitter>().enabled = false;

        var check = new List<object> ();
        foreach (Text text in GetComponentsInChildren<Text>())
        {
            if (!check.Contains(text))
                if (   text.rectTransform.parent.name.StartsWith("InputField") 
                    || text.rectTransform.parent.name.StartsWith("Button"))
                {
                    Component inp = text.rectTransform.parent.GetComponent<InputField>();
                    if(inp == null)
                        inp = text.rectTransform.parent.GetComponent<Button>();

                    if (!check.Contains(inp))
                    {
                        contentHeight += (inp.transform as RectTransform).sizeDelta.y; 
                        check.Add(inp);
                    }
                }  else
                {
                    check.Add(text);
                    contentHeight += text.preferredHeight + 5; // add some for the border of the Input field
                    if (text.gameObject.name != "Label") // destroys the label, if the resize is done on that too
                        text.rectTransform.sizeDelta = new Vector2(text.rectTransform.rect.width, text.preferredHeight + 5);
                }
        }

        contentHeight += MainPanel.childCount * vlg.padding.top;//these are the paddings per children
        Content.sizeDelta = new Vector2(Content.rect.width, contentHeight);
        Content.offsetMax = new Vector2(0, Content.offsetMax.y);
    }



    public void AddElement(AddElementArguments args)
    {
        args.CheckConsistency();

        var group = new ElementGroup();
        var text = Instantiate(TaskEntryPrefab_text);
        text.transform.SetParent(MainPanel, false);
        group.VisibleText = text.GetComponent<Text>();
        group.VisibleText.text = LanguageManager.Instance.GetString(args.Text);

        foreach (var row in args.Inputs)
        {
            var Panel = Instantiate(RowPanel);
            Panel.SetParent(MainPanel, false);

            foreach (var inp in row)
                if (inp is FeedbackButton)
                {
                    var btn = Instantiate(TaskEntryPrefab_btn);
                    group.Buttons.Add(new InpButton() {
                        GUIButton = btn.GetComponent<Button>(),
                        FBButton = inp as FeedbackButton
                    });

                    btn.transform.Find("Text").GetComponent<Text>().text = LanguageManager.Instance.GetString(inp.Text);
                    btn.GetComponent<Button>().onClick.AddListener(delegate {
                        internalEventHandler(
                            inp,
                            group,
                            inp.DefaultValue,
                            args.SendHandler
                        );
                    });

                    PlaceInputField(btn, Panel, row.Count);
                } else if (inp is FeedbackToggle)
                {
                    var tgl = Instantiate(TaskEntryPrefab_tgl);
                    var guitgl = tgl.GetComponent<Toggle>();
                    group.Toggles.Add(new InpToggle() {
                        GUIToggle = guitgl,
                        FBToggle = inp as FeedbackToggle
                    });

                    bool defaultVal = false;
                    bool.TryParse(inp.DefaultValue, out defaultVal);
                    guitgl.isOn = defaultVal;

                    List<FeedbackInput> tglGroup = args.Inputs.Where(a => a.Where(b => b is FeedbackToggle).FirstOrDefault() != null).SelectMany(i => i).ToList();
                    

                    guitgl.onValueChanged.AddListener(delegate {
                        if ((inp as FeedbackToggle).Group == "")
                            return;

                        if (!group.HandlerActive)
                            return;

                        group.HandlerActive = false;

                        if (guitgl.isOn)
                            foreach (var t in group.Toggles.Where(a => a.FBToggle.Group == (inp as FeedbackToggle).Group))
                                t.GUIToggle.isOn = false;
                        
                        guitgl.isOn = true;
                        group.HandlerActive = true;

                    });


                    tgl.transform.Find("Label").GetComponent<Text>().text = LanguageManager.Instance.GetString(inp.Text);
                    PlaceInputField(tgl, Panel, row.Count);
                } else if(inp is FeedbackDropDown)
                {
                    var dd = Instantiate(TaskEntryPrefab_dd);
                    var dropd = dd.GetComponent<Dropdown>();
                    group.DropDowns.Add(new  InpDropdown() {
                        GUIDropdown = dropd,
                        FBDropdown = inp as FeedbackDropDown
                    });

                    dropd.ClearOptions();
                    dropd.AddOptions((inp as FeedbackDropDown).Values.Select(i => LanguageManager.Instance.GetString(i)).ToList());
                    dropd.value = dropd.options.FindIndex((i) => i.text == inp.DefaultValue);

                    PlaceInputField(dd, Panel, row.Count);
                } else
                {
                    var inpfield = Instantiate(TaskEntryPrefab_inp);
                    var input = inpfield.GetComponent<InputField>();
                    group.InputFields.Add(new InpInputField() {
                         GUIField = input,
                         FBField = inp as FeedbackInputField
                    });

                    input.text = inp.DefaultValue;
                    input.placeholder.GetComponent<Text>().text = LanguageManager.Instance.GetString(inp.Text);

                    PlaceInputField(inpfield, Panel, row.Count);
                }
        }
        
        Elements.Add(group);

        updateHeight();
    }

    private void PlaceInputField(GameObject inp, RectTransform Panel, int nrOfElementsInRow)
    {
        inp.transform.SetParent(Panel, false);
        var input = inp.GetComponent<InputField>();
        var rectTrans = inp.transform as RectTransform;

        rectTrans.sizeDelta = new Vector2(
            (Canvas.Find("Scroll View").transform as RectTransform).sizeDelta.x / nrOfElementsInRow,
            rectTrans.rect.height
        );

        Panel.rect.Set(
            Panel.rect.x,
            Panel.rect.y,
            (Canvas.Find("Scroll View").transform as RectTransform).sizeDelta.x,
            Math.Max(MainPanel.rect.height, (inp.transform as RectTransform).rect.height)
        );
    }

    /// <summary>
    /// Clears all entries from the assignment sheet
    /// </summary>
    public void Clear()
    {
        Trash.SetActive(false);

        foreach (Transform tr in MainPanel)
            tr.SetParent( Trash.transform);

        //some f***ing buttons stay behind and no one knows why...
        foreach (Transform tr in MainPanel)
            tr.SetParent(Trash.transform);

        updateHeight();
    }

    private void SetCorrectRotationAndScale(RectTransform obj)
    {
        obj.localScale = new Vector3(1, 1, 1);
        obj.localRotation = Quaternion.identity;
        obj.localPosition.Set(obj.localPosition.x, obj.localPosition.y, 0);
        obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, 0);
    }

    private static void internalEventHandler(FeedbackInput sender, ElementGroup group, object value, Func<ButtonPressedEvent, ColorCode> handler)
    {
        var ret = new ButtonPressedEvent()
        {
            Sender = sender,
            Value = value,
            ComponentGroup = group,
            SystemTime = DateTime.Now,
            UnityTime = new Time()
        };


        switch (handler(ret))
        {
            case ColorCode.Success:
                group.VisibleText.color = SuccessColor;
                break;
            case ColorCode.Hint:
                group.VisibleText.color = HintColor;
                break;
            default:
                group.VisibleText.color = MistakeColor;
                break;
        }
}




    public class ButtonPressedEvent : EventArgs
    {
        public FeedbackInput Sender;
        public object Value;
        public ElementGroup ComponentGroup;
        public DateTime SystemTime;
        public Time UnityTime;
    }
    public class AddElementArguments
    {
        public string Text { get; set; }
        public List<List<FeedbackInput>> Inputs { get; set; } 
        public Func<ButtonPressedEvent, ColorCode> SendHandler { get; set; }

        public void CheckConsistency()
        {
            Func<object, bool> isEmpty = (object var) => var == null || var.ToString() == "";

            if (isEmpty(Text))
                throw new ArgumentNullException("Text is mandatory");
            for(int r = 0; r < Inputs.Count; r++)
                for(int i = 0; i  < Inputs[r].Count; i++)
                { 
                    if (!isEmpty(Inputs[r][i].VariableName) && isEmpty(Inputs[r][i].VariableType))
                        throw new ArgumentNullException(
                            String.Format(
                                "If a VariableName is provided, VariableType is mandatory (Row {0}, element {1})",
                                r, 
                                i
                            )
                        );

                    if (!isEmpty(Inputs[r][i].VariableType) && isEmpty(SendHandler))
                        throw new ArgumentNullException(
                            String.Format(
                                "If a VariableName and VariableType is given, a SendHandler must be provided (Row {0}, element {1})",
                                r,
                                i
                            )
                        );
                }
        }

        public AddElementArguments()
        {
            Inputs = new List<List<FeedbackInput>>();
        }
    }

    public class ElementGroup
    {
        public Text VisibleText;
        public List<InpButton> Buttons = new List<InpButton>();
        public List<InpInputField> InputFields = new List<InpInputField>();
        public List<InpDropdown> DropDowns = new List<InpDropdown>();
        public List<InpToggle> Toggles = new List<InpToggle>();
        public Boolean HandlerActive = true;


    }

    public class InpButton
    {
        public Button GUIButton;
        public FeedbackButton FBButton;
    }

    public class InpInputField
    {
        public InputField GUIField;
        public FeedbackInputField FBField;
    }
    public class InpDropdown
    {
        public Dropdown GUIDropdown;
        public FeedbackDropDown FBDropdown;
    }

    public class InpToggle
    {
        public Toggle GUIToggle;
        public FeedbackToggle FBToggle;
    }
}
