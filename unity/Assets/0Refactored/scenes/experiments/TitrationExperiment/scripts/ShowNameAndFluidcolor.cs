using UnityEngine;
using UnityEngine.UI;

public class ShowNameAndFluidcolor : MonoBehaviour 
{

    [SerializeField] private Text label;
    [SerializeField] private Renderer labelRend;
    [SerializeField] private MeshRenderer fluidRend;

    private ShowFluid _showFluidScript;

    private void Start () 
    {
        _showFluidScript = ShowFluid.Instance;
        ChangeName(label);
    }
    
    public void ChangeName (Text label) 
    {
        labelRend.material.mainTexture = Resources.Load<Texture>("Sprites/" + label.text);

        if (label.text.Equals("HNO3"))
            fluidRend.material = _showFluidScript.FluidWaterYellow;
        else
            fluidRend.material = _showFluidScript.FluidWaterColorless;
    }
}
