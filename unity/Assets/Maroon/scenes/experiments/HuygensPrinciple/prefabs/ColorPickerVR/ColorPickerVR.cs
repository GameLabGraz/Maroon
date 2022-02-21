using System;
using System.Collections;
using System.Collections.Generic;
using GameLabGraz.VRInteraction;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ValueChangeEventColor: UnityEvent<Color>
{
}

public class ColorPickerVR : MonoBehaviour
{
    [SerializeField] 
    protected VRCircularDrive ColorCircle;

    [SerializeField] 
    protected VR3DDrive ColorRect;

    [SerializeField] protected Renderer ColorRectRenderer;
    [SerializeField] protected int ColorRectRenderMaterialIndex = 0;
    [SerializeField] protected Renderer OutputRenderer;
    [SerializeField] protected int OutputRenderMaterialIndex;

    public Color currentRGB = new Color(1, 0, 0);
    public Vector3 currentHSV = new Vector3(0, 100, 0);
    
    public ValueChangeEventColor onColorChanged;
    private static readonly int ColorAngle = Shader.PropertyToID("_ColorAngle");

    // Start is called before the first frame update
    void Start()
    {
        ChangeColor(currentHSV, false);
        ColorCircle.onValueChanged.AddListener(arg0 => OnColorCircleChanged());
    }

    void ChangeColor(Vector3 hsv, bool notify)
    {
        currentHSV = hsv;
        currentRGB = HSVToRGB(currentHSV.x, currentHSV.y / 100f, currentHSV.z / 100f);

        if (ColorRectRenderer && ColorRectRenderMaterialIndex < ColorRectRenderer.materials.Length)
        {
            ColorRectRenderer.materials[ColorRectRenderMaterialIndex].SetFloat(ColorAngle, (int)currentHSV.x);
        }

        if (OutputRenderer && OutputRenderMaterialIndex < OutputRenderer.materials.Length)
        {
            OutputRenderer.materials[OutputRenderMaterialIndex].color = currentRGB;
        }
        
        if(notify)
            onColorChanged.Invoke(currentRGB);
    }
    
    void OnColorCircleChanged()
    {
        currentHSV.x = Math.Abs((int) ColorCircle.outAngle) % 360;
        ChangeColor(currentHSV, true);
    }
    
    Color HSVToRGB(float h, float s, float v)
    {
        var c = s * v;
        var x = c * (1 - Mathf.Abs(((h/60) % 2) - 1));
        var m = v - c;
        Vector3 rgb1;

        if(0 <= h && h < 60)
            rgb1 = new Vector3(c, x, 0);
        else if(60 <= h && h < 120)
            rgb1 = new Vector3(x, c, 0);
        else if(120 <= h && h < 180)
            rgb1 = new Vector3(0, c, x);
        else if(180 <= h && h < 240)
            rgb1 = new Vector3(0, x, c);
        else if(240 <= h && h < 300)
            rgb1 = new Vector3(x, 0, c);
        else
            rgb1 = new Vector3(c, 0, x);
            
        return new Color(rgb1.x + m, rgb1.y + m, rgb1.z + m, 1f);
    }

}
