using UnityEngine;

public class PenController : MonoBehaviour
{
    [SerializeField]
    private Material penMaterial;

    [SerializeField]
    private float lineWidth = 0.1f;

    public Material getPenMaterial()
    {
        return this.penMaterial;
    }

    public float getLineWidth()
    {
        return this.lineWidth;
    }
}
