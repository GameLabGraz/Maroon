using System;
using Maroon.Physics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assessment
{
    [Serializable]
    public class TestClass<T>
    {
        public bool testNow = false;
        public T NewValue;
    }

    [Serializable] public class Vector3Test : TestClass<Vector3> {}
    [Serializable] public class FloatTest : TestClass<float> {}
    [Serializable] public class IntTest : TestClass<int> {}
    [Serializable] public class BoolTest : TestClass<bool> {}
    [Serializable] public class StringTest : TestClass<string> {}
    
    public class AssessmentSystemTester : MonoBehaviour
    {
        public QuantityReferenceValue quantityReference;

        public Vector3Test testVector3;
        public FloatTest testFloat;
        public IntTest testInteger;
        public BoolTest testBool;
        public StringTest testString;
        
        // Start is called before the first frame update
   
        // Update is called once per frame
        void Update()
        {
            var testingQuantity = quantityReference.Value;
            if (testingQuantity == null) return;
            
            if (testVector3.testNow)
            {
                if (testingQuantity is QuantityVector3 vecQuantity)
                    vecQuantity.SystemSetsQuantity(testVector3.NewValue);
                else
                    Debug.LogError("Tried to test a Vector with an IQuantity that is no QuantityVector3.");
            
                testVector3.testNow = false;
            }

            if (testFloat.testNow)
            {
                if (testingQuantity is QuantityFloat fQuantity)
                    fQuantity.SystemSetsQuantity(testFloat.NewValue);
                else
                    Debug.LogError("Tried to test a Vector with an IQuantity that is no QuantityVector3.");
            
                testFloat.testNow = false;
            }
        
            if (testBool.testNow)
            {
                if (testingQuantity is QuantityBool bQuantity)
                    bQuantity.SystemSetsQuantity(testBool.NewValue);
                else
                    Debug.LogError("Tried to test a Vector with an IQuantity that is no QuantityVector3.");
            
                testBool.testNow = false;
            }
        
            if (testInteger.testNow)
            {
                if (testingQuantity is QuantityInt nQuantity)
                    nQuantity.SystemSetsQuantity(testInteger.NewValue);
                else
                    Debug.LogError("Tried to test a Vector with an IQuantity that is no QuantityVector3.");
            
                testInteger.testNow = false;
            }
        
            if (testString.testNow)
            {
                if (testingQuantity is QuantityString sQuantity)
                    sQuantity.SystemSetsQuantity(testString.NewValue);
                else
                    Debug.LogError("Tried to test a Vector with an IQuantity that is no QuantityVector3.");
            
                testString.testNow = false;
            }
        }
    }
}