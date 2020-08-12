using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace Maroon.UI
{
    public enum MessageIcon
    {
        MI_None,
        MI_Ok,
        MI_Warning,
        MI_Error,
        MI_Hint
    }
    
    public class Message
    {
        public MessageIcon Icon { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; }

        public Message(string text, MessageIcon icon = MessageIcon.MI_None) : this(text, Color.black, icon) { }

        public Message(string text, Color color, MessageIcon icon = MessageIcon.MI_None )
        {
            Text = text;
            Color = color;
            Icon = icon;
        }
    }

    public class DialogueManager : MonoBehaviour
    {
        [SerializeField]
        private DialogueView dialogView;
        [SerializeField] [Range(0.001f, 0.05f)]
        private float letterPause = 0.01f;
        [SerializeField]
        private AudioClip typeSound;

        [Range(2f, 10f)] [SerializeField] private float minMessageTime = 2f;

        private readonly Queue<Message> _messages = new Queue<Message>();

        public bool TypeMessageRunning { get; private set; }

        private void Awake()
        {
            if (!dialogView)
            {
                Debug.LogError("DialogueManager::Awake: There is no dialogue view.");
                return;
            }
            dialogView.SetActive(false);
        }

        private void Update()
        {
            if (!dialogView)
                return;

            if (dialogView.IsActive && Input.GetMouseButtonDown(0))
                dialogView.SetActive(false);

            if (_messages.Count > 0 && !TypeMessageRunning)
                StartCoroutine(TypeMessage(_messages.Dequeue()));
        }

        public void ShowMessage(string message)
        {
            _messages.Enqueue(new Message(message));
        }

        public void ShowMessage(Message message)
        {
            _messages.Enqueue(message);
        }

        //Function to display text letter-by-letter
        private IEnumerator TypeMessage(Message message)
        {
            TypeMessageRunning = true;

            dialogView.SetActive(true);
            SoundManager.Instance.PlaySingle(typeSound);

            var text = "";
            dialogView.ClearMessage();
            dialogView.SetTextColor(message.Color);
            dialogView.SetIcon(message.Icon);

            foreach (var letter in message.Text)
            {
                if (!dialogView.IsActive)
                {
                    TypeMessageRunning = false;
                    yield break;
                }

                text += letter;
                dialogView.ShowMessage(text);

                yield return new WaitForSeconds(letterPause);
            }

            yield return new WaitForSeconds(Mathf.Max(minMessageTime, letterPause * 5f * message.Text.Length));
            dialogView.SetActive(false);
            TypeMessageRunning = false;
        }
    }
}


