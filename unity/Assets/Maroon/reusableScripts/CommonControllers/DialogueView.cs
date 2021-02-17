using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.UI
{
    public class DialogueView : MonoBehaviour
    {
        [SerializeField] private Text textComponent;
        [SerializeField] private TMP_Text textTMP;

        [SerializeField] private Image iconHint;
        [SerializeField] private Image iconError;
        [SerializeField] private Image iconWarning;
        [SerializeField] private Image iconSuccess;

        [Range(0f, 5f)] 
        public float fadeTime = 2f;

        public bool IsActive => gameObject.activeSelf;
        private Image _currentIcon;
        private bool _fadeIn;

        private float _currentTime;

        private void Start()
        {
            ClearIcons();
        }

        private void Update()
        {
            if (!_fadeIn) return;
            _currentTime += Time.deltaTime;
            _currentIcon.color = new Color(1f, 1f, 1f, _currentTime / fadeTime);

            if (_currentTime <= fadeTime) return;
            _currentIcon.color = new Color(1f, 1f, 1f, 1f);
            _fadeIn = false;
        }

        public void ShowMessage(string message)
        {
            if (textComponent)
                textComponent.text = message;

            if (textTMP)
                textTMP.text = message;
        }

        public void ClearMessage()
        {
            if (textComponent)
                textComponent.text = string.Empty;

            if (textTMP)
                textTMP.text = string.Empty;

            ClearIcons();
        }

        public void ClearIcons()
        {
            _fadeIn = false;
            _currentIcon = null;
            
            if(iconHint) iconHint.gameObject.SetActive(false);
            if (iconWarning) iconWarning.gameObject.SetActive(false);
            if (iconSuccess) iconSuccess.gameObject.SetActive(false);
            if (iconError) iconError.gameObject.SetActive(false);
        }

        public void SetIcon(MessageIcon icon)
        {
            ClearIcons();
            switch (icon)
            {
                case MessageIcon.MI_None:
                    _currentIcon = null;
                    break;
                case MessageIcon.MI_Ok:
                    _currentIcon = iconSuccess;
                    break;
                case MessageIcon.MI_Warning:
                    _currentIcon = iconWarning;
                    break;
                case MessageIcon.MI_Error:
                    _currentIcon = iconError;
                    break;
                case MessageIcon.MI_Hint:
                    _currentIcon = iconHint;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(icon), icon, null);
            }

            if (_currentIcon == null) return;
            _currentIcon.color = new Color(1f, 1f, 1f, 0f);
            _currentIcon.gameObject.SetActive(true);
            _fadeIn = true;
            _currentTime = 0f;
        }
        
        public void SetTextColor(Color color)
        {
            if (textComponent)
                textComponent.color = color;

            if (textTMP)
                textTMP.color = color;
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public void OnDisable()
        {
            ClearIcons();
        }
    }
}
