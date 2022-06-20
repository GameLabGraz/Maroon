using GEAR.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.Physics.CathodeRayTube
{
    public class ToolTipHandler : MonoBehaviour
    {
        public bool PartInfoToggle { get; set; }

        private bool _collision;
        private Transform _hitObject;
        private GUIStyle _guiStyle = new GUIStyle();

        private void Start()
        {
            PartInfoToggle = true;
            _guiStyle.normal.textColor = Color.black;
            _guiStyle.fontSize = 18;
        }

        private void Update()
        {
            if (!PartInfoToggle || Camera.main == null)
                return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            _collision = false;
            if (UnityEngine.Physics.Raycast(ray, out var hit, 10.0f, LayerMask.NameToLayer("IgnorePostProcessing")) &&
                hit.transform.CompareTag("crtPart"))
            {
                _collision = true;
                _hitObject = hit.transform;
            }
        }

        private void OnGUI()
        {
            if (!_collision || !PartInfoToggle)
                return;

            var screenPos = Event.current.mousePosition;
            var convertedGUIPos = GUIUtility.ScreenToGUIPoint(screenPos);

            GUI.Label(new Rect(convertedGUIPos.x + 15, convertedGUIPos.y, 200, 20),
                LanguageManager.Instance.GetString(_hitObject.name), _guiStyle);
        }
    }
}