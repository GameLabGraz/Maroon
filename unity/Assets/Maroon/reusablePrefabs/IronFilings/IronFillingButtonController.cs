using UnityEngine;
using VRTK;

public class IronFillingButtonController : VRTK_InteractableObject
{
    [SerializeField]
    private scrIronFilings ironFilling;

    public override void StartUsing(VRTK_InteractUse currentUsingObject = null)
    {
        base.StartUsing(usingObject);

        Debug.Log("Iron Filling Button pressed, start Iron Filling");

        ironFilling.generateFieldImage();
    }
}
