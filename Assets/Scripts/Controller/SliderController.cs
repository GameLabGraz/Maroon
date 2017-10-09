using System.Collections;
using UnityEngine;
using VRTK;

public class SliderController : VRTK_InteractableObject
{
    [SerializeField]
    private GameObject invokeObject;

    [SerializeField]
    private string methodName;

    [SerializeField]
    private Vector3 moveOffset;

    [SerializeField]
    private Vector3 MinPosition;
    [SerializeField]
    private Vector3 MaxPosition;

    [SerializeField]
    private float minValue;
    [SerializeField]
    private float maxValue;

    [SerializeField]
    private TextMesh ValueText;

    [SerializeField]
    private bool isInteger = false;

    [SerializeField]
    private float MoveSpeedFactor = 0.25f;

    private int oldIntValue;

    private bool IsMoving = false;

    private Vector3 UsingObjectPosition;

    private Vector3 UsingObjectPositionOld;

    private Vector3 SliderMoveDirection;

    private void Start()
    {
        SliderMoveDirection = MinPosition - MaxPosition;
        oldIntValue = (int)getValue();
    }

    public override void StartUsing(GameObject usingObject)
    {
        base.StartUsing(usingObject);

        Debug.Log("Slider start using...");

        IsMoving = true;
        UsingObjectPosition = usingObject.transform.position;
        StartCoroutine(Move());
    }

    public override void StopUsing(GameObject usingObject)
    {
        base.StopUsing(usingObject);

        Debug.Log("Slider stop using...");

        IsMoving = false;
    }

    public float getValue()
    {
        float minMaxDistance = Vector3.Distance(MinPosition, MaxPosition);
        float sliderDistance = Vector3.Distance(MinPosition, this.transform.localPosition);
        float value = minValue + (maxValue - minValue) * (sliderDistance / minMaxDistance);
        return value;
    }

    private IEnumerator Move()
    {
        while (IsMoving)
        {
            /*
            UsingObjectPositionOld = UsingObjectPosition;
            UsingObjectPosition = usingObject.transform.position;

            Vector3 moveDirection = transform.InverseTransformDirection(UsingObjectPositionOld - UsingObjectPosition);

            float moveDistance = moveDirection.magnitude;

            if (Vector3.Dot(SliderMoveDirection, moveDirection) > 0)
                this.transform.localPosition += SliderMoveDirection * moveDistance * MoveSpeedFactor;
            else
                this.transform.localPosition -= SliderMoveDirection * moveDistance * MoveSpeedFactor;

            if (Vector3.Dot(MaxPosition - this.transform.localPosition, Vector3.right) > 0)
                this.transform.localPosition = MaxPosition;

            if (Vector3.Dot(MinPosition - this.transform.localPosition, Vector3.right) < 0)
                this.transform.localPosition = MinPosition;
            
            */

            UsingObjectPositionOld = UsingObjectPosition;
            UsingObjectPosition = usingObject.transform.position;

            Vector3 moveDirection = transform.InverseTransformDirection(UsingObjectPositionOld - UsingObjectPosition);

            float moveDistance = moveDirection.magnitude;

            if (Vector3.Dot(SliderMoveDirection, moveDirection) > 0)
                this.transform.localPosition += SliderMoveDirection * moveDistance * MoveSpeedFactor;
            else
                this.transform.localPosition -= SliderMoveDirection * moveDistance * MoveSpeedFactor;


            if (Vector3.Distance(MinPosition, MaxPosition) < Vector3.Distance(MinPosition, this.transform.localPosition))
                this.transform.localPosition = MaxPosition;

            if (Vector3.Distance(MinPosition, MaxPosition) < Vector3.Distance(MaxPosition, this.transform.localPosition))
                this.transform.localPosition = MinPosition;





            /*
            Vector3 newPosition = this.transform.position;

            Vector3 UsingObjectPosition = UsingObject.transform.position;

            newPosition.x = UsingObjectPosition.x;


            this.transform.position = newPosition + moveOffset;

            if (MaxPosition.x <= 0 && this.transform.localPosition.x < MaxPosition.x || MaxPosition.x > 0 && this.transform.localPosition.x > MaxPosition.x)
                this.transform.localPosition = new Vector3(MaxPosition.x, this.transform.localPosition.y, this.transform.localPosition.z);

            if (MinPosition.x < 0 && this.transform.localPosition.x < MinPosition.x || MinPosition.x >= 0 && this.transform.localPosition.x > MinPosition.x)
                this.transform.localPosition = new Vector3(MinPosition.x, this.transform.localPosition.y, this.transform.localPosition.z);
            */

            if (isInteger)
            {
                int intValue = (int)getValue();
                if(intValue != oldIntValue)
                {
                    oldIntValue = intValue;
                    invokeObject.SendMessage(methodName, intValue);
                }
            }
            else
                invokeObject.SendMessage(methodName, getValue());
            
                
            yield return new WaitForFixedUpdate();
        }
    }

    protected override void Update()
    {
        base.Update();

        if (ValueText == null)
            return;

        if (isInteger)
            ValueText.text = ((int)getValue()).ToString();
        else
            ValueText.text = getValue().ToString("0.00");
    }

}
