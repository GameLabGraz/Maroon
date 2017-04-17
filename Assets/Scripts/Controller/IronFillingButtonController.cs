using UnityEngine;
using VRTK;

public class IronFillingButtonController : VRTK_InteractableObject
{
    [SerializeField]
    private IronFiling ironFilling;

    public override void StartUsing(GameObject usingObject)
    {
        base.StartUsing(usingObject);

        Debug.Log("Iron Filling Button pressed, start Iron Filling");

        ironFilling.generateFieldImage();
    }

    public override void StopUsing(GameObject usingObject)
    {
        base.StopUsing(usingObject);
    }
}
