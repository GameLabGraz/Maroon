
using Evaluation.UnityInterface;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Evaluation.UnityInterface;
using UnityEngine.UI;

/// <summary>
/// Controller class to manage the field lines
/// </summary>
public class PendulumManager : MonoBehaviour
{
    [SerializeField]
    private GameObject PendulumWeight;
    [SerializeField]
    private GameObject StandRopeJoint;
    [SerializeField]
    public GameObject Stopwatch;
    [SerializeField]
    private GameObject InfoTextPanel;
    [SerializeField]
    private GameObject SlowMoObject;
    [SerializeField]
    private GameObject AngleTextField;

    public float ropeLength = 0.3f;
    public float weight = 1.0f;

    private  bool mouseDown;
    private Vector3 mouseStart;
    private GameObject lastLine;
    private Boolean slow = false;
    private Vector3 defaultPosition;


    [SerializeField]
    private float WeightChangeStepSize = 0.05f;
    [SerializeField]
    private float WeightMin = 1.0f;
    [SerializeField]
    private float WeightMax = 2.0f;

    [SerializeField]
    private float RopeLengthChangeStepSize = 0.01f;
    [SerializeField]
    private float RopeMinLength = 0.2f;
    [SerializeField]
    private float RopeMaxLength = 0.5f;


    public void Awake()
    {
        Calculator.OnButtonPressed += CalculatorButtonPressed;
        StopWatch.OnStart += StopWatchStart;
        StopWatch.OnStop += StopWatchStop;
        AssessmentManager.OnEnteredSection += (result) => {
            GuiPendulum.ShowFeedback(result.Feedback);
        };

        AssessmentWatchValue.OnValueRegistered += (result) => {
            GuiPendulum.ShowFeedback(result.Feedback);
        };
    }

    /// <summary>
    /// Initialization
    /// </summary>
    public void Start()
    {

        defaultPosition = PendulumWeight.transform.position;

        //This is for initialy setting the ropelength in assessment 
        setRopeLengthRelative(0);
        
    }

    public void Update()
    {

        HingeJoint joint = PendulumWeight.GetComponent<HingeJoint>();
        joint.GetComponent<Rigidbody>().WakeUp();

        if (Input.GetMouseButtonUp(0) && mouseDown)
        {
            Debug.Log("Sending Release action");
            IterationResult res = AssessmentManager.Instance.Send(GameEventBuilder.UseObject("operation", "release"));
            GuiPendulum.ShowFeedback(res.Feedback);

            mouseDown = false;
            joint.useLimits = false;

            //AssessmentManager.Instance.PrintSummary();
        }
         else if (Input.GetMouseButtonDown(0) || mouseDown)
        {
            if (!mouseDown)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                    if (hit.transform.name == PendulumWeight.name)
                    {
                        mouseDown = true;
                        mouseStart = Input.mousePosition;
                    } else if (hit.transform.name == Stopwatch.name)
                    {
                        if (Stopwatch.GetComponent<StopWatch>().isRunning)
                            Stopwatch.SendMessage("SWStop");
                        else
                            Stopwatch.SendMessage("SWStart");
                    } else if (hit.transform.name == SlowMoObject.name)
                    {
                        if (slow)
                        {
                            Time.timeScale = 1.0f;
                            GuiPendulum.ShowFeedback(
                                AssessmentManager.Instance.Send(
                                    GameEventBuilder.UseObject("timelaps", "deactivate")
                                ).Feedback
                            );
                        }
                        else
                        {
                            Time.timeScale = 0.2f;
                            GuiPendulum.ShowFeedback(
                                AssessmentManager.Instance.Send(
                                    GameEventBuilder.UseObject("timelaps", "activate")
                                ).Feedback
                            );
                        }

                        Time.fixedDeltaTime = 0.02F * Time.timeScale;
                        slow = !slow;
                    }
            }

            if (mouseDown)
            {
                //relative mouse movement / (scaling factor for easy use) 
                var angle = ((mouseStart.x - Input.mousePosition.x) / (ropeLength * 10));
                //Everything above 140 degrees seems to freak out unity enormously, just don't allow it
                angle = Math.Min(Math.Max(-140f, angle), 140f);

                limitHinge(joint, angle);
            }
        } else if (Input.GetMouseButtonDown(1) )
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
                if (hit.transform.name == Stopwatch.name)
                    Stopwatch.SendMessage("SWReset");
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            setRopeLengthRelative(RopeLengthChangeStepSize);

        } else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            setRopeLengthRelative(-RopeLengthChangeStepSize);
        }

        checkKeyboardInput();
        assertPosition();

        drawRopeAndAngle();
        adjustWeight();
        setText();


    }

    private void setText()
    {
        InfoTextPanel.GetComponent<TextMesh>().text = Math.Round(weight, 2).ToString() + " kg\n" + Math.Round(ropeLength * 100) + " cm";
    }

    /// <summary>
    /// Sometimes, the engine freaks out and sets insane values to the position of the weight. If that happens, this function should
    /// return the weight to its default location
    /// </summary>
    private void assertPosition()
    {
        if (!( between(-100, PendulumWeight.transform.position.x, 100))
            && between(-100, PendulumWeight.transform.position.y, 100)
            && between(-100, PendulumWeight.transform.position.z, 100)
            )
        {
            Debug.Log("Assertion Error: resetting position due to far off values");
            PendulumWeight.transform.position = defaultPosition;
        }
            
    }

    /// <summary>
    /// Returns true if lower <= value <= upper 
    /// </summary>
    /// <param name="lower"></param>
    /// <param name="value"></param>
    /// <param name="upper"></param>
    /// <returns></returns>
    private Boolean between(double lower, double value, double upper)
    {
        return (lower <= value) && (value <= upper);
     }

    private void checkKeyboardInput()
    {
        if (!Input.anyKeyDown)
            return;

        if (GuiPendulum.isFocused())
            return;

        if (Input.GetKeyDown(KeyCode.W))
        {

        }
        if (Input.GetKeyDown(KeyCode.S))
        {

        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            weight -= WeightChangeStepSize;
            GuiPendulum.ShowFeedback(
                AssessmentManager.Instance.Send(
                    GameEventBuilder.UseObject("pendulum_weight", "decrease")
                ).Feedback
            );

        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            weight += WeightChangeStepSize;
            GuiPendulum.ShowFeedback(
                AssessmentManager.Instance.Send(
                    GameEventBuilder.UseObject("pendulum_weight", "increase")
                ).Feedback
            );
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Stopwatch.SendMessage("SWStart");
        } 
        if (Input.GetKeyDown(KeyCode.E))
        {
            Stopwatch.SendMessage("SWStop");
        }

        if (Input.GetKeyDown(KeyCode.Space))
            ExitExperiment();


    }

    public static void ExitExperiment()
    {
        SceneManager.LoadScene("Laboratory");
    }

    private void CalculatorButtonPressed(Calculator.CalculatorButtonPressedEvent evt)
    {
        if (evt.Name != "enter")
            return;

        var ge = GameEventBuilder
            .EnvironmentVariable(name, "calculated_value", evt.CurrentNumber)
            .Add(GameEventBuilder.UseObject("operation", "submit-value"));

        var res = AssessmentManager.Instance.Send(ge);
        GuiPendulum.ShowFeedback(res.Feedback);
    }

    private void StopWatchStart(StopWatch.StopWatchEvent evt)
    {
        var res = AssessmentManager.Instance.Send(GameEventBuilder.UseObject("operation", "sw-start"));
        GuiPendulum.ShowFeedback(res.Feedback);
    }
    private void StopWatchStop(StopWatch.StopWatchEvent evt)
    {
        var res = AssessmentManager.Instance.Send(GameEventBuilder.UseObject("operation", "sw-stop"));
        GuiPendulum.ShowFeedback(res.Feedback);
    }

    private void adjustWeight()
    {
        var obj = PendulumWeight.transform.Find("weight_obj/weight_gizmo");

        weight = Math.Min(Math.Max(weight, WeightMin), WeightMax);
        obj.transform.localScale = new Vector3(weight / 1000, weight / 1000, weight / 1000);

        //set the weight at the rigidbody (doesn't change anything physically, but.. you know...)
        PendulumWeight.GetComponent<Rigidbody>().mass = weight;

    }
    private void drawRopeAndAngle()
    {
        var startPos = PendulumWeight.transform.Find("weight_obj").transform.position;
        startPos.Set(startPos.x, startPos.y, startPos.z);
        DrawLine(startPos, StandRopeJoint.transform.position, new Color(0, 0, 0));

        if (!AngleTextField)
            return;

        var text = AngleTextField.GetComponent<Text>();
        if (!text)
            throw new Exception(String.Format("The given text field ({0}) does not contain a text component", AngleTextField.name));

        var x = transform.localEulerAngles.x;
        x = x < 180 ? x : x - 360;
        text.text = Math.Round(x, 1).ToString() + "°";
    }

    void setRopeLengthRelative(float value)
    {
        limitHinge(PendulumWeight.GetComponent<HingeJoint>(), 0);
        Vector3 currPos = PendulumWeight.transform.position;
        var obj = PendulumWeight.transform.Find("weight_obj");
        var pos = obj.transform.position;
        float newVal = Math.Max(-RopeMaxLength, -ropeLength + value);
        newVal = Math.Min(newVal, -RopeMinLength);
        ropeLength = -newVal;

        pos.Set(transform.position.x, transform.position.y - ropeLength, transform.position.z);
        obj.transform.position = pos;

        /*
        double theoretical_freq = 1 / (2 * Math.PI) * Math.Sqrt(Physics.gravity.magnitude / ropeLength);
        var ec = GameEventBuilder.EnvironmentVariable(
            this.name,
            "theoretical_frequency",
            theoretical_freq
        ).Add(
            GameEventBuilder.EnvironmentVariable(
                this.name,
                "theoretical_period",
                1 / theoretical_freq
            )
        );
        */
        GuiPendulum.ShowFeedback(
            AssessmentManager.Instance.Send(
                GameEventBuilder.UseObject("operation", "ropelength_change")
            ).Feedback
        );
        
    }

    

    void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = lr.endColor = color;
        lr.startWidth = lr.endWidth = 0.001f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(lastLine);
        lastLine = myLine;
    }

    void limitHinge(HingeJoint joint, float angle)
    {
        JointLimits jl = new JointLimits {
            min = angle,
            max = angle + 0.0001f // because it bugs out otherwise...
        };
        joint.useLimits = true;
        joint.limits = jl;
    }
    

}
