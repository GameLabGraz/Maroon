using System.Collections.Generic;
using UnityEngine;

public class Dielectric : MonoBehaviour
{
    private enum DielectricMaterial
    {
        Vacuum,  Glass, Water, Porcelain, Ceramic
    }

    private Dictionary<DielectricMaterial, float> relativePermittivity = new Dictionary<DielectricMaterial, float>
    {
        { DielectricMaterial.Vacuum, 1.0f },
        { DielectricMaterial.Glass,  7.0f },
        { DielectricMaterial.Water, 80.0f },
        { DielectricMaterial.Porcelain, 5.5f },
        { DielectricMaterial.Ceramic, 1530.0f }
    };

    private Dictionary<DielectricMaterial, Color> materialColor = new Dictionary<DielectricMaterial, Color>
    {
        { DielectricMaterial.Glass, Color.black },
        { DielectricMaterial.Water, Color.blue },
        { DielectricMaterial.Porcelain, Color.white },
        { DielectricMaterial.Ceramic, Color.gray }
    };

    [SerializeField]
    private DielectricMaterial material = DielectricMaterial.Vacuum;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float materialAlphaColorvalue = 0.5f;

    private Renderer renderer_;

    void Start ()
    {
        renderer_ = GetComponent<Renderer>();
        UpdateMaterialColor();

    }

    private void UpdateMaterialColor()
    {
        if (materialColor.ContainsKey(material))
        {
            Color color = materialColor[material];
            color.a = materialAlphaColorvalue;
            renderer_.material.color = color;
            renderer_.enabled = true;
        }
        else
            renderer_.enabled = false;
    }

    public float GetRelativePermittivity()
    {
        return relativePermittivity[material];
    }
}
