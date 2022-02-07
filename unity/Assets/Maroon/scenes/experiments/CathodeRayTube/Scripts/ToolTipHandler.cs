using GEAR.Localization;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipHandler : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject partInfoToggle;
    private bool _collision;
    private Transform _hitObject;
    private GUIStyle _guiStyle = new GUIStyle();

    private void Start()
    {
        _guiStyle.normal.textColor = Color.black;
        _guiStyle.fontSize = 14;
    }

    private void Update()
    {
        if (!partInfoToggle.GetComponent<Toggle>().isOn || Camera.main != mainCamera)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        _collision = false;
        if (Physics.Raycast(ray, out var hit, 10.0f, LayerMask.NameToLayer("IgnorePostProcessing")) &&
            hit.transform.CompareTag("crtPart"))
        {
            _collision = true;
            _hitObject = hit.transform;
        }
    }

    private void OnGUI()
    {
        if (_collision)
        {
            Vector2 screenPos = Event.current.mousePosition;
            Vector2 convertedGUIPos = GUIUtility.ScreenToGUIPoint(screenPos);

            GUI.Label(new Rect(convertedGUIPos.x + 15, convertedGUIPos.y, 200, 20),
                LanguageManager.Instance.GetString(_hitObject.name), _guiStyle);
        }
    }
}