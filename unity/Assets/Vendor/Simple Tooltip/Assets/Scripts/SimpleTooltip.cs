using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class SimpleTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SimpleTooltipStyle simpleTooltipStyle;
    [TextArea] public string infoLeft = "Hello";
    [TextArea] public string infoRight = "";
    private STController tooltipController;
    private EventSystem eventSystem;
    private bool cursorInside = false;
    private bool isUIObject = false;
    private bool showing = false;

    private void Awake()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        tooltipController = FindObjectOfType<STController>();

        // Add a new tooltip prefab if one does not exist yet
        if (!tooltipController)
        {
            tooltipController = AddTooltipPrefabToScene();
        }
        if (!tooltipController)
        {
            Debug.LogWarning("Could not find the Tooltip prefab");
            Debug.LogWarning("Make sure you don't have any other prefabs named `SimpleTooltip`");
        }

        if (GetComponent<RectTransform>())
            isUIObject = true;

        // Always make sure there's a style loaded
        if (!simpleTooltipStyle)
            simpleTooltipStyle = Resources.Load<SimpleTooltipStyle>("STDefault");
    }

    private void Update()
    {
        if (!cursorInside)
            return;

        tooltipController.ShowTooltip();
    }

    public static STController AddTooltipPrefabToScene()
    {
        return Instantiate(Resources.Load<GameObject>("SimpleTooltip")).GetComponentInChildren<STController>();
    }

    private void OnMouseOver()
    {
        if (isUIObject)
            return;

        if (eventSystem)
        {
            if (eventSystem.IsPointerOverGameObject())
            {
                HideTooltip();
                return;
            }
        }
        ShowTooltip();
    }

    private void OnMouseExit()
    {
        if (isUIObject)
            return;
        HideTooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isUIObject)
            return;
        ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isUIObject)
            return;
        HideTooltip();
    }

    public void ShowTooltip()
    {
        showing = true;
        cursorInside = true;

        // Update the text for both layers
        tooltipController.SetCustomStyledText(infoLeft, simpleTooltipStyle, STController.TextAlign.Left);
        tooltipController.SetCustomStyledText(infoRight, simpleTooltipStyle, STController.TextAlign.Right);

        // Then tell the controller to show it
        tooltipController.ShowTooltip();
    }

    public void HideTooltip()
    {
        if (!showing)
            return;
        showing = false;
        cursorInside = false;
        tooltipController.HideTooltip();
    }

    private void Reset()
    {
        // Load the default style if none is specified
        if (!simpleTooltipStyle)
            simpleTooltipStyle = Resources.Load<SimpleTooltipStyle>("STDefault");

        // If UI, nothing else needs to be done
        if (GetComponent<RectTransform>())
            return;

        // If has a collider, nothing else needs to be done
        if (GetComponent<Collider>())
            return;

        // There were no colliders found when the component is added so we'll add a box collider by default
        // If you are making a 2D game you can change this to a BoxCollider2D for convenience
        // You can obviously still swap it manually in the editor but this should speed up development
        gameObject.AddComponent<BoxCollider>();
    }
}
