using System;
using System.Collections.Generic;
using System.Linq;
using Antares.Evaluation;
using Maroon.Assessment.Handler;
using Maroon.Physics;
using UnityEngine;

namespace Maroon.Assessment
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
    
    [Serializable]
    public class DataUpdateMessage
    {
        public string Identifier = "";
        public bool IsVector3 = false;
        public Vector3 vec3Value = Vector3.zero;
        public bool IsFloat = false;
        public float fValue = 0f;
        public bool IsInteger = false;
        public int nValue = 0;
        public bool isBool = false;
        public bool bValue = false;
        public bool isString = false;
        public string strValue = "";

        public DataUpdate getDataUpdate()
        {
            if (IsVector3) return new DataUpdate(DataUpdateType.SetValue, Identifier, vec3Value);
            if(IsFloat) return new DataUpdate(DataUpdateType.SetValue, Identifier, fValue);
            if(IsInteger) return new DataUpdate(DataUpdateType.SetValue, Identifier, nValue);
            if(isBool) return new DataUpdate(DataUpdateType.SetValue, Identifier, bValue);
            if(isString) return new DataUpdate(DataUpdateType.SetValue, Identifier, strValue);
            return null;
        }
    }
    
    public class AssessmentSystemTester : MonoBehaviour
    {
        [Header("Quantity Testing")]
        public QuantityReferenceValue quantityReference;

        public Vector3Test testVector3;
        public FloatTest testFloat;
        public IntTest testInteger;
        public BoolTest testBool;
        public StringTest testString;

        [Header("Message Testing")] 
        public AssessmentFeedbackHandler FeedbackHandler;
        [Tooltip("Set this to null if you want to test a create message!")]
        public AssessmentObject assessmentObj = null;
        public ObjectUpdateType messageType = ObjectUpdateType.Update;
        public List<DataUpdateMessage> messageData = new List<DataUpdateMessage>();
        public bool sendMessageNow = false;
        
        void Update()
        {
            if (sendMessageNow)
            {
                SendMessage();
                sendMessageNow = false;
            }
            
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


        private void SendMessage()
        {
            if (!FeedbackHandler) return;
            var manipulateObject = new ManipulateObject(DateTime.Now, messageType, 
                assessmentObj == null? "" : assessmentObj.ObjectID, messageData.Select(md => md.getDataUpdate()).ToArray());

            FeedbackHandler.HandleFeedback(new FeedbackEventArgs(new List<FeedbackCommand> {manipulateObject}.ToArray()));
        }
    }
}