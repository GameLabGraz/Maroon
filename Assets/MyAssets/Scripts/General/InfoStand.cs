using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InfoStand : MonoBehaviour
{
    private Text _text;
    private Text _buttonText;
    private ButtonSceneLoad _buttonSceneLoad;

    public string ShownName;
    public string SceneName;
    public string ButtonText = "Enter";

    private void Awake()
    {
        var textFields = GetComponentsInChildren<Text>();
        _text = textFields[0];
        _buttonText = textFields[1];
        _buttonSceneLoad = GetComponentInChildren<ButtonSceneLoad>();
        
    }

    private void Start()
    {
        _text.text = ShownName;
        _buttonText.text = ButtonText;
        _buttonSceneLoad.SceneName = SceneName;
        _buttonSceneLoad.SceneLoading = SceneLoad;
        TextTimerPass = TimeLetterFadeIn;
    }

    private int _textCnt = 0;
    private float _textTimer;
    private float TextTimerPass = 0f;
    public float TimeLetterFadeIn = 0.1f;
    public float TimeLetterFullShown = 2f;
    public float TimeLetterFullBlink = 0.4f;
    public int BlinkCount = 5;
    private int blinkCounter = 0;
    private bool blinkShown = true;
    public TextState _TextState = TextState.FadeIn;
    public ButtonSceneLoad.SceneLoad SceneLoad = ButtonSceneLoad.SceneLoad.Async;

    public enum TextState
    {
        FadeIn,
        Blink,
        FullShown
    }

    private void Update()
    {
        _textTimer += Time.deltaTime;
        if (_textTimer >= TextTimerPass)
        {
            _textTimer = 0f;
            switch (_TextState)
            {
                case TextState.FadeIn:
//                    Debug.Log("Fade In");
//                    Debug.Log(_textCnt);
                    if (_textCnt >= ShownName.Length)
                    {
                        _TextState = TextState.FullShown;
                        TextTimerPass = TimeLetterFullShown;
                        _textCnt = 0;
                        break;
                    }
                    _textCnt++;
                    _text.text = ShownName.Substring(0, _textCnt);
                    break;
                case TextState.FullShown:
//                    Debug.Log("Full shown");
                    _TextState = TextState.Blink;
                    TextTimerPass = TimeLetterFullBlink;
                    break;
                case TextState.Blink:
                    if (blinkCounter >= BlinkCount)
                    {
                        _TextState = TextState.FadeIn;
                        TextTimerPass = TimeLetterFadeIn;
                        blinkShown = false;
                        blinkCounter = 0;
                        break;
                    }

                    blinkShown = !blinkShown;
                    if (blinkShown)
                    {
                        blinkCounter++;
                        _text.text = ShownName;
                    }
                    else
                    {
                        _text.text = "";
                    }
                    break;
            }
        }
    }
}