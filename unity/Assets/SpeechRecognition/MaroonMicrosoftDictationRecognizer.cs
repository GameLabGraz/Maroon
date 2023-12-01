using UnityEngine;
using UnityEngine.Windows.Speech;
using Maroon.GlobalEntities;
using GameLabGraz.VRInteraction;
using System;
using GEAR.Localization;
using ChatGPTWrapper;
using System.Collections.Generic;
using ACTA;
using System.IO;

namespace Valve.VR.InteractionSystem
{
    public class MaroonMicrosoftDictationRecognizer : MonoBehaviour
    {
        public TextTips textTips;
        public Narrator narrator;
        private DictationRecognizer dictationRecognizer;
        public ChatGPTConversation commandConversation;
        public ChatGPTConversation tutorConversation;
        public ParameterChangerHelper parameterChangerHelper;
        public AzureTTS azureTTS;


        public enum TtsOption
        {
            OnDevice, Azure
        }
        public TtsOption chosenTTS = TtsOption.Azure;

        enum menuButton
        {
            Left, Right
        }
        private menuButton lastButtonPressed = menuButton.Left;


        [SerializeField] private SystemLanguage chosenSpeechLanguage = SystemLanguage.English;

        public Maroon.CustomSceneAsset[] experimentScenes;

        [TextArea(10, 15)]
        public string[] roomSpecificPrompts;


        //private string displayString, listeningString;
        AudioSource[] audioSources;
        bool alreadySetupLanguage = false;

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            audioSources = GetComponents<AudioSource>();
        }

        void Start()
        {
            parameterChangerHelper = gameObject.GetComponent<ParameterChangerHelper>();
            textTips = GameObject.Find("TextTipsObject").GetComponent<TextTips>();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += HandleRoomChange;
            SystemLanguage systemLang = Application.systemLanguage;


            //displayString = "";
            //listeningString = "";
            dictationRecognizer = new DictationRecognizer();

            dictationRecognizer.DictationResult += (text, confidence) =>
            {
                //textTips.FadeOut();
                if (dictationRecognizer.Status.Equals(SpeechSystemStatus.Running))
                    dictationRecognizer.Stop();

                Debug.Log(Time.time.ToString());
                Debug.LogFormat("Dictation result: <{0}> with <{1}>% confidence", text, confidence.ToString());
                if (lastButtonPressed == menuButton.Left && commandConversation != null)
                {
                    commandConversation.SendToChatGPT(text);
                }
                if (lastButtonPressed == menuButton.Right && tutorConversation != null)
                {
                    tutorConversation.SendToChatGPT(text);
                }
            };

            dictationRecognizer.DictationError += (error, hresult) =>
            {
                Debug.Log(Time.time.ToString());
                Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
            };

            //dictationRecognizer.Start();
            textTips.DisplayTip("Welcome to Maroon!\nPress the left menu button to issue a command, or the right menu button if you need help.");
            Debug.Log(Time.time.ToString());
            Debug.Log("begin experiment");
        }


        public void HandleChatGPTResponse(string s)
        {
            Debug.Log(Time.time.ToString());
            Debug.Log("chatgpt said " + s);
            //textTips.DisplayTipWithFade(s, 2f);

            List<StructuredCommand> commands = ParseTheString(s);

            if (commands != null && commands.Count > 0)
            {                
                audioSources[1].Play();  // play the recognize tone

                for (int i = 0; i < commands.Count; i++)
                {
                    StructuredCommand command = commands[i];
                    //Debug.Log(Time.time.ToString());
                    //Debug.Log(command.ToString());

                    if (command.Tag.Equals("command"))
                    {
                        if (command.Value.Equals("changeRoom"))
                        {
                            StructuredCommand nextCommand = commands[i + 1];
                            Debug.Log(Time.time.ToString());
                            Debug.Log("let's change the room, to " + nextCommand.Value);
                            //textTips.DisplayTip("let's change the room,\n to " + nextCommand.Value);  

                            if (nextCommand.Value.Equals("Falling Coil"))
                            {
                                speak(nextCommand.Value);
                                RequestRoomChange("FallingCoil.vr");
                            }
                            else if (nextCommand.Value.Equals("Faraday's Law"))
                            {
                                speak(nextCommand.Value);
                                RequestRoomChange("FaradaysLaw.vr");
                            }
                            else if (nextCommand.Value.Equals("Huygen's Principle"))
                            {
                                speak(nextCommand.Value);
                                RequestRoomChange("HuygensPrinciple.vr");
                            }
                            else if (nextCommand.Value.Equals("Lobby"))
                            {
                                speak(nextCommand.Value);
                                RequestRoomChange("MainMenu.vr");
                            }
                            else if (nextCommand.Value.Equals("Van de Graaf Generator"))
                            {
                                speak(nextCommand.Value);
                                RequestRoomChange("VandeGraaffGenerator.vr");
                            }
                        }
                        else if (command.Value.Equals("modifyVariable"))
                        {
                            StructuredCommand attributeToChange = commands[i + 1];
                            StructuredCommand unit = commands[i + 2];
                            StructuredCommand newValue = commands[i + 3];
                            //Debug.Log("i need to change " + attributeToChange.Value + " to the new value " + newValue.Value + " " + unit.Value);                                                        
                            string theString = attributeToChange.Value + ": " + newValue.Value;
                            if (unit.Value.Equals("percent"))
                            {
                                theString += " " + unit.Value;
                            }
                            textTips.DisplayTipWithFade(theString, 1.5f);

                            if (attributeToChange.Equals("ShowChargeButton"))
                            {
                                VRHoverButton theButton = parameterChangerHelper.FindParticularInactiveButtonInChildren("ButtonFront_ShowCharge", "ShowChargeButton");
                                if (theButton != null)
                                {
                                    if (newValue.Value.Contains("hide"))
                                        theButton.ForceButtonState(false);
                                    else if (newValue.Value.Contains("show"))
                                        theButton.ForceButtonState(true);
                                }
                            }

                            else if (attributeToChange.Value.Equals("iron filings"))
                            {
                                GameObject theParent = GameObject.Find("Experiment");
                                if (theParent != null)
                                {
                                    scrIronFilings theScript = theParent.GetComponentInChildren<scrIronFilings>(true);

                                    if (newValue.Value.Contains("off"))
                                        theScript.hideFieldImage();
                                    else
                                        theScript.generateFieldImage();
                                }
                                else
                                    Debug.Log("Speech: the script PARENT was null");
                            }
                            
                            else if (attributeToChange.Value.Equals("propagation mode"))
                                parameterChangerHelper.SetLinearDriveValue("PropagationModeHandle", unit.Value, float.Parse(newValue.Value));
                            else if (attributeToChange.Value.Equals("wave frequency"))
                                parameterChangerHelper.SetLinearDriveValue("WaveFrequencyHandle", unit.Value, float.Parse(newValue.Value));
                            else if (attributeToChange.Value.Equals("wave length"))
                                parameterChangerHelper.SetLinearDriveValue("WaveLengthHandle", unit.Value, float.Parse(newValue.Value));
                            else if (attributeToChange.Value.Equals("wave amplitude"))
                                parameterChangerHelper.SetLinearDriveValue("WaveAmplitudeHandle", unit.Value, float.Parse(newValue.Value));
                            else if (attributeToChange.Value.Equals("slit width"))
                                parameterChangerHelper.SetLinearDriveValue("SlitWidthHandle", unit.Value, float.Parse(newValue.Value));
                            else if (attributeToChange.Value.Equals("number of slits"))
                                parameterChangerHelper.SetLinearDriveValue("NumSlitsHandle", unit.Value, float.Parse(newValue.Value));
                            else if (attributeToChange.Value.Equals("field lines"))
                                parameterChangerHelper.SetLinearDriveValue("FieldLinesHandle", unit.Value, float.Parse(newValue.Value));
                            else if (attributeToChange.Value.Equals("ring resistance"))
                                parameterChangerHelper.SetLinearDriveValue("RingResistanceHandle", unit.Value, float.Parse(newValue.Value));
                            else if (attributeToChange.Value.Contains("field resolution"))
                                parameterChangerHelper.SetLinearDriveValue("VecFieldResHandle", unit.Value, float.Parse(newValue.Value));
                            else if (attributeToChange.Value.Contains("magnetic moment"))
                                parameterChangerHelper.SetLinearDriveValue("MagneticMomentHandle", unit.Value, float.Parse(newValue.Value));

                        }
                        else if (command.Value.Equals("startExperiment"))
                        {
                            SimulationController.Instance.StartSimulation();
                        }
                        else if (command.Value.Equals("stopExperiment"))
                        {
                            SimulationController.Instance.StopSimulation();
                        }
                        else if (command.Value.Equals("resetExperiment"))
                        {
                            SimulationController.Instance.ResetSimulation();
                        }
                    }
                }
            }
            else
            {
                //Debug.Log("Speech: found no commands in chatgpt's response, so let's say it instead");
                textTips.DisplayTip(s);
                speak(s);
            }
        }


        public void speak(string whatToSay)
        {
            if (chosenTTS.Equals(TtsOption.Azure))
            {
                if (azureTTS != null)
                    azureTTS.speak(whatToSay, true);
            }
            else
            {
                if (narrator != null)
                    narrator.speak(whatToSay);
            }
        }

        public void HandleChatGPTtutorResponse(string s)
        {
            Debug.Log(Time.time.ToString());
            Debug.Log("Speech: chatgpt tutor said " + s);
            textTips.DisplayTip(s);
            speak(s);
        }

        private void HandleLanguageChange(SystemLanguage lang)
        {
            Debug.Log("Speech: the language has changed!  now it's " + lang.ToString());
            if (lang.Equals(SystemLanguage.English))
                // do something
                Debug.Log("english");
            else if (lang.Equals(SystemLanguage.German))
                // do something
                Debug.Log("german");

            //LoadGeneralGrammar();

            int foundIndex = Array.FindIndex(experimentScenes, element => element.SceneName.Equals(SceneManager.Instance.ActiveSceneName));
            if (foundIndex >= 0)
                //LoadExperimentalGrammar(foundIndex + 1);
                Debug.Log("load experimental grammar");
        }

        private void HandleRoomChange(UnityEngine.SceneManagement.Scene currentScene, UnityEngine.SceneManagement.LoadSceneMode loadMode)
        {
            Debug.Log(Time.time.ToString());
            Debug.Log("dave, they have changed rooms, and now they're in <" + currentScene.name + ">");

            int foundIndex = Array.FindIndex(experimentScenes, element => element.SceneName.Equals(currentScene.name));
            if (foundIndex >= 0)
            {
                if (!alreadySetupLanguage)  //fyi, this happens here instead of the start method because this script begins its life in the bootstrapper scene
                {
                    LanguageManager languageManager = LanguageManager.Instance;
                    if (languageManager != null)
                    {
                        Debug.Log("Speech: the language manager says that the language is " + languageManager.CurrentLanguage.ToString());
                        languageManager.OnLanguageChanged.AddListener(HandleLanguageChange);
                    }
                }

                string basePrompt = "You are a tutor who is explaining how to use an educational, physics simulation software program called Maroon.  I'm going to show you several examples of commands that a player can give in the program.  \r\n\r\nOnce you have read these items, I'll ask you to explain the program to a potential player.  \r\n\r\nA player can change rooms by saying commands like the following:\r\nTake me to Falling Coil experiment\r\nGo to the Huygen's Principle room\r\nChange to Faraday's Law\r\n\r\nA player can control the playback of an experiment (or simulation) by saying commands like the following:\r\nReset the simulation\r\nStop experiment\r\nStart the simulation\r\nBegin the experiment\r\n\r\nA player can modify attributes of the experiment by saying commands like the following:\r\nChange field lines to 25\r\nModify ring resistance to 40%\r\nTurn iron filings off\r\nSet vector field resolution at 85%\r\nSet magnetic moment to 25%\r\n\r\n\r\n";
                string endPrompt = "\r\nPlease limit your future responses to 35 words.";
                string roomSpecificPrompt = "\r\nThe player is now in the Lobby.  Since this is not an experiment room, the player can only change rooms from here or ask general questions.";

                // here's where i load up a room-specific tutor prompt                
                if (currentScene.name.Equals("FallingCoil.vr"))
                {
                    roomSpecificPrompt = "\r\nThe player is now in the Falling Coil Experiment Room.  Here’s some info about that room that will help you explain things to the player if they have questions.\r\n\r\nThe Falling Coil experiment demonstrates the dynamics between a permanent magnet and a conductive non-magnetic ring. The magnet is positioned above a table and interacts with the coil falling down because of gravity. When the coil enters the magnetic field of the magnet, it induces an electric current. This leads to a magnetic field created by the coil, which interacts with the magnetic field of the magnet. If the current is high enough, the acting force pushes the coil upwards. However, the experiment output depends on the parameters of the coil and the magnet. The coil is defined by its mass, resistance, and self-inductance. The magnet is characterized by its magnetic moment, a.k.a magnet dipole moment. Users can change these parameters to observe the change in magnetic flux and the induced current. Additional visualizations such as field lines, vector fields or iron filling make the experiment more interactive and allow the user to see invisible phenomena to get a better understanding of the underlying concepts.\r\n\r\nIn this room, the player can change the following settings: field lines, magnetic moment, ring resistance, vector field resolution, and iron filings.";
                }
                else if (currentScene.name.Equals("MainMenu.vr"))
                {
                    roomSpecificPrompt = "\r\nThe player is now in the Lobby.  Since this is not an experiment room, the player can only change rooms from here or ask general questions.";
                }
                else if (currentScene.name.Equals("HuygensPrinciple.vr"))
                {
                    roomSpecificPrompt = "\r\nThe player is now in the Huygens’s Principle Experiment Room.  Here’s some info about that room that will help you explain things to the player if they have questions.\r\n\r\nThe Huygens’s Principle Experiment uses water waves in a basin to demonstrate the physical concept of diffraction. It is a phenomenon that occurs when a wave hits an obstacle or a slit. To show the effect of diffraction, a slit plate is placed into the basin. When a wave hits this plate, the points on the wave act as a new source of secondary waves that propagate. This results in an interference pattern behind the plate. To obtain different interference patterns, the user can replace the plate with three types of slit plates. The experiment is influenced by the user by grabbing and moving the plates and changing physical parameters such as frequency, amplitude, wavelength, or the propagation mode. To make the wave peaks and wave trough more visible, the wave color can be changed using a color wheel.\r\n\r\nIn this room, the player can change the following settings: number of slits, slit width, wave amplitude, wave frequency, wave length, and propagation mode.";
                }
                else if (currentScene.name.Equals("FaradaysLaw.vr"))
                {
                    roomSpecificPrompt = "\r\nThe player is now in the Faraday’s Law Experiment Room.  Here’s some info about that room that will help you explain things to the player if they have questions.\r\n\r\nThe Faraday’s Law Experiment shows the principle of induction when interacting with a permanent magnet and a conductive non-magnetic ring. Whenever the user moves the magnet, it causes a change in the magnetic flux and induces an electric current that generates another magnetic field. Through different visualizations the invisible magnetic field becomes visible and helps the user to better understand the underlying concepts. The virtual controls allow the user to influence the result of the experiment by changing the parameters of the magnet and the coil. The special feature of this experiment is the haptic feedback from the controllers, which allows the user to feel the acting forces. The controller vibrates as soon as a physical force acts on the magnet, where low vibration means a weak force, and high vibration means a heavy force.\r\n\r\nIn this room, the player can change the following settings: field lines, magnetic moment, ring resistance, vector field resolution, and iron filings.";
                }
                tutorConversation.ResetChat(basePrompt + roomSpecificPrompt + endPrompt);
            }
            else
            {
                //Debug.Log("Speech: couldn't find it! i guess i'm not in an experiment room right now");
            }
        }


        void RequestRoomChange(string roomName)
        {
            //speak(roomName);
            string sceneName = System.IO.Path.GetFileName(roomName);
            Maroon.CustomSceneAsset sceneAsset;            
            sceneAsset = SceneManager.Instance.GetSceneAssetBySceneName(sceneName);
            SceneManager.Instance.LoadSceneRequest(sceneAsset);
        }


        void Update()
        {
            if (SteamVR_Actions.default_Fire.GetStateDown(SteamVR_Input_Sources.Any))
            {
                if (SteamVR_Actions.default_Fire.GetStateDown(SteamVR_Input_Sources.LeftHand))
                {
                    lastButtonPressed = menuButton.Left;                    
                }
                else if (SteamVR_Actions.default_Fire.GetStateDown(SteamVR_Input_Sources.RightHand))
                {
                    lastButtonPressed = menuButton.Right;
                }

                if (!dictationRecognizer.Status.Equals(SpeechSystemStatus.Running))
                    dictationRecognizer.Start();

                //stop talking, fade out etc.
                if (chosenTTS.Equals(TtsOption.Azure))
                    azureTTS.shutUp();
                else
                    narrator.DaveReallyStopSpeech();
                
                textTips.FadeOut();

                //audioSources[0].Play();

                if (lastButtonPressed == menuButton.Left)
                {
                    audioSources[2].Play();
                    //if (narrator != null)
                    //narrator.speak("command?");
                    //speak("what's your command?");
                }
                else if (lastButtonPressed == menuButton.Right)
                {
                    audioSources[0].Play();
                    //if (narrator != null)
                    //narrator.speak("tutor");
                }
            }
        }

        
        private List<StructuredCommand> ParseTheString(string stringToBeParsed)
        {
            List<StructuredCommand> commands = new List<StructuredCommand>();

            if (stringToBeParsed != null && stringToBeParsed.Length > 0)
            {
                string[] separatedString = stringToBeParsed.Split(';');

                if (separatedString.Length > 0)
                {
                    for (int i = 0; i < separatedString.Length; i++)
                    {
                        string s = separatedString[i];
                        //Debug.Log("here's a whole line: " + s);

                        //first, find the tag            
                        string tag, value;
                        int startIndex = s.IndexOf('<');
                        int endIndex = s.IndexOf('>');

                        if (startIndex != -1 && endIndex != -1)
                        {
                            tag = s.Substring(startIndex + 1, endIndex - startIndex - 1);
                            //second, the value
                            startIndex = s.IndexOf('"');
                            if (startIndex != 0)
                            {
                                s = s.Substring(startIndex + 1);
                                endIndex = s.IndexOf('"');

                                if (endIndex != 0)
                                {
                                    value = s.Substring(0, endIndex);
                                    //Debug.Log("so the tag is " + tag + " and the value is " + value);
                                    commands.Add(new StructuredCommand(tag, value));
                                }
                            }
                        }
                    }
                }
            }
            return commands;
        }


        public struct StructuredCommand
        {
            public StructuredCommand(string tag, string value)
            {
                Tag = tag;
                Value = value;
            }
            public string Tag { get; }
            public string Value { get; }
            public override string ToString() => $"({Tag}, {Value})";
        }


    }
}