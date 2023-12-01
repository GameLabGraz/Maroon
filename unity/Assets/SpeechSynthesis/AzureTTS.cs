
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using System.Threading.Tasks;
using Valve.VR.InteractionSystem;
using static ACTA.Narrator;
using ACTA;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CognitiveServices.Speech.Audio;
using System.IO;
using System;

public class AzureTTS : MonoBehaviour
{
    public UnityEngine.Object keyFile;
    public Narrator narrator;    
    
    public enum GermanVoice
    {
        Katja, Conrad, Amala, Bernd, Christoph, Elke, Gisela, Kasper, Killian, Klarissa, Klaus, Louisa, Maja, Ralf, Seraphina, Tanja
    }
    public GermanVoice preferredGermanVoice = GermanVoice.Katja;

    public enum EnglishVoice
    {
        JennyMultilingual, Jenny, Guy, Aria, Davis, Amber, Ana, Andrew, Ashley, Brandon, Brian, Christopher, Cora, Elizabeth, Emma, Eric, Jacob, Jane, Jason, Michelle, Monica, Nancy, Roger, Sara, Steffan, Tony, AIGenerate1, AIGenerate2, Blue, JennyMultilingualV2, RyanMultilingual
    }
    public EnglishVoice preferredEnglishVoice = EnglishVoice.JennyMultilingual;

    public List<string> germanVoiceNameList = new List<string>();
    public List<string> englishVoiceNameList = new List<string>();

    public IEnumerable<VoiceInfo> potentialVoices;
    public IEnumerable<VoiceInfo> germanVoices;
    public IEnumerable<VoiceInfo> englishVoices;

    private string SubscriptionKey;
    private const string Region = "eastus";    
    private SpeechConfig speechConfig;
    private SpeechSynthesizer synthesizer;
    
    IEnumerable<VoiceInfo> allTheVoices;
    

    void Start()
    {
        GetApiKeyFromFile();

        speechConfig = SpeechConfig.FromSubscription(SubscriptionKey, Region);
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw24Khz16BitMonoPcm);            
        synthesizer = new SpeechSynthesizer(speechConfig);
        speechConfig.SpeechSynthesisLanguage = "en-US";
        speechConfig.SpeechSynthesisVoiceName = "en-US-JennyNeural";   
        GetVoices();
    }



    public void GetApiKeyFromFile()
    {
        string path = Application.streamingAssetsPath + "/" + keyFile.name;
        Debug.Log("dave, path is " + path);
        try
        {
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    SubscriptionKey = sr.ReadLine();
                    sr.Close();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async Task GetVoices ()
    {        
        SynthesisVoicesResult result = await synthesizer.GetVoicesAsync();
                
        if (result != null) { 
            allTheVoices = result.Voices;
            germanVoices = allTheVoices.Where(voice => voice.Locale.Equals("de-DE"));
            englishVoices = allTheVoices.Where(voice => voice.Locale.Equals("en-US"));

            germanVoiceNameList.Clear();            
            foreach (VoiceInfo vi in germanVoices)
            {
                germanVoiceNameList.Add(vi.Name);
                //Debug.Log("daveGerman, <" + vi.Name + ">");
            }
            englishVoiceNameList.Clear();
            foreach (VoiceInfo vi in englishVoices)
            {
                englishVoiceNameList.Add(vi.Name);
                //Debug.Log("daveEnglish, <" + vi.Name + ">");
            }
        }
    }
        
    public void SetTeacher()
    {
        if (narrator.preferredTeacher.Equals(Teacher.David))
        {            
            potentialVoices = allTheVoices.Where(voice => voice.Locale.Equals("en-US")).Where(voice => voice.Gender == SynthesisVoiceGender.Male);
            speechConfig.SpeechSynthesisVoiceName = potentialVoices.First().Name;
            speechConfig.SpeechSynthesisLanguage = "en-US";
        }
        else if (narrator.preferredTeacher.Equals(Teacher.Lisa))
        {
            potentialVoices = allTheVoices.Where(voice => voice.Locale.Equals("de-DE")).Where(voice => voice.Gender == SynthesisVoiceGender.Female);
            speechConfig.SpeechSynthesisVoiceName = potentialVoices.First().Name;            
            speechConfig.SpeechSynthesisLanguage = "de-DE";       
        }
        else  // just look at the chosen voice
        {
            if (narrator.preferredLanguage.Equals(Language.English))
            {
                speechConfig.SpeechSynthesisLanguage = "en-US";
                int chosenIndex = (int)preferredEnglishVoice;
                //Debug.Log("dave, they chose language English, and voice item number " + preferredEnglishVoice + ", which is at index " + chosenIndex.ToString());
                //Debug.Log("dave, so i think that means they want this voice: " + englishVoiceNameList[chosenIndex]);
                speechConfig.SpeechSynthesisVoiceName = englishVoiceNameList[chosenIndex];

            }
            if (narrator.preferredLanguage.Equals(Language.Deutsch))
            {
                speechConfig.SpeechSynthesisLanguage = "de-DE";
                int chosenIndex = (int)preferredGermanVoice;
                //Debug.Log("dave, they chose language German, and voice item number " + preferredGermanVoice + ", which is at index " + chosenIndex.ToString());
                //Debug.Log("dave, so i think that means they want this voice: " + germanVoiceNameList[chosenIndex]);
                speechConfig.SpeechSynthesisVoiceName = germanVoiceNameList[chosenIndex];
            }
        }
        synthesizer = new SpeechSynthesizer(speechConfig);        
    }
        
    void OnDestroy()
    {
        if (synthesizer != null)
            synthesizer.Dispose();        
    }

    public async Task speak(string whatToSay, bool withMarkup = false)
    {
        if (withMarkup)
        {
            string prefixString = "<speak version=\"1.0\" xmlns =\"http://www.w3.org/2001/10/synthesis\" xmlns:mstts = \"https://www.w3.org/2001/mstts\" xml:lang = ";
            if (narrator.preferredLanguage.Equals(Language.English))
                prefixString += "\"en-US\""; 
            else if (narrator.preferredLanguage.Equals(Language.Deutsch))
                prefixString += "\"de - DE\"";

            prefixString += "> <voice name = \"";
            prefixString += speechConfig.SpeechSynthesisVoiceName;            
            prefixString += "\"> <mstts:express-as style = \"";
            prefixString += narrator.preferredTone;
            prefixString += "\" styledegree = \"2\" >";

            string postfixString = "</mstts:express-as> </voice></speak>";
            whatToSay = prefixString + whatToSay.TrimEnd() + postfixString;
            //Debug.Log("dave" + whatToSay);
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
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetTeacher();
            //speak(narrator.whatToSay, true);
            if (narrator.preferredLanguage.Equals(Language.Deutsch))
            {
                speak("Mein Name ist " + preferredGermanVoice.ToString() + ", und ich werde Ihre hilfreiche Nachhilfelehrerin sein", true);
            }
            if (narrator.preferredLanguage.Equals(Language.English))
            {
                speak("my name is " + preferredEnglishVoice.ToString() + ", and I will be your helpful tutor", true);
            }

        }
    }

}