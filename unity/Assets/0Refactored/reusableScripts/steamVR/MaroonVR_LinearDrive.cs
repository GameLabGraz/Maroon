using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

namespace MaroonVR
{
	[System.Serializable]
	public class ValueChangeEventFloat : UnityEvent<float> { }
	[System.Serializable]
	public class ValueChangeEventInt : UnityEvent<float> { }
	
	public class MaroonVR_LinearDrive : LinearDrive
	{
		[Header("Maroon VR Specific")] public bool useSteps = true;
		[Range(0f, 10f)] public float stepSize = 1f;
		public bool useAsInteger = false;

		public float initialValue = 5f;
		public float minimum = 0f;
		public float maximum = 10f;

		public ValueChangeEventFloat onValueChanged;
		public ValueChangeEventInt onValueChangedInt;

		protected float _currentValue;
		protected float _valueRange;
		
		protected virtual void Awake()
		{
			base.Awake();
		}

		protected virtual void Start()
		{
			if ( linearMapping == null )
				linearMapping = GetComponent<LinearMapping>();
			if ( linearMapping == null )
				linearMapping = gameObject.AddComponent<LinearMapping>();

			initialMappingOffset = linearMapping.value;

			if ( repositionGameObject )
				UpdateLinearMapping( transform );
			
			_valueRange = maximum - minimum;
			_currentValue = Mathf.Clamp(initialValue, minimum, maximum);
			linearMapping.value = (_currentValue - minimum) / _valueRange;
			transform.position = Vector3.Lerp(startPosition.position, endPosition.position, linearMapping.value);
			
			onValueChanged.Invoke(_currentValue);
			if(useAsInteger)
				onValueChanged.Invoke(Mathf.RoundToInt(_currentValue));
		}
		
		protected override void HandAttachedUpdate(Hand hand)
		{
			UpdateLinearMapping(hand.transform);

			if (hand.IsGrabEnding(gameObject))
			{
				hand.DetachObject(gameObject);
			}
		}
		
		protected new void UpdateLinearMapping(Transform updateTransform)
		{
			prevMapping = linearMapping.value;
			linearMapping.value = Mathf.Clamp01(initialMappingOffset + CalculateLinearMapping(updateTransform));

			_currentValue = Mathf.Clamp(linearMapping.value * _valueRange + minimum, minimum, maximum);
			if (useAsInteger)
				_currentValue = Mathf.RoundToInt(_currentValue);

			if (useSteps)
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
				onValueChanged.Invoke(Mathf.RoundToInt(_currentValue));
			
			mappingChangeSamples[sampleCount % mappingChangeSamples.Length] =
				(1.0f / Time.deltaTime) * (linearMapping.value - prevMapping);
			sampleCount++;

			if (repositionGameObject)
			{
				transform.position = Vector3.Lerp(startPosition.position, endPosition.position, linearMapping.value);
			}
		}
	}
}