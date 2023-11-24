
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using System.Threading.Tasks;
using Valve.VR.InteractionSystem;
using System;
using static ACTA.Narrator;
using ACTA;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;

public class AzureTTS : MonoBehaviour
{  
    private const string SubscriptionKey = "7d4a4b4e6fe347198f9ae1f6185b449b";
    private const string Region = "eastus";
    public AudioSource audioSource;
    private SpeechConfig speechConfig;
    private SpeechSynthesizer synthesizer;
    public Narrator narrator;
    IEnumerable<VoiceInfo> allTheVoices;

    void Start()
    {        
        speechConfig = SpeechConfig.FromSubscription(SubscriptionKey, Region);
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw24Khz16BitMonoPcm);            
        synthesizer = new SpeechSynthesizer(speechConfig);

        speechConfig.SpeechSynthesisLanguage = "en-US";
        speechConfig.SpeechSynthesisVoiceName = "en-US-JennyNeural";        

        GetVoices();
    }

    public async Task GetVoices ()
    {        
        SynthesisVoicesResult result = await synthesizer.GetVoicesAsync();
                
        if (result != null)
            allTheVoices = result.Voices;                        
    }
        
    public void SetTeacher()
    {
        if (narrator.preferredTeacher.Equals(Teacher.David))
        {
            IEnumerable<VoiceInfo> potentialVoices = allTheVoices.Where(voice => voice.Locale.Equals("en-US")).Where(voice => voice.Gender == SynthesisVoiceGender.Male);
            speechConfig.SpeechSynthesisVoiceName = potentialVoices.First().Name;
            speechConfig.SpeechSynthesisLanguage = "en-US";
        }
        else if (narrator.preferredTeacher.Equals(Teacher.Lisa))
        {
            IEnumerable<VoiceInfo> potentialVoices = allTheVoices.Where(voice => voice.Locale.Equals("de-DE")).Where(voice => voice.Gender == SynthesisVoiceGender.Female);
            speechConfig.SpeechSynthesisVoiceName = potentialVoices.First().Name;
            speechConfig.SpeechSynthesisLanguage = "de-DE";
            
        }
        synthesizer = new SpeechSynthesizer(speechConfig);
    }
        
    void OnDestroy()
    {
        if (synthesizer != null)
        {
            synthesizer.Dispose();
        }
    }

    public async Task speak(string whatToSay, bool withMarkup = false)
    {
        /*if (narrator != null)
        {
            
            string speakerPreferences = "<voice required=";  //<voice required=\"Gender=Male\">
            if (narrator.preferredGender.Equals(Gender.Male))
                speakerPreferences += "\"Gender=Male";
            else
                speakerPreferences += "\"Gender=Female";

            speakerPreferences += ";";

            if (narrator.preferredLanguage.Equals(Language.English))
                speakerPreferences += "Language=409\"";
            else if (narrator.preferredLanguage.Equals(Language.Deutsch))
                speakerPreferences += "Language=407\"";

            speakerPreferences += ">";
            

            whatToSay = speakerPreferences + whatToSay;
        }*/
        //Debug.Log("Speech: here's what i'm sending the narrator: " + whatToSay);


        if (withMarkup)
        {
            string prefixString = "<speak version=\"1.0\" xmlns =\"http://www.w3.org/2001/10/synthesis\" xmlns:mstts = \"https://www.w3.org/2001/mstts\" xml:lang = ";
            if (narrator.preferredLanguage.Equals(Language.English))
                prefixString += "\"en-US\""; 
            else if (narrator.preferredLanguage.Equals(Language.Deutsch))
                prefixString += "\"de - DE\"";

            prefixString += "> <voice name = \"";
            prefixString += speechConfig.SpeechSynthesisVoiceName;            
            //prefixString += "en-US-JennyNeural";
            prefixString += "\"> <mstts:express-as style = \"";
            prefixString += narrator.preferredTone;
            prefixString += "\" styledegree = \"2\" >";

            string postfixString = "</mstts:express-as> </voice></speak>";
            whatToSay = prefixString + whatToSay.TrimEnd() + postfixString;
            Debug.Log("dave" + whatToSay);
            await synthesizer.SpeakSsmlAsync(whatToSay);
        }
        else
        {
            await synthesizer.SpeakTextAsync(whatToSay);
        }
        TextTips tips = GameObject.Find("TextTipsObject").GetComponent<TextTips>();
        if (tips != null)
            tips.FadeOut();
    }

    public void shutUp()
    {
        synthesizer.StopSpeakingAsync();
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetTeacher();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetTeacher();
            speak(narrator.whatToSay, true);
            
        }
    }

}