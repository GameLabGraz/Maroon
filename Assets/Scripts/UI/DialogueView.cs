using UnityEngine;
using UnityEngine.UI;

namespace Maroon.UI
{
    public class DialogueView : MonoBehaviour
    {
        [SerializeField]
        private Text textComponent;

        public bool IsActive => gameObject.activeSelf;

        public void ShowMessage(string message)
        {
            if (textComponent)
                textComponent.text = message;
        }

        public void ClearMessage()
        {
            if (textComponent)
                textComponent.text = string.Empty;
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
