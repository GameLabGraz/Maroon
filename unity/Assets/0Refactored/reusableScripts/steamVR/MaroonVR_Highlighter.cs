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

        private Color _defaultCol;
        
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
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            for (int rendererIndex = 0; rendererIndex < renderers.Length; rendererIndex++)
            {
                renderers[rendererIndex].material.color = newColor;
            }
        }
    }
}
