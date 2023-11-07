using UnityEngine;
using UnityEngine.Windows.Speech;
using Maroon.GlobalEntities;
using GameLabGraz.VRInteraction;
using System;
using GEAR.Localization;
using ChatGPTWrapper;
using System.Collections.Generic;
using ACTA;

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
        
        enum menuButton
        {
            Left, Right
        }
        private menuButton lastButtonPressed = menuButton.Left;


        [SerializeField] private SystemLanguage chosenSpeechLanguage = SystemLanguage.English;

        public Maroon.CustomSceneAsset[] experimentScenes;

        [TextArea(10, 15)]
        public string[] roomSpecificPrompts;


        private string displayString, listeningString;
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

            displayString = "";
            listeningString = "";                   
            dictationRecognizer = new DictationRecognizer();            

            dictationRecognizer.DictationResult += (text, confidence) =>
            {
                //textTips.FadeOut();
                if (dictationRecognizer.Status.Equals(SpeechSystemStatus.Running))
                    dictationRecognizer.Stop();

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
                Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
            };

            dictationRecognizer.Start();
        }


        public void HandleChatGPTResponse(string s)
        {
            Debug.Log("chatgpt said " + s);
            textTips.DisplayTipWithFade(s, 2f);            

            List<StructuredCommand> commands = ParseTheString(s);

            if (commands != null && commands.Count > 0)
            {
                //commands.ForEach(p => Debug.Log(p));
                audioSources[1].Play();  // play the recognize tone

                for (int i = 0; i < commands.Count; i++)
                {
                    StructuredCommand command = commands[i];
                    Debug.Log(command.ToString());

                    if (command.Tag.Equals("command"))
                    {
                        if (command.Value.Equals("changeRoom")) {
                            StructuredCommand nextCommand = commands[i + 1];

                            Debug.Log("let's change the room, to " + nextCommand.Value);
                            //textTips.DisplayTip("let's change the room,\n to " + nextCommand.Value);  

                            if (nextCommand.Value.Equals("Falling Coil"))
                                RequestRoomChange("FallingCoil.vr");
                            else if (nextCommand.Value.Equals("Faraday's Law"))
                                RequestRoomChange("FaradaysLaw.vr");
                            else if (nextCommand.Value.Equals("Huygen's Principle"))
                                RequestRoomChange("HuygensPrinciple.vr");
                            else if (nextCommand.Value.Equals("Lobby"))
                                RequestRoomChange("MainMenu.vr");
                            else if (nextCommand.Value.Equals("Van de Graaf Generator"))
                                RequestRoomChange("VandeGraaffGenerator.vr");
                        }
                        else if (command.Value.Equals("modifyVariable"))
                        {                            
                            StructuredCommand attributeToChange = commands[i + 1];
                            StructuredCommand unit = commands[i + 2];
                            StructuredCommand newValue = commands[i + 3];
                            //Debug.Log("i need to change " + attributeToChange.Value + " to the new value " + newValue.Value + " " + unit.Value);

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
                                    //Debug.Log("dave, i found the IronFiling,which holds the script i want");
                                    scrIronFilings theScript = theParent.GetComponentInChildren<scrIronFilings>(true);

                                    if (newValue.Value.Contains("off"))
                                        theScript.hideFieldImage();
                                    else
                                        theScript.generateFieldImage();
                                }
                                else
                                    Debug.Log("dave, the script PARENT was null");
                            }
                            else if (attributeToChange.Value.Equals("field lines"))
                                parameterChangerHelper.SetLinearDriveValue("FieldLinesHandle", unit.Value, int.Parse(newValue.Value));
                            else if (attributeToChange.Value.Equals("ring resistance"))
                                parameterChangerHelper.SetLinearDriveValue("RingResistanceHandle", unit.Value, int.Parse(newValue.Value));
                            else if (attributeToChange.Value.Contains("field resolution"))
                                parameterChangerHelper.SetLinearDriveValue("VecFieldResHandle", unit.Value, int.Parse(newValue.Value));
                            else if (attributeToChange.Value.Contains("magnetic moment"))
                                parameterChangerHelper.SetLinearDriveValue("MagneticMomentHandle", unit.Value, int.Parse(newValue.Value));

                        }
                        else if (command.Value.Equals("startExperiment"))                        
                            SimulationController.Instance.StartSimulation();                        
                        else if (command.Value.Equals("stopExperiment"))                        
                            SimulationController.Instance.StopSimulation();                        
                        else if (command.Value.Equals("resetExperiment"))                        
                            SimulationController.Instance.ResetSimulation();                        
                    }
                }
            }
            else {
                //Debug.Log("dave, found no commands in chatgpt's response, so let's say it instead");
                textTips.DisplayTip(s);

                if (narrator != null)
                    narrator.speak(s);
            }
        }


        public void HandleChatGPTtutorResponse(string s)
        {
            Debug.Log("dave, chatgpt tutor said " + s);
            textTips.DisplayTip(s);            

            if (narrator != null)            
                narrator.speak(s);
        }


        private void HandleLanguageChange(SystemLanguage lang)
        {
            Debug.Log("dave, the language has changed!  now it's " + lang.ToString());
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
            int foundIndex = Array.FindIndex(experimentScenes, element => element.SceneName.Equals(currentScene.name));
            if (foundIndex >= 0)
            {
                if (!alreadySetupLanguage)  //fyi, this happens here instead of the start method because this script begins its life in the bootstrapper scene
                {
                    LanguageManager languageManager = LanguageManager.Instance;
                    if (languageManager != null)
                    {
                        Debug.Log("dave, the language manager says that the language is " + languageManager.CurrentLanguage.ToString());
                        languageManager.OnLanguageChanged.AddListener(HandleLanguageChange);

                    }
                }

                // here's where i would load up a second system prompt                
                //LoadExperimentSpecificTutorPrompt(foundIndex + 1);
            }
            else
            {
                //Debug.Log("dave, couldn't find it! i guess i'm not in an experiment room right now");
            }
        }


        /*void LoadExperimentSpecificTutorPrompt(int desiredIndex)
        {
            string promptToLoad = roomSpecificPrompts[desiredIndex];            
            tutorConversation.ReplaceSecondaryPrompt(promptToLoad);
            Debug.Log("dave, just loaded this prompt: " + promptToLoad);
        }*/



        void RequestRoomChange(string roomName)
        {
            string sceneName = System.IO.Path.GetFileName(roomName);
            Maroon.CustomSceneAsset sceneAsset;
            //sceneAsset = SceneManager.Instance.getScenesFromAllCategories().Find(element => sceneName == element.SceneName);
            sceneAsset = SceneManager.Instance.GetSceneAssetBySceneName(sceneName);
            SceneManager.Instance.LoadSceneRequest(sceneAsset);
            
        }


        void Update()
        {
            
            if (SteamVR_Actions.default_Fire.GetStateDown(SteamVR_Input_Sources.Any))
            {
                if (SteamVR_Actions.default_Fire.GetStateDown(SteamVR_Input_Sources.LeftHand))
                    lastButtonPressed = menuButton.Left;
                else if (SteamVR_Actions.default_Fire.GetStateDown(SteamVR_Input_Sources.RightHand))
                    lastButtonPressed = menuButton.Right;

                if (!dictationRecognizer.Status.Equals(SpeechSystemStatus.Running))
                    dictationRecognizer.Start();

                //stop talking, fade out etc.
                narrator.DaveReallyStopSpeech();
                textTips.FadeOut();

                audioSources[0].Play();
            }
        }

        /*
        void OnGUI()
        {
            listeningString = "<" + dictationRecognizer.Status.ToString() + ">";
            GUI.Label(new Rect(10, 10, 400, 20), listeningString);
            GUI.Label(new Rect(10, 30, 400, 20), displayString);
        }
        */
        private List<StructuredCommand> ParseTheString(string stringToBeParsed)
        {            
            List<StructuredCommand> commands = new List<StructuredCommand>();

            if (stringToBeParsed != null && stringToBeParsed.Length > 0) {                
                string[] separatedString = stringToBeParsed.Split(';');
            

                if (separatedString.Length > 0) { 

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