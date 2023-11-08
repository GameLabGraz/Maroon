using UnityEngine;
using UnityEngine.Windows.Speech;
using Maroon.GlobalEntities;
using GameLabGraz.VRInteraction;
using System;
using Maroon.Physics.Electromagnetism.VanDeGraaff;
using GEAR.Localization;


namespace Valve.VR.InteractionSystem
{
    public class GrammarSpeechRecognizerReboot : MonoBehaviour
    {
        [SerializeField] private SystemLanguage chosenSpeechLanguage = SystemLanguage.English;
        public Maroon.CustomSceneAsset[] experimentScenes;
        public UnityEngine.Object[] englishGrammarFiles;
        public UnityEngine.Object[] germanGrammarFiles;
        public ParameterChangerHelper parameterChangerHelper;

        private GrammarRecognizer grammarRecognizer;
        private GrammarRecognizer grammarRecognizerExperiment;
        private UnityEngine.Object[] selectedGrammarSet;

        private string displayString, listeningString;
        AudioSource[] audioSources;
        bool alreadySetupLanguage = false;
        TextTips textTips;

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            audioSources = GetComponents<AudioSource>();
        }

        void Start()
        {
            parameterChangerHelper = gameObject.GetComponent<ParameterChangerHelper>();
            textTips = GameObject.Find("TextTipsObject").GetComponent<TextTips>();            
            selectedGrammarSet = englishGrammarFiles;

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += HandleRoomChange;

            SystemLanguage systemLang = Application.systemLanguage;

            if (systemLang == SystemLanguage.English)
                selectedGrammarSet = englishGrammarFiles;
            else if (systemLang == SystemLanguage.German)
                selectedGrammarSet = germanGrammarFiles;

            displayString = "";
            listeningString = "";

            LoadGeneralGrammar();
            
            PhraseRecognitionSystem.Shutdown();
            PhraseRecognitionSystem.OnError += Grammar_OnError;
        }

        private void HandleLanguageChange(SystemLanguage lang)
        {
            Debug.Log("dave, the language has changed!  now it's " + lang.ToString());
            if (lang.Equals(SystemLanguage.English))
                selectedGrammarSet = englishGrammarFiles;
            else if (lang.Equals(SystemLanguage.German))
                selectedGrammarSet = germanGrammarFiles;

            LoadGeneralGrammar();

            int foundIndex = Array.FindIndex(experimentScenes, element => element.SceneName.Equals(SceneManager.Instance.ActiveSceneName));
            if (foundIndex >= 0)            
                LoadExperimentalGrammar(foundIndex + 1);            
        }

        private void HandleRoomChange(UnityEngine.SceneManagement.Scene currentScene, UnityEngine.SceneManagement.LoadSceneMode loadMode)
        {
            //Debug.Log("dave, egad!  the room has changed and is now " + currentScene.name);

            int foundIndex = Array.FindIndex(experimentScenes, element => element.SceneName.Equals(currentScene.name));
            if (foundIndex >= 0)
            {
                if (!alreadySetupLanguage)  //fyi, this happens here instead of the start method because this script begins its life in the bootstrapper scene
                {
                    LanguageManager languageManager = LanguageManager.Instance;
                    if (languageManager != null)
                    {
                        //Debug.Log("dave, the language manager says that the language is " + languageManager.CurrentLanguage.ToString());
                        languageManager.OnLanguageChanged.AddListener(HandleLanguageChange);
                    }
                }

                if (currentScene.name.Equals("FallingCoil.vr"))
                {                    
                    textTips.DisplayTip("here are some example commands: \n - begin experiment \n - set field lines to 45 \n - change vector field resolution to 50%");
                }

                LoadExperimentalGrammar(foundIndex + 1);
            }
            else
            {
                Debug.Log("dave, couldn't find it! i guess i'm not in an experiment room right now");
            }
        }

        void LoadGeneralGrammar()
        {
            if (grammarRecognizer != null)
            {
                grammarRecognizer.Stop();
                grammarRecognizer.Dispose();
            }
            grammarRecognizer = new GrammarRecognizer(Application.streamingAssetsPath + "/SRGS/" + selectedGrammarSet[0].name + ".xml", ConfidenceLevel.Low);
            grammarRecognizer.OnPhraseRecognized += Grammar_OnPhraseRecognized;
            grammarRecognizer.Start();
            Debug.Log("dave, just loaded grammar " + selectedGrammarSet[0].name);
        }

        void LoadExperimentalGrammar(int desiredIndex)
        {
            string experimentGrammarToLoad = selectedGrammarSet[desiredIndex].name;
            
            if (grammarRecognizerExperiment != null)
            {
                grammarRecognizerExperiment.Stop();
                grammarRecognizerExperiment.Dispose();
            }
            grammarRecognizerExperiment = new GrammarRecognizer(Application.streamingAssetsPath + "/SRGS/" + experimentGrammarToLoad + ".xml", ConfidenceLevel.Low);
            grammarRecognizerExperiment.OnPhraseRecognized += Grammar_OnPhraseRecognized;
            grammarRecognizerExperiment.Start();
            Debug.Log("dave, just loaded grammar " + experimentGrammarToLoad);
        }

        void RequestRoomChange(string roomName)
        {
            string sceneName = System.IO.Path.GetFileName(roomName);
            Maroon.CustomSceneAsset sceneAsset;
                        
            //sceneAsset = SceneManager.Instance.getScenesFromAllCategories().Find(element => sceneName == element.SceneName);
            sceneAsset = SceneManager.Instance.GetSceneAssetBySceneName(sceneName);
            SceneManager.Instance.LoadSceneRequest(sceneAsset);
        }

        private void Grammar_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            // we've heard a phrase, so stop listening            
            PhraseRecognitionSystem.Shutdown();
            audioSources[1].Play();  // play the recognize tone
            textTips.FadeOut();

            Debug.Log("dave, Phrase Recognized: <" + args.text + ">");
            SemanticMeaning[] semanticMeanings = args.semanticMeanings;  // extracting the semantic tags
            if (semanticMeanings != null)
            {
                Debug.Log("dave, there's a semantic meaning here, with " + semanticMeanings.Length.ToString() + " items.");

                // let's parse the semantic meaning to figure out what action to take               
                for (int i = 0; i < semanticMeanings.Length; i++)
                {
                    SemanticMeaning currentSemanticMeaning = semanticMeanings[i];
                    Debug.Log("dave, <" + currentSemanticMeaning.key.ToString() + "> = <" + currentSemanticMeaning.values[0] + ">");

                    if (currentSemanticMeaning.key.ToString().Equals("roomName"))  // let's change rooms                    
                    {
                        //textTips.DisplayTipWithFade("let's change the room,\n to " + currentSemanticMeaning.values[0]);
                        RequestRoomChange(currentSemanticMeaning.values[0]);
                    }

                    if (currentSemanticMeaning.key.ToString().Equals("experimentAction"))  // let's affect the experiment
                    {
                        if (currentSemanticMeaning.values[0].Equals("reset"))
                            SimulationController.Instance.ResetSimulation();
                        else if (currentSemanticMeaning.values[0].Equals("start")) 
                        { 
                            SimulationController.Instance.StartSimulation();
                            
                            if(SceneManager.Instance.ActiveSceneName.Equals("VandeGraaffGenerator.vr"))
                            {                                
                                GameObject parent = GameObject.Find("VandeGraaff");
                                if (parent != null)
                                {
                                    if (parent.TryGetComponent<VanDeGraaffController>(out var c))
                                    {
                                        c.On = true;
                                    }
                                }

                            }
                        }
                        else if (currentSemanticMeaning.values[0].Equals("stop")) { 
                            SimulationController.Instance.StopSimulation();
                            
                            if (SceneManager.Instance.ActiveSceneName.Equals("VandeGraaffGenerator.vr"))
                            {
                                GameObject parent = GameObject.Find("VandeGraaff");
                                if (parent != null)
                                {
                                    if (parent.TryGetComponent<VanDeGraaffController>(out var c))
                                    {
                                        c.On = false;
                                    }
                                }

                            }
                        }

                    }

                    if (currentSemanticMeaning.key.ToString().Equals("attribute")) // let's change the value of an experimental variable
                    {
                        string attributeToChange = currentSemanticMeaning.values[0];
                        Debug.Log("dave, attribute to change is <" + attributeToChange + ">");

                        if (attributeToChange.Equals("ShowChargeButton"))
                        {
                            VRHoverButton theButton = parameterChangerHelper.FindParticularInactiveButtonInChildren("ButtonFront_ShowCharge", "ShowChargeButton");
                            if (theButton != null)
                            {
                                if (args.text.Contains("hide"))
                                    theButton.ForceButtonState(false);
                                else if (args.text.Contains("show"))
                                    theButton.ForceButtonState(true);
                            }
                        }

                        else if (attributeToChange.Equals("IronFilings"))
                        {
                            GameObject theParent = GameObject.Find("Experiment");
                            if (theParent != null)
                            {
                                //Debug.Log("dave, i found the IronFiling,which holds the script i want");
                                scrIronFilings theScript = theParent.GetComponentInChildren<scrIronFilings>(true);

                                if (args.text.Contains("off"))
                                    theScript.hideFieldImage();
                                else
                                    theScript.generateFieldImage();
                            }
                            else
                                Debug.Log("dave, the script PARENT was null");
                        }
                        else
                        {
                            string newValue = "0";
                            string unit = semanticMeanings[i + 1].values[0];
                            if (semanticMeanings.Length > i + 2)
                            {
                                newValue = semanticMeanings[i + 2].values[0];
                            }                            
                            Debug.Log("dave, they want to change <" + attributeToChange + "> to this new value: " + newValue + " percent? " + unit);
                            parameterChangerHelper.SetLinearDriveValue(attributeToChange, unit, int.Parse(newValue));
                        }
                    }

                }
            }

        }


        void Grammar_OnError(SpeechError errorCode)
        {
            Debug.Log("malfunction: " + errorCode.ToString());
        }

        void Update()
        {            
            if (SteamVR_Actions.default_Fire.GetStateDown(SteamVR_Input_Sources.Any))
            {
                PhraseRecognitionSystem.Restart();
                textTips.DisplayTip("I'm listening...");
                audioSources[0].Play();
            }
        }
        /*
        void OnGUI()
        {
            listeningString = "<" + PhraseRecognitionSystem.Status.ToString() + ">";
            GUI.Label(new Rect(10, 10, 400, 20), listeningString);
            GUI.Label(new Rect(10, 30, 400, 20), displayString);
        }
        */
    }
}