
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace MaroonVR
{
    public class MaroonVR_CircularDrive : CircularDrive
    {
        [Header("Maroon VR Specific")] 
        public bool useStepsForValues = true;
        public bool useStepsForRotation = true;
        public bool useStepsAfterGrabEnded = true;
        [Range(0f, 10f)] public float stepSize = 1f;
        public bool useAsInteger = false;
        public bool useAsBool = false;

        public float initialValue = 5f;
        public float minimum = 0f;
        public float maximum = 10f;
        public int boolIsTrueWhenValue = 1;
        
        public ValueChangeEventFloat onValueChanged;
        public ValueChangeEventInt onValueChangedInt;
        public ValueChangeEventBool onValueChangedBool;

        protected float _currentValue;
        protected bool _currentValueBool;
        protected float _valueRange;
        
        private void Start()
        {
            if ( childCollider == null )
                childCollider = GetComponentInChildren<Collider>();

            if ( linearMapping == null )
                linearMapping = GetComponent<LinearMapping>();

            if ( linearMapping == null )
                linearMapping = gameObject.AddComponent<LinearMapping>();

            worldPlaneNormal = new Vector3(0.0f, 0.0f, 0.0f) {[(int) axisOfRotation] = 1.0f};
            localPlaneNormal = worldPlaneNormal;

            if ( transform.parent )
                worldPlaneNormal = transform.parent.localToWorldMatrix.MultiplyVector( worldPlaneNormal ).normalized;

            if ( limited )
            {
                start = Quaternion.identity;
                outAngle = transform.localEulerAngles[(int)axisOfRotation];

                if ( forceStart )
                {
                    outAngle = Mathf.Clamp( startAngle, minAngle, maxAngle );
                }
            }
            else
            {
                start = Quaternion.AngleAxis( transform.localEulerAngles[(int)axisOfRotation], localPlaneNormal );
                outAngle = 0.0f;
            }

            if ( debugText )
            {
                debugText.alignment = TextAlignment.Left;
                debugText.anchor = TextAnchor.UpperLeft;
            }

            _valueRange = maximum - minimum;
            _currentValue = Mathf.Clamp(initialValue, minimum, maximum);
            linearMapping.value = (_currentValue - minimum) / _valueRange;
            
            onValueChanged.Invoke(_currentValue);
            if(useAsInteger)
                onValueChangedInt.Invoke(Mathf.RoundToInt(_currentValue));
            _currentValueBool = Mathf.RoundToInt(_currentValue) == boolIsTrueWhenValue;
            if(useAsBool)
                onValueChangedBool.Invoke(_currentValueBool);
            
            UpdateAll();
        }
        protected override void UpdateGameObject()
        {
            if ( rotateGameObject )
            {
                var realAngle = outAngle;
                if (useStepsForRotation)
                {
                    realAngle = minAngle + (maxAngle - minAngle) * linearMapping.value;
                }

                transform.localRotation = start * Quaternion.AngleAxis( realAngle, localPlaneNormal );
            }
        }
        
        protected bool lastGrabEnded = false;
        
        protected override void HandHoverUpdate( Hand hand )
        {
            GrabTypes startingGrabType = hand.GetGrabStarting();
            bool isGrabEnding = hand.IsGrabbingWithType(grabbedWithType) == false;
            
            base.HandHoverUpdate(hand);
            
            if (lastGrabEnded != isGrabEnding && isGrabEnding && useStepsAfterGrabEnded && rotateGameObject)
            {
                Debug.Log("Correct end Position");
                var realAngle = minAngle + (maxAngle - minAngle) * linearMapping.value;
                Debug.Log("Lin Map: " + linearMapping.value + " - " + realAngle);

                transform.localRotation = start * Quaternion.AngleAxis( realAngle, localPlaneNormal );
            }
            lastGrabEnded = isGrabEnding;
            
        }
  
        
        protected override void UpdateLinearMapping()
        {
            if ( limited )
            {
                // Map it to a [0, 1] value
                linearMapping.value = ( outAngle - minAngle ) / ( maxAngle - minAngle );
            }
            else
            {
                // Normalize to [0, 1] based on 360 degree windings
                float flTmp = outAngle / 360.0f;
                linearMapping.value = flTmp - Mathf.Floor( flTmp );
            }
            
            _currentValue = Mathf.Clamp(linearMapping.value * _valueRange + minimum, minimum, maximum);
            
            if (useAsInteger)
                _currentValue = Mathf.RoundToInt(_currentValue);
            if (useAsBool)
                _currentValueBool = Mathf.RoundToInt(_currentValue) == boolIsTrueWhenValue;
            
            if (useStepsForValues)
            {
                var tmp = _currentValue % stepSize;
                if (tmp > stepSize / 2)
                    _currentValue = _currentValue - tmp + stepSize;
                else
                    _currentValue -= tmp;
				
                if (useAsInteger)
                    _currentValue = Mathf.RoundToInt(_currentValue);

                linearMapping.value = (_currentValue - minimum) / _valueRange;
            }
            
            onValueChanged.Invoke(_currentValue);
            if(useAsInteger)
                onValueChangedInt.Invoke(Mathf.RoundToInt(_currentValue));
            if(useAsBool)
                onValueChangedBool.Invoke(_currentValueBool);
		
            UpdateDebugText();
        }
    }
}
