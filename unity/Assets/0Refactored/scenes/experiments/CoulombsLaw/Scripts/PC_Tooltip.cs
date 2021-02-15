using GEAR.Localization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class PC_Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [FormerlySerializedAs("TooltipKey")] public string tooltipKey = null;

    [FormerlySerializedAs("TooltipText")] [Tooltip("If a key is specified the TooltipText gets ignored.")]
    public string tooltipText = "Default Text";
    
    private string _key;
    private string _tooltipText = "";
    private UI_TooltipPopup _tooltipPopup = null;
    
    private bool _onHovered = false;
    private float _hoverTime = 0f;
    private Vector2 _screenPt;
    private static readonly Vector2 AllowedOffset = new Vector2(10f, 10f);

    private void Start()
    {
        var obj = GameObject.FindWithTag("TooltipPopup");
        if (obj) _tooltipPopup = obj.GetComponent<UI_TooltipPopup>();

        if (LanguageManager.Instance)
        {
            LanguageManager.Instance.OnLanguageChanged.AddListener((language) =>
            {
                UpdateKey();
            });
        }

        UpdateKey();
    }

    private void FixedUpdate()
    {
        if (_key != tooltipKey) UpdateKey();

        if (!_onHovered) return;

        var inputPos = Input.mousePosition;
        if (Mathf.Abs(inputPos.x - _screenPt.x) > AllowedOffset.x ||
            Mathf.Abs(inputPos.y - _screenPt.y) > AllowedOffset.y)
        {
            _hoverTime = 0;
            _screenPt = new Vector2(inputPos.x, inputPos.y);
            _tooltipPopup.HideTooltip();
            return;
        }
        
        _hoverTime += Time.deltaTime;
        
        if (_hoverTime >= _tooltipPopup.displayedHoverTime)
        {
            if (!string.IsNullOrEmpty(_tooltipText)) 
                _tooltipPopup.DisplayTooltip(_tooltipText);

            // _onHovered = false; //as we already displayed the tooltip
        }
    }

    private void UpdateKey()
    {
        if (!LanguageManager.Instance) return;
        var tmp = LanguageManager.Instance.GetString(tooltipKey);
        if (!string.IsNullOrEmpty(tmp))
        {
            _tooltipText = tmp;
            _key = tooltipKey;
        }
        else
        {
            _tooltipText = tooltipText;
            Debug.Assert(false, "Localized Key '" + tooltipKey + "' was not found using Tooltip Text!");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipPopup == null) return;
        _onHovered = true;
        _hoverTime = 0f;
        _screenPt = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        
        if (string.IsNullOrEmpty(_tooltipText)) UpdateKey();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _onHovered = false;
        if (_tooltipPopup == null) return;
        _tooltipPopup.HideTooltip();
    }
}