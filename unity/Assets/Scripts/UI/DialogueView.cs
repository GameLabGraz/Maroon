using System;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.UI
{
    public class DialogueView : MonoBehaviour
    {
        [SerializeField]
        private Text textComponent;

        public Image iconHint;
        public Image iconError;
        public Image iconWarning;
        public Image iconSuccess;
        [Range(0f, 5f)] 
        public float fadeTime = 2f;

        public bool IsActive => gameObject.activeSelf;
        private Image _currentIcon = null;
        private bool _fadeIn = false;

        private float _currentTime = 0f;

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
            _fadeIn = false;
            _currentIcon = null;
            
            iconHint.gameObject.SetActive(false);
            iconWarning.gameObject.SetActive(false);
            iconSuccess.gameObject.SetActive(false);
            iconError.gameObject.SetActive(false);
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
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public void OnDisable()
        {
            ClearIcons();
            Debug.Log("OnDisable Dialogue View");
        }
    }
}
