using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TooltipPopup : MonoBehaviour
{
    public Canvas popupCanvas;
    public RectTransform popupObject;
    public TextMeshProUGUI tooltipText;
    public Vector3 offset = Vector3.zero;
    public float padding = 0f;

    public float displayedHoverTime = 1f;

    // Update is called once per frame
    void Update()
    {
        if(!popupCanvas.gameObject.activeSelf) return;
        //update so that the tooltip is not out of the screen
        var newPos = Input.mousePosition + offset;
        newPos.z = 0f;
        var rightToScreen = Screen.width - (newPos.x + popupObject.rect.width * popupCanvas.scaleFactor / 2) - padding;
        if (rightToScreen < 0)
            newPos.x += rightToScreen;

        var leftToScreen = 0 - (newPos.x - popupObject.rect.width * popupCanvas.scaleFactor / 2) + padding;
        if (leftToScreen > 0)
            newPos.x += leftToScreen;

        var topToScreen = Screen.height - (newPos.y + popupObject.rect.height * popupCanvas.scaleFactor) - padding;
        if (topToScreen < 0)
            newPos.y += topToScreen;

        popupObject.transform.position = newPos;
    }

    public void DisplayTooltip(string tooltip)
    {
        tooltipText.text = tooltip;
        popupCanvas.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(popupObject); //force correct resizing
    }

    public void HideTooltip()
    {
        popupCanvas.gameObject.SetActive(false);
    }
}
