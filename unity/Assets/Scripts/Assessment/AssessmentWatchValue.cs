using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Maroon.Physics;
using UnityEngine.Events;

namespace Maroon.Assessment
{
    class UnityQuantityEvent<T> : UnityEvent<Quantity<T>> { }
    
    [RequireComponent(typeof(AssessmentObject))]
    public class AssessmentWatchValue : MonoBehaviour
    {
        [SerializeField]
        private Component component;

        [SerializeField]
        private string attribute;

        [SerializeField]
        [Tooltip("If the value is dynamic, then the value gets send in each update to the assessment system, otherwise just changes are reported.")]
        private bool isDynamic;


        private List<string> _attributeComponents;

        public virtual string Name => gameObject.name;

        public string ObjectID => $"{gameObject.name}{gameObject.GetInstanceID()}";

        public string PropertyName => string.Join(".", _attributeComponents);

        public bool IsDynamic
        {
            get => isDynamic;
            set => isDynamic = value;
        }

        private void Awake()
        {
            _attributeComponents = new List<string>(attribute.Split('.'));
        }

        public object GetValue()
        {
            object obj = component;
            foreach (var attributePart in _attributeComponents)
            {
                if (obj == null)
                    throw new ArgumentNullException($"{Name}.{attribute}","Unable to get value.");

                var memberInfo = obj.GetType().GetMember(attributePart).First();
                obj = memberInfo?.MemberType == MemberTypes.Property ? 
                    obj.GetType().GetProperty(attributePart)?.GetValue(obj) : 
                    obj.GetType().GetField(attributePart)?.GetValue(obj);
            }

            switch (obj)
            {
                case IQuantity quantity:
                    return quantity.GetValue();
                default:
                    return obj;
            }
        }
    }
}

