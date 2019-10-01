using UnityEngine;
using UnityEngine.UI;

public class UIChargeDragHandler : UIItemDragHandlerSimple
{
    private CoulombLogic _coulombLogic;

    private float _chargeValue = 0.0f;

    private Color _chargeColor;

    [Header("General Settings")]
    public float maxValue = 5;
    public float minValue = -5;

    [Header("Image Components")]
    public Image BackgroundImage;
    public Image MinusImage;
    public Image PlusImage;

    public bool FixedPosition { get; set; } = false;

    public float ChargeValue
    {
        get => _chargeValue;
        set
        {
            _chargeValue = value;

            if (Mathf.Abs(_chargeValue) < Mathf.Epsilon)
            {
                BackgroundImage.color = Teal.NeutralChargeColor;
                MinusImage.gameObject.SetActive(false);
                PlusImage.gameObject.SetActive(false);
                return;
            }

            MinusImage.gameObject.SetActive(_chargeValue < 0);
            PlusImage.gameObject.SetActive(_chargeValue > 0);
            BackgroundImage.color = _chargeValue < 0 ?
                Color.Lerp(Teal.MinNegativeChargeColor, Teal.MaxNegativeChargeColor, _chargeValue / minValue) :
                Color.Lerp(Teal.MinPositiveChargeColor, Teal.MaxPositiveChargeColor, _chargeValue / maxValue);
        }
    }

    private void Start()
    {
        var simControllerObject = GameObject.Find("CoulombLogic");
        if (simControllerObject)
            _coulombLogic = simControllerObject.GetComponent<CoulombLogic>();

        ChargeValue = _chargeValue;
    }

    public void CreateParticleAtOrigin()
    {
        var vecFields = GameObject.FindGameObjectsWithTag("VectorField"); //should be 2 -> one 2d and one 3d, where only one should be active at a time

        foreach (var vecField in vecFields)
        {
            if (!vecField.activeInHierarchy) continue;
            Debug.Log("Add To Field: " + vecField.name);
            ShowObject(vecField.transform.position, vecField.transform.parent);
        }
    }

    protected override void ShowObject(Vector3 position, Transform parent)
    {
        Debug.Log("Set Active");

        var obj = Instantiate(generatedObject, parent, true);
        Debug.Assert(obj != null);
        var is2dScene = _coulombLogic.IsIn2dMode();

        var particle = obj.GetComponent<CoulombChargeBehaviour>();
        Debug.Assert(particle != null);
        particle.SetPosition(position);
        particle.SetFixedPosition(FixedPosition);
        particle.Charge = _chargeValue;

        var movement = obj.GetComponent<PC_DragHandler>();
        if (!movement) movement = obj.GetComponentInChildren<PC_DragHandler>();
        Debug.Assert(movement != null);
        movement.SetBoundaries(is2dScene ? minPosition2d : minPosition3d, is2dScene ? maxPosition2d : maxPosition3d);
        movement.allowedXMovement = movement.allowedYMovement = true;
        movement.allowedZMovement = !is2dScene;

        var arrowMovement = obj.GetComponentInChildren<PC_ArrowMovement>();
        Debug.Assert(arrowMovement != null);
        arrowMovement.minimumBoundary = is2dScene ? minPosition2d.transform : minPosition3d.transform;
        arrowMovement.maximumBoundary = is2dScene ? maxPosition2d.transform : maxPosition3d.transform;
        arrowMovement.restrictZMovement = is2dScene;

        var field = GameObject.FindGameObjectWithTag("Field").GetComponent<IField>(); //should be 2 -> one 2d and one 3d, where only one should be active at a time
        obj.GetComponentInChildren<FieldLine>().field = field;

        obj.SetActive(true);

        _coulombLogic.AddParticle(particle);
    }
}