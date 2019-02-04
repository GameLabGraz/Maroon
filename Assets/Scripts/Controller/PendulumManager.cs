using System;
using System.Globalization;
using Evaluation.UnityInterface;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Controller
{
    /// <summary>
    /// Controller class to manage the field lines
    /// </summary>
    public class PendulumManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _pendulumWeight;
        [SerializeField]
        private GameObject _standRopeJoint;
        [SerializeField]
        public GameObject Stopwatch;
        [SerializeField]
        private GameObject _infoTextPanel;
        [SerializeField]
        private GameObject _slowMoObject;
        [SerializeField]
        private GameObject _angleTextField;
        [SerializeField]
        private Material _ropeMaterial;


        public float RopeLength = 0.3f;
        public float Weight = 1.0f;

        private  bool _mouseDown;
        private Vector3 _mouseStart;
        private GameObject _lastLine;
        private bool _slow = false;
        private Vector3 _defaultPosition;


        [SerializeField]
        private float _weightChangeStepSize = 0.05f;
        [SerializeField]
        private float _weightMin = 1.0f;
        [SerializeField]
        private float _weightMax = 2.0f;

        [SerializeField]
        private float _ropeLengthChangeStepSize = 0.01f;
        [SerializeField]
        private float _ropeMinLength = 0.2f;
        [SerializeField]
        private float _ropeMaxLength = 0.5f;


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

            _defaultPosition = _pendulumWeight.transform.position;

            //This is for initialy setting the rope length in assessment 
            SetRopeLengthRelative(0);
        
        }

        public void Update()
        {

            var joint = _pendulumWeight.GetComponent<HingeJoint>();
            joint.GetComponent<Rigidbody>().WakeUp();

            if (Input.GetMouseButtonUp(0) && _mouseDown)
            {
                Debug.Log("Sending Release action");
                var res = AssessmentManager.Instance.Send(GameEventBuilder.UseObject("operation", "release"));
                GuiPendulum.ShowFeedback(res.Feedback);

                _mouseDown = false;
                joint.useLimits = false;

                //AssessmentManager.Instance.PrintSummary();
            }
            else if (Input.GetMouseButtonDown(0) || _mouseDown)
            {
                if (!_mouseDown)
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 100))
                        if (hit.transform.name == _pendulumWeight.name)
                        {
                            _mouseDown = true;
                            _mouseStart = Input.mousePosition;
                        } else if (hit.transform.name == Stopwatch.name)
                        {
                            Stopwatch.SendMessage(Stopwatch.GetComponent<StopWatch>().isRunning ? "SWStop" : "SWStart");
                        } else if (hit.transform.name == _slowMoObject.name)
                        {
                            if (_slow)
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
                            _slow = !_slow;
                        }
                }

                if (_mouseDown)
                {
                    //relative mouse movement / (scaling factor for easy use) 
                    var angle = ((_mouseStart.x - Input.mousePosition.x) / (RopeLength * 10));
                    //Everything above 140 degrees seems to freak out unity enormously, just don't allow it
                    angle = Math.Min(Math.Max(-140f, angle), 140f);

                    LimitHinge(joint, angle);
                }
            } else if (Input.GetMouseButtonDown(1) )
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                    if (hit.transform.name == Stopwatch.name)
                        Stopwatch.SendMessage("SWReset");
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                SetRopeLengthRelative(_ropeLengthChangeStepSize);

            } else if(Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                SetRopeLengthRelative(-_ropeLengthChangeStepSize);
            }

            CheckKeyboardInput();
            AssertPosition();

            DrawRopeAndAngle();
            AdjustWeight();
            SetText();


        }

        private void SetText()
        {
            _infoTextPanel.GetComponent<TextMesh>().text = Math.Round(Weight, 2).ToString() + " kg\n" + Math.Round(RopeLength * 100) + " cm";
        }

        /// <summary>
        /// Sometimes, the engine freaks out and sets insane values to the position of the weight. If that happens, this function should
        /// return the weight to its default location
        /// </summary>
        private void AssertPosition()
        {
            if (!( Between(-100, _pendulumWeight.transform.position.x, 100))
                && Between(-100, _pendulumWeight.transform.position.y, 100)
                && Between(-100, _pendulumWeight.transform.position.z, 100)
            )
            {
                Debug.Log("Assertion Error: resetting position due to far off values");
                _pendulumWeight.transform.position = _defaultPosition;
            }
            
        }

        /// <summary>
        /// Returns true if lower <= value <= upper 
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="value"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        private bool Between(double lower, double value, double upper)
        {
            return (lower <= value) && (value <= upper);
        }

        private void CheckKeyboardInput()
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
                Weight -= _weightChangeStepSize;
                GuiPendulum.ShowFeedback(
                    AssessmentManager.Instance.Send(
                        GameEventBuilder.UseObject("pendulum_weight", "decrease")
                    ).Feedback
                );

            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Weight += _weightChangeStepSize;
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

        private void AdjustWeight()
        {
            var obj = _pendulumWeight.transform.Find("weight_obj/weight_gizmo");

            Weight = Math.Min(Math.Max(Weight, _weightMin), _weightMax);
            obj.transform.localScale = new Vector3(Weight / 1000, Weight / 1000, Weight / 1000);

            //set the weight at the rigidbody (doesn't change anything physically, but.. you know...)
            _pendulumWeight.GetComponent<Rigidbody>().mass = Weight;

        }
        private void DrawRopeAndAngle()
        {
            var startPos = _pendulumWeight.transform.Find("weight_obj").transform.position;
            startPos.Set(startPos.x, startPos.y, startPos.z);
            DrawLine(startPos, _standRopeJoint.transform.position);

            if (!_angleTextField)
                return;

            var text = _angleTextField.GetComponent<Text>();
            if (!text)
                throw new Exception(string.Format("The given text field ({0}) does not contain a text component", _angleTextField.name));

            var x = transform.localEulerAngles.x;
            x = x < 180 ? x : x - 360;
            text.text = Math.Round(x, 1).ToString(CultureInfo.InvariantCulture) + "°";
        }

        private void SetRopeLengthRelative(float value)
        {
            LimitHinge(_pendulumWeight.GetComponent<HingeJoint>(), 0);
            var currPos = _pendulumWeight.transform.position;
            var obj = _pendulumWeight.transform.Find("weight_obj");
            var pos = obj.transform.position;
            var newVal = Math.Max(-_ropeMaxLength, -RopeLength + value);
            newVal = Math.Min(newVal, -_ropeMinLength);
            RopeLength = -newVal;

            pos.Set(transform.position.x, transform.position.y - RopeLength, transform.position.z);
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


        private void DrawLine(Vector3 start, Vector3 end)
        {
            var ropeLine = new GameObject();
            ropeLine.transform.position = start;
            ropeLine.AddComponent<LineRenderer>();
            var lr = ropeLine.GetComponent<LineRenderer>();
            lr.material = _ropeMaterial;
            lr.startWidth = lr.endWidth = 0.001f;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            GameObject.Destroy(_lastLine);
            _lastLine = ropeLine;
        }

        private void LimitHinge(HingeJoint joint, float angle)
        {
            var jl = new JointLimits {
                min = angle,
                max = angle + 0.0001f // because it bugs out otherwise...
            };
            joint.useLimits = true;
            joint.limits = jl;
        }
    

    }
}
