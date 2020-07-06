using System;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.UI
{
    public class DialogueView : MonoBehaviour
    {
        [SerializeField]
        private Text textComponent;

        public GameObject iconHint;
        public GameObject iconError;
        public GameObject iconWarning;
        public GameObject iconSuccess;
        [Range(0f, 5f)] 
        public float FadeTime = 2f;

        public bool IsActive => gameObject.activeSelf;
        private GameObject currentIcon = null;
        private bool fadeOut = false;
        private bool fadeIn = false;

        private float _currentTime = 0f;

        private void Update()
        {
            if (fadeOut)
            {
                _currentTime -= Time.deltaTime;
                currentIcon.GetComponent<Image>().color = new Color(1f, 1f, 1f, _currentTime / FadeTime);

                if (_currentTime < 0f)
                {
                    currentIcon.SetActive(false);
                    currentIcon = null;
                    fadeOut = false;
                }
            }
            else if (fadeIn)
            {
                _currentTime += Time.deltaTime;
                currentIcon.GetComponent<Image>().color = new Color(1f, 1f, 1f, _currentTime / FadeTime);

                if (_currentTime > FadeTime)
                {
                    currentIcon.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                    fadeIn = false;
                }
            }
        }

        public void ShowMessage(string message)
        {
            if (textComponent)
                textComponent.text = message;
        }

        public void ClearMessage()
        {
            if (textComponent)
            {
                textComponent.text = string.Empty;
            }
            ClearIcons();
        }

        public void ClearIcons()
        {
            if (currentIcon != null)
            {
                fadeOut = true;
                fadeIn = false;
                _currentTime = FadeTime;
            }
                
            if (iconHint != currentIcon) iconHint.SetActive(false);
            if(iconWarning != currentIcon) iconWarning.SetActive(false);
            if(iconSuccess != currentIcon) iconSuccess.SetActive(false);
            if(iconError != currentIcon) iconError.SetActive(false);
        }

        public void SetIcon(MessageIcon icon)
        {
            ClearIcons();
            if(currentIcon != null) currentIcon.SetActive(false);

            switch (icon)
            {
                case MessageIcon.MI_None:
                    currentIcon = null;
                    break;
                case MessageIcon.MI_Ok:
                    currentIcon = iconSuccess;
                    break;
                case MessageIcon.MI_Warning:
                    currentIcon = iconWarning;
                    break;
                case MessageIcon.MI_Error:
                    currentIcon = iconError;
                    break;
                case MessageIcon.MI_Hint:
                    currentIcon = iconHint;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(icon), icon, null);
            }

            if (currentIcon != null)
            {
                currentIcon.SetActive(true);
                fadeIn = true;
                fadeOut = false;
                _currentTime = 0f;
            }
        }
        
        public void SetTextColor(Color color)
        {
            if (textComponent)
                textComponent.color = color;
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

    }
}
