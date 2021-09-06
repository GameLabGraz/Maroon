using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowNameAndFluidcolor : MonoBehaviour 
{
    [SerializeField] private Renderer labelRend;
    [SerializeField] private MeshRenderer fluidRend;

    private ShowFluid _showFluidScript;

    private void Start () 
    {
        _showFluidScript = ShowFluid.Instance;
    }

    public void ChangeName(Text label)
    {
        ChangeName(label.text);
    }

    public void ChangeName(TMP_Text label)
    {
        ChangeName(label.text);
    }

    public void ChangeName (string name) 
    {
        labelRend.material.mainTexture = Resources.Load<Texture>("Sprites/" + name);

        if (name.Equals("HNO3"))
            fluidRend.material = _showFluidScript.FluidWaterYellow;
        else
            fluidRend.material = _showFluidScript.FluidWaterColorless;
    }
}
