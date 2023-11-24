using UnityEngine;
using System.Collections;
using TMPro;
using System.Text.RegularExpressions;

namespace Valve.VR.InteractionSystem
{
    public class TextTips : MonoBehaviour
    {
        public GameObject theCamera;
        public Canvas theCanvas;
        public TextMeshProUGUI theText;
        public int maxCharactersPerLine = 32;
        public float yShift = 0;
        public float zShift = 1;

        CanvasGroup canvasGroup;
        IEnumerator CurrentFadeRoutine = null;

        void Start()
        {
            theCamera = GameObject.Find("VRCamera");
            if (theCamera != null)
            {
                Debug.Log("Speech: found the camera.  it's at " + theCamera.transform.position.ToString());
            }
            if (theCanvas != null)
            {
                canvasGroup = theCanvas.GetComponent<CanvasGroup>();
            }
            //DisplayTipWithFade("Welcome to Maroon!\nPress the left menu button to issue a command, or the right menu button if you need help.", 6f); 
        }
        
        void Update()
        {
            if (theCanvas != null)
            {
                Vector3 cameraPos = theCamera.transform.position;
                theCanvas.transform.position = new Vector3(cameraPos.x, cameraPos.y + yShift, cameraPos.z + zShift);
            }
        }

        public void DisplayTip(string newText)
        {
            theText.text = newText; 
            FadeIn();
        }
        public void DisplayTipWithFade(string newText, float fadeTimeInSeconds)
        {
            theText.text = newText; 
            FadeIn();
            Invoke(nameof(FadeOut), fadeTimeInSeconds);
        }
    
        public void FadeIn()
        {
            if (CurrentFadeRoutine != null)            
                StopCoroutine(CurrentFadeRoutine);            
            //CurrentFadeRoutine = Fade(0.1f, 1.0f, true, 0.02f, 0.01f);
            CurrentFadeRoutine = Fade(canvasGroup.alpha, 1.0f, true, 0.02f, 0.01f);
            StartCoroutine(CurrentFadeRoutine);
        }
        public void FadeOut() {
            if (CurrentFadeRoutine != null)
                StopCoroutine(CurrentFadeRoutine);
            //CurrentFadeRoutine = Fade(1.0f, 0f, false, 0.02f, 0.01f);
            CurrentFadeRoutine = Fade(canvasGroup.alpha, 0f, false, 0.02f, 0.01f);
            StartCoroutine(CurrentFadeRoutine);
        }

        IEnumerator Fade(float startAlpha, float endAlpha, bool fadeIn, float stepSize, float stepTime)
        {
            if (fadeIn)
            {
                for (float alpha = startAlpha; alpha <= endAlpha; alpha += stepSize)
                {
                    canvasGroup.alpha = alpha;
                    yield return new WaitForSeconds(stepTime);
                }
            }
            else
            {
                for (float alpha = startAlpha; alpha >= endAlpha; alpha -= stepSize)
                {
                    canvasGroup.alpha = alpha;
                    yield return new WaitForSeconds(stepTime);
                }
            }
        }
    }
}