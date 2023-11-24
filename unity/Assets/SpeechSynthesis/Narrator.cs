using System.Collections;
using UnityEngine;
using System.Text;
using System.Runtime.InteropServices;
using Valve.VR.InteractionSystem;
using UnityEditor.PackageManager;

namespace ACTA
{
    public class Narrator : MonoBehaviour
    {

        [DllImport("WindowsTTS")]
        public static extern void initSpeech();

        [DllImport("WindowsTTS")]
        public static extern void destroySpeech();

        [DllImport("WindowsTTS")]
        public static extern void addToSpeechQueue(byte[] s);

        [DllImport("WindowsTTS")]
        public static extern void clearSpeechQueue();

        [DllImport("WindowsTTS")]
        public static extern void statusMessage(StringBuilder str, int length);

        [DllImport("WindowsTTS")]
        public static extern void changeVoice(int vIdx);

        
        [DllImport("WindowsTTS")]
        public static extern bool isSpeaking();        
        public static Narrator theVoice = null;
        public int voiceIdx = 0;

        [TextArea(10, 15)]
        public string whatToSay = "<voice required=\"Gender=Female;Age!=Child\"><volume level=\"50\"> This text should be spoken at volume level fifty.   </volume> <volume level=\"100\">     This text should be spoken at volume level one hundred.   </volume>";

        public enum Gender
        {
            Male, Female
        }
        public Gender preferredGender = Gender.Female;

        public enum Language
        {
            English, Deutsch
        }
        public Language preferredLanguage = Language.English;

        void OnEnable()
        {
            if (theVoice == null)
            {
                theVoice = this;
                Debug.Log("Initializing speech");
                initSpeech();
            }
        }

        void OnDestroy()
        {
            if (theVoice == this)
            {
                Debug.Log("Destroying speech");
                destroySpeech();
                theVoice = null;
            }
        }

        public void NextVoice()
        {            
            voiceIdx++;
        }
                
        public void speak(string whatToSay, bool interruptable = true)
        {
            ChangeTheVoice();
            
            

            string speakerPreferences = "<voice required=";  //<voice required=\"Gender=Male\">
            if (preferredGender.Equals(Gender.Male))            
                speakerPreferences += "\"Gender=Male";
            else
                speakerPreferences += "\"Gender=Female";

            speakerPreferences += ";";

            if (preferredLanguage.Equals(Language.English))
                speakerPreferences += "Language=409\"";
            else if (preferredLanguage.Equals(Language.Deutsch))
                speakerPreferences += "Language=407\"";

            speakerPreferences += ">";


            whatToSay = speakerPreferences + whatToSay;
            //Debug.Log("Speech: here's what i'm sending the narrator: " + whatToSay);

            Encoding encoding = Encoding.GetEncoding(System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ANSICodePage);
            var data = encoding.GetBytes(whatToSay);
            if (interruptable)
            {
                //clearSpeechQueue();
                DaveReallyStopSpeech();
            }
            addToSpeechQueue(data);

            StartCoroutine(CheckIfImStillTalking());
        }

        public void DaveReallyStopSpeech()
        {
            if (theVoice == this)
            {
                destroySpeech();
                theVoice = null;
            }
            if (theVoice == null)
            {
                theVoice = this;                
                initSpeech();
            }
        }

        private void Awake()
        {
            ChangeTheVoice();
        }

        public void ChangeTheVoice()
        {
            changeVoice(voiceIdx);
            
        }
         
        IEnumerator CheckIfImStillTalking()
        {            
            yield return new WaitForSeconds(2f);
            
            if(!isSpeaking())
            {
                //Debug.Log("coroutine: i finished speaking!");
                TextTips tips = GameObject.Find("TextTipsObject").GetComponent<TextTips>();
                if (tips != null)
                    tips.FadeOut();
                yield return null;
            }            
            else
            {
                StopCoroutine(CheckIfImStillTalking());                
                StartCoroutine(CheckIfImStillTalking());                
            }
        }


        /*public void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                speak(whatToSay);
            }
        }*/

        private void OnApplicationQuit()
        {
            Narrator.destroySpeech();
        }
    }
}
