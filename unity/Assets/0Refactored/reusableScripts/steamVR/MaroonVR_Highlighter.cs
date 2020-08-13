using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace MaroonVR
{
    public class MaroonVR_Highlighter : MonoBehaviour
    {
        public Color highlightColor = Color.cyan;
        public Color pressedColor = Color.gray;
        public Color defaultColor = Color.white;
        public Color attachedColor = Color.white;

        private Color _defaultCol;
        private bool _isHovering = false;

        protected void Start()
        {
            var maroonHoverBtn = GetComponent<MaroonVR_HoverButton>();
            if(maroonHoverBtn && maroonHoverBtn.stayPressed)
            {
                maroonHoverBtn.OnButtonOn.AddListener(() => { _defaultCol = pressedColor; });
                maroonHoverBtn.OnButtonOff.AddListener(() => { _defaultCol = defaultColor; });
            } 
            var hoverButton = GetComponent<HoverButton>();
            if (hoverButton)
            {
                _defaultCol = default;
                hoverButton.onButtonDown.AddListener(OnButtonDown);
                hoverButton.onButtonUp.AddListener(OnButtonUp);
            }
            
            var hoverEvents = GetComponent<InteractableHoverEvents>();
            if(hoverEvents){
                hoverEvents.onHandHoverBegin.AddListener(() => { 
                    ColorSelf(highlightColor);
                    _isHovering = true;
                });
                hoverEvents.onHandHoverEnd.AddListener(() =>
                {
                    ColorSelf(defaultColor);
                    _isHovering = false;
                });
                hoverEvents.onAttachedToHand.AddListener(() => { ColorSelf(attachedColor); });
                hoverEvents.onDetachedFromHand.AddListener(() =>
                {
                    if (_isHovering)
                        ColorSelf(highlightColor);
                    else
                        ColorSelf(defaultColor);
                });
            }
        }
        
        public void OnButtonDown(Hand fromHand)
        {
            ColorSelf(highlightColor);
        }

        public void OnButtonUp(Hand fromHand)
        {
            ColorSelf(_defaultCol);
        }

        private void ColorSelf(Color newColor)
        {
            var renderers = GetComponentsInChildren<Renderer>();
            foreach (var t in renderers)
            {
                t.material.color = newColor;
            }
        }
    }
}
