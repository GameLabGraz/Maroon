using System;
using System.Collections.Generic;
using Maroon.Physics;
using UnityEngine;

namespace Maroon.Assessment
{
    public enum AssessmentClass
    {
        StopWatch,
        Calculator,
        Pendulum,
        Voltmeter,
        Ruler,
        Charge,
        Display,
        Slide,
        Visualization,
        Logic
    }
    
    [Serializable]
    public class AssessmentObject : MonoBehaviour
    {
        [SerializeField]
        protected AssessmentClass classType;

        public AssessmentClass ClassType => classType;
        
        public List<IQuantity> WatchedValues = new List<IQuantity>();

        [SerializeField] private List<QuantityReferenceValue> watchValues = new List<QuantityReferenceValue>();

        public string ObjectID { get; protected set; }

        protected virtual void Awake()
        {
            ObjectID = $"{gameObject.name}{gameObject.GetInstanceID()}";
            
            foreach (var refValue in watchValues) // quantityCallbacks.Select(callback => callback.Invoke()).Where(quantity => quantity != null))
            {
                var quantity = refValue.Value;
                if (quantity == null)
                {
                    Debug.LogError("Reference Value is Null: " + refValue.objectInfo.ComponentName + "::" + refValue.objectInfo.FieldName);
                    throw new NullReferenceException();
                }
                
                WatchedValues.Add(quantity);
                
                (quantity as QuantityFloat)?.onValueChanged.AddListener((value) => ValueChangedForProperty<float>(value, quantity.GetName(), quantity.IsDynamic()));
                (quantity as QuantityVector3)?.onValueChanged.AddListener((value) => ValueChangedForProperty<Vector3>(value, quantity.GetName(), quantity.IsDynamic()));
                (quantity as QuantityBool)?.onValueChanged.AddListener((value) => ValueChangedForProperty<bool>(value, quantity.GetName(), quantity.IsDynamic()));
                (quantity as QuantityInt)?.onValueChanged.AddListener((value) => ValueChangedForProperty<int>(value, quantity.GetName(), quantity.IsDynamic()));
                (quantity as QuantityString)?.onValueChanged.AddListener((value) => ValueChangedForProperty<string>(value, quantity.GetName(), quantity.IsDynamic()));
            }
            AssessmentManager.Instance?.RegisterAssessmentObject(this);
        }

        private void ValueChangedForProperty<T>(T val, string name, bool isDynamic)
        {
            if (isDynamic) return;
            
            // Debug.Log(ObjectID + "::" + name + " = " + val);
            AssessmentManager.Instance?.SendDataUpdate(ObjectID, name, val);
        }

        public void SendUserAction(string actionName)
        {
            // Debug.Log(ObjectID + "::" + actionName);
            AssessmentManager.Instance?.SendUserAction(actionName, ObjectID);
        }
        
        protected virtual void OnDestroy()
        {
            AssessmentManager.Instance?.DeregisterAssessmentObject(this);
        }
    }
}
