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
    
    #region Preview
    public GameObject PreviewObject;
    public PreviewMode PreviewSetting = PreviewMode.None;
    public Vector3 PreviewOffset;
    [Range(0f, 10f)] public float PreviewScale = 1f;
    public Color PreviewColor = Color.red;

    public enum PreviewMode
    {
        None,
        Full
    }
    #endregion

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

        switch (PreviewSetting)
        {
            case PreviewMode.Full:
                if(PreviewObject == null)
                    return;
                // determine size of preview object
                var previewBounds = GetLocalBounds(PreviewObject);
                Vector3 previewSpawnPos = transform.position + PreviewOffset;

                // TODO deactivate every script (not renderer!) on that preview object + childs?

                // instantiate previewGO
                var o = Instantiate(PreviewObject, previewSpawnPos, Quaternion.identity);
                o.transform.localScale = o.transform.localScale * PreviewScale;

                Vector3 worldExtents = previewBounds.size * PreviewScale;
                Debug.Log("Extents X/Y/Z: " + worldExtents.x + "/" + worldExtents.y + "/" + worldExtents.z);
                Debug.Log("Spawn Position X/Y/Z: " + previewSpawnPos.x + "/" + previewSpawnPos.y + "/" + previewSpawnPos.z);


                // determine size of console
                //        GetComponent<Collider>().bounds.size
                //        GetComponent<Renderer>().bounds.size
                break;
        }
    }
    
    void OnDrawGizmosSelected() {
        switch (PreviewSetting)
        {
            case PreviewMode.Full:
                if(PreviewObject == null)
                    return;
                // determine size of preview object
                var previewBounds = GetLocalBounds(PreviewObject);
//                Debug.Log("X/Y/Z: " + previewBounds.size.x + "/" + previewBounds.size.y + "/" + previewBounds.size.z);
                // draw gizmo in the size of the bounds
                Vector3 previewSpawnPos = transform.position + PreviewOffset;
                Gizmos.color = PreviewColor;
                Gizmos.DrawCube(previewSpawnPos + previewBounds.center, previewBounds.size * PreviewScale);
                break;
        }
    }

    private Bounds GetLocalBounds(GameObject go)
    {
        Quaternion currentRotation = go.transform.rotation;
        go.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        Bounds bounds = new Bounds(go.transform.position, Vector3.zero);

        foreach (var renderer in go.GetComponentsInChildren<Renderer>())
            bounds.Encapsulate(renderer.bounds);

        Vector3 localCenter = bounds.center - go.transform.position;
        bounds.center = localCenter;
//        Debug.Log("The local bounds of this model is " + bounds);

        go.transform.rotation = currentRotation;
        return bounds;
    }

    private void Update()
    {
        _textTimer += Time.deltaTime;
        if (_textTimer >= TextTimerPass)
        {
            _textTimer = 0f;
            UpdateText();
        }
    }

    #region TextAnimation
    private int _textCnt = 0;
    private float _textTimer;
    private float TextTimerPass = 0f;
    public float TimeLetterFadeIn = 0.1f;
    public float TimeLetterFullShown = 2f;
    public float TimeLetterFullBlink = 0.4f;
    public int BlinkCount = 5;
    private int _blinkCounter = 0;
    private bool _blinkShown = true;
    public TextState _TextState = TextState.FadeIn;
    public ButtonSceneLoad.SceneLoad SceneLoad = ButtonSceneLoad.SceneLoad.Async;

    public enum TextState
    {
        FadeIn,
        Blink,
        FullShown
    }

    private void UpdateText()
    {
        switch (_TextState)
        {
            case TextState.FadeIn:
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
                _TextState = TextState.Blink;
                TextTimerPass = TimeLetterFullBlink;
                break;
            case TextState.Blink:
                if (_blinkCounter >= BlinkCount)
                {
                    _TextState = TextState.FadeIn;
                    TextTimerPass = TimeLetterFadeIn;
                    _blinkShown = false;
                    _blinkCounter = 0;
                    break;
                }

                _blinkShown = !_blinkShown;
                if (_blinkShown)
                {
                    _blinkCounter++;
                    _text.text = ShownName;
                }
                else
                {
                    _text.text = "";
                }

                break;
        }
    }

    #endregion
}