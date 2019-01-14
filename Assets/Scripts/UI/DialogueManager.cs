using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _dialogBox;
    [SerializeField]
    private Text _text;
    [SerializeField]
    private float _waitAtEnd = 1.0f;
    [SerializeField]
    private float _letterPause = 0.01f;
    [SerializeField]
    private AudioClip _typeSound;

    private Queue<string> _messages = new Queue<string>();
    private bool _typeMessageRunning = false;

    public bool TypeMessageRunning
    {
        get { return _typeMessageRunning; }
    }

    private void Awake()
    {
        _dialogBox.SetActive(false);
    }

    private void Update () 
    {
        if (_dialogBox.activeSelf && Input.GetMouseButtonDown(0))
            _dialogBox.SetActive(false);

        if (_messages.Count > 0 && !_typeMessageRunning)
            StartCoroutine(TypeMessage(_messages.Dequeue()));
    }

    public void ShowMessage(string message)
    {
        _messages.Enqueue(message);
    }

    //Function to display text letter-by-letter
    private IEnumerator TypeMessage(string message)
    {
        _typeMessageRunning = true;

        _dialogBox.SetActive(true);
        SoundManager.Instance.PlaySingle(_typeSound);

        _text.text = "";
        foreach (var letter in message)
        {
            if (!_dialogBox.activeSelf)
            {
                _typeMessageRunning = false;
                yield break;
            }

            _text.text += letter;        
            yield return new WaitForSeconds(_letterPause);
        }

        yield return new WaitForSeconds(_waitAtEnd);
        _dialogBox.SetActive(false);
        _typeMessageRunning = false;
    }
}
