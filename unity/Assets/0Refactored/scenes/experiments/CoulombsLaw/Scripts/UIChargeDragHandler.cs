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
                BackgroundImage.color = CoulombChargeBehaviour.NeutralChargeColor;
                MinusImage.gameObject.SetActive(false);
                PlusImage.gameObject.SetActive(false);
                return;
            }

            MinusImage.gameObject.SetActive(_chargeValue < 0);
            PlusImage.gameObject.SetActive(_chargeValue > 0);
            BackgroundImage.color = _chargeValue < 0 ?
                Color.Lerp(CoulombChargeBehaviour.MinNegativeChargeColor, CoulombChargeBehaviour.MaxNegativeChargeColor, _chargeValue / minValue) :
                Color.Lerp(CoulombChargeBehaviour.MinPositiveChargeColor, CoulombChargeBehaviour.MaxPositiveChargeColor, _chargeValue / maxValue);
        }
    }
    
    private new void Start()
    {
        base.Start();

        var simControllerObject = GameObject.Find("CoulombLogic");
        if (simControllerObject)
            _coulombLogic = simControllerObject.GetComponent<CoulombLogic>();
    }

    protected override void ShowObject(Vector3 position, Transform parent)
    {
        _coulombLogic.CreateCharge(generatedObject, position, _chargeValue, FixedPosition);
    }

    public void AddWithGivenSettings()
    {
        _coulombLogic.CreateCharge(generatedObject, _coulombLogic.GetComponent<PC_SelectionHandler>().GetPositionInWorldSpace(PC_SelectScript.SelectType.ChargeSelect),
            _chargeValue, FixedPosition);
    }
}