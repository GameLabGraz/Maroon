using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshRenderer))]
public class SortingElement : MonoBehaviour
{
    [Header("Sorting Criteria:")]
    [Range(0, 100)]
    public int number;

    public Color color;
    [Range(0f, 0.3f)] public float size;

    [Header("Components")]
    public TextMeshPro text;
    
    private float _lastSize;
    private int _lastNumber;
    private Color _lastColor;
    private MeshRenderer _meshRenderer;

    private void Start()
    {
        //TODO: Maybe we want to change the visualisation
        number = Random.Range(1, 100);
        size = 0.1f + (float)number / 500;
        resetToDefaultColor();
        
        _meshRenderer = GetComponent<MeshRenderer>();
        AdaptAppearance();
    }

    private void Update()
    {
        if (number != _lastNumber || color != _lastColor || Math.Abs(size - _lastSize) > 0.00001)
            AdaptAppearance();
    }


    public void AdaptAppearance()
    {
        if(text)
            text.text = number.ToString();

        var trans = transform;
        trans.localScale = new Vector3(size, size, size);
        var localPos = trans.localPosition;
        localPos.y = 0.2f + size / 2f;
        trans.localPosition = localPos;

        foreach (var mat in _meshRenderer.materials)
            mat.color = color;
        
        _lastColor = color;
        _lastNumber = number;
        _lastSize = size;
    }

    public void resetToDefaultColor()
    {
        color.r = (float) number * 8.0f / 1000.0f + 0.2f;
        color.g = 0.0f;
        color.b = 0.0f;
    }

    public void markActiveColor()
    {
        color.r = 0.0f;
        color.g = 0.0f;
        color.b = (float) number * 8.0f / 1000.0f + 0.2f;
    }

    public void SetPivotColor()
    {
        color.g = 1.0f;
    }
}