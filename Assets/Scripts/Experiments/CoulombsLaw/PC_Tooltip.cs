using Localization;
using UnityEngine;
using UnityEngine.EventSystems;

public class PC_Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string TooltipKey = null;

    [Tooltip("If a key is specified the TooltipText gets ignored.")]
    public string TooltipText = "Default Text";
    
    private string _key;
    private string _tooltipText = "";
    private UI_TooltipPopup _tooltipPopup = null;


    private bool _onHovered = false;
    private float _hoverTime = 0f;

    private void Start()
    {
        var obj = GameObject.FindWithTag("TooltipPopup");
        if (obj) _tooltipPopup = obj.GetComponent<UI_TooltipPopup>();

        UpdateKey();
    }

    private void Update()
    {
        if (_key != TooltipKey) UpdateKey();

        if (_onHovered)
        {
            _hoverTime += Time.deltaTime;
            if (_hoverTime >= _tooltipPopup.displayedHoverTime)
            {
                if (_tooltipText != "") _tooltipPopup.DisplayTooltip(_tooltipText);
                _onHovered = false; //as we already displayed the tooltip
            }
                
        }
        
    }

    private void UpdateKey()
    {
        if (!Localization.LanguageManager.Instance) return;
        var tmp = Localization.LanguageManager.Instance.GetString(TooltipKey);
        if (tmp != "")
        {
            _tooltipText = tmp;
            _key = TooltipKey;
        }
        else
        {
            _tooltipText = TooltipText;
            Debug.Assert(false, "Localized Key '" + TooltipKey + "' was not found using Tooltip Text!");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipPopup == null) return;
        _onHovered = true;
        _hoverTime = 0f;
        
        if (_tooltipText == "") UpdateKey();

//        if (_tooltipText != "") _tooltipPopup.DisplayTooltip(_tooltipText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _onHovered = false;
        if (_tooltipPopup == null) return;
        _tooltipPopup.HideTooltip();
    }
}