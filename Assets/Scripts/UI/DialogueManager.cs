using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Maroon.UI
{
    public class Message
    {
        public string Text { get; set; }
        public Color Color { get; set; }

        public Message(string text) : this(text, Color.white) { }

        public Message(string text, Color color)
        {
            Text = text;
            Color = color;
        }
    }

    public class DialogueManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _dialogBox;
        [SerializeField]
        private Text _text;
        [SerializeField]
        private float _letterPause = 0.01f;
        [SerializeField]
        private AudioClip _typeSound;

        private Queue<Message> _messages = new Queue<Message>();

        public bool TypeMessageRunning { get; private set; } = false;

        private void Awake()
        {
            _dialogBox.SetActive(false);
        }

        private void Update()
        {
            if (_dialogBox.activeSelf && Input.GetMouseButtonDown(0))
                _dialogBox.SetActive(false);

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

            _dialogBox.SetActive(true);
            SoundManager.Instance.PlaySingle(_typeSound);

            _text.text = "";
            _text.color = message.Color;

            foreach (var letter in message.Text)
            {
                if (!_dialogBox.activeSelf)
                {
                    TypeMessageRunning = false;
                    yield break;
                }

                _text.text += letter;
                yield return new WaitForSeconds(_letterPause);
            }

            yield return new WaitForSeconds(Mathf.Min(2f, 0.05f * message.Text.Length));
            _dialogBox.SetActive(false);
            TypeMessageRunning = false;
        }
    }
}


