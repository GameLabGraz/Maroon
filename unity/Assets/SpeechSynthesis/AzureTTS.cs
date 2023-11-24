
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using System.Threading.Tasks;
using Valve.VR.InteractionSystem;
using System;

public class AzureTTS : MonoBehaviour
{  
    private const string SubscriptionKey = "7d4a4b4e6fe347198f9ae1f6185b449b";
    private const string Region = "eastus";
    public AudioSource audioSource;
    private SpeechConfig speechConfig;
    private SpeechSynthesizer synthesizer;
    public bool isSpeaking = false;

    void Start()
    {         
        speechConfig = SpeechConfig.FromSubscription(SubscriptionKey, Region);
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw24Khz16BitMonoPcm);            
        synthesizer = new SpeechSynthesizer(speechConfig);

        GetVoices();
    }

    public async Task GetVoices ()
    {
        Debug.Log("in get voices task");
        SynthesisVoicesResult result = await synthesizer.GetVoicesAsync();
        
        
        if (result != null)
        {            
            Debug.Log("dave, here are the available voices");
            foreach (VoiceInfo vi in result.Voices)
            {
                Debug.Log(vi.LocalName);
            }
            //result.Voice
        }
        Debug.Log("dave, no voices");
    }
        
    void OnDestroy()
    {
        if (synthesizer != null)
        {
            synthesizer.Dispose();
        }
    }

    public async Task speak(string whatToSay)
    {        
        await synthesizer.SpeakTextAsync(whatToSay);
        TextTips tips = GameObject.Find("TextTipsObject").GetComponent<TextTips>();
        if (tips != null)
            tips.FadeOut();
    }

    public void shutUp()
    {
        synthesizer.StopSpeakingAsync();
    }
}