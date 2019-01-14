using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using Evaluation.UnityInterface;

public class AssessmentManager : MonoBehaviour {

    [SerializeField]
    private string AssessmentUrl;
    [SerializeField]
    private string AssignmentFileName;
    [SerializeField]
    private string FirstSectionName;

    [TextArea]
    public string GlobalAttributes;
    private AssessmentWatchValue GlobalWatchValue;

    private static AssessmentManager instance_;
    private EvaluationService evalService_;
    private List<IAssessmentValue> values_;
    public bool Enabled { get; private set; }

    public delegate void OnEnteredSectionHandler(IterationResult result);
    public static event OnEnteredSectionHandler OnEnteredSection;

    public static AssessmentManager Instance {
        get {
            return instance_;
        }
    }
  
    public void Start()
    {

        values_ = new List<IAssessmentValue>();
        instance_ = this;

        if (GlobalAttributes != "")
        {
            GlobalWatchValue = gameObject.AddComponent<AssessmentWatchValue>();
            GlobalWatchValue.Attributes = GlobalAttributes;
            GlobalWatchValue.Internal = true;
        }


        try
        {
            Debug.Log("Connecting to WebService");
            evalService_ = new EvaluationService(AssessmentUrl, AssignmentFileName);
            Debug.Log("Got ID: " + evalService_.ContextID);
            Enabled = true;
            //var ev = GameEventBuilder.EnterSection(FirstSectionName);
            var ev = new GameEvent().Add(
                new PlayerAction() {
                    Name = "EnterSection",
                    Parameters = new ActionParameter[] {
                             new ActionParameter () {
                                Name = "section_class",
                                Value = FirstSectionName
                            }
                        }
                }
            );

            IterationResult result = Send(ev);
            
            if (OnEnteredSection != null)
                OnEnteredSection(result);

        } catch(Exception e)
        {
            Enabled = false;
            Debug.Log("WARNING! An error happened while connection to the Assessment service: " + e.Message);
            return;
        }


    }

    public void UpdateEnvironment()
    {
        if (!Enabled)
            return;

        Send(GetAllEnvironmentalChanges());
    }

    private GameEvent GetAllEnvironmentalChanges()
    {
        if (values_.Count > 0)
            return values_.Where(val => !val.ContinuousUpdate).Select(val => val.GameEvent).Aggregate((prev, curr) => prev.Add(curr));
        else
            return new GameEvent();
    }

    public IterationResult Send(GameEvent Event, bool UpdateOfEnvironment = true)
    {
        if (!Enabled)
            return new IterationResult(null);

        if (UpdateOfEnvironment)
            Event.Add(GetAllEnvironmentalChanges());
        
        return evalService_.Send(Event);
    }
    

    public IterationResult RegisterValue(IAssessmentValue Value)
    {
        Debug.Log("registering value: " + Value.gameObject.name);
        values_.Add(Value);
        if(evalService_ == null)
        {
            Debug.Log("Warning! Assessmentservice is not active!");
            return new IterationResult(null);
        }
        return evalService_.Send(Value.GameEvent, true);
    }

    public void PrintSummary()
    {
        Debug.Log(evalService_.GetSummary().ToString());
    }

    public void Update()
    {
        if (values_.Count == 0)
            return;

        var evt = values_
            .Where(val => val.ContinuousUpdate)
            .Select(val => val.GameEvent)
            .Aggregate(new GameEvent(), (prev, curr) => prev.Add(curr));

        if (evt.Actions.Length > 0 || evt.EnvironmentChanges.Length > 0)
            evalService_.Send(evt);
    }
}

