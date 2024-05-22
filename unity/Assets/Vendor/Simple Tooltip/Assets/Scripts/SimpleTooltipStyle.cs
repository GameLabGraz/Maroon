using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[Serializable]
[CreateAssetMenu]
public class SimpleTooltipStyle : ScriptableObject
{
    [System.Serializable]
    public struct Style
    {
        public string tag;
        public Color color;
        public bool bold;
        public bool italic;
        public bool underline;
        public bool strikethrough;
    }

    [Header("Tooltip Panel")]
    public Sprite slicedSprite;
    public Color color = Color.gray;

    [Header("Font")]
    public TMP_FontAsset fontAsset;
    public Color defaultColor = Color.white;

    [Header("Formatting")]
    public Style[] fontStyles;
}