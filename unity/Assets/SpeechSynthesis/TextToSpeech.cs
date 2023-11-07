using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public static class TextToSpeech {
	[DllImport("WindowsVoice")]
	public static extern void initSpeech();
	[DllImport("WindowsVoice")]
	public static extern void destroySpeech();
	[DllImport("WindowsVoice")]
	public static extern void addToSpeechQueue( string s);
    
	static TextToSpeech() {
		initSpeech();
	}
	public static void speak(string msg) {
		addToSpeechQueue(msg);
	}    
}