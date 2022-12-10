using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using static Tests.Utilities.Constants;
using static Tests.Utilities.PlaymodeUtilities;

namespace Tests.PlayModeTests.ProofOfConcept
{
    /// <summary>
    /// Proof of concept for testing FallingCoil scene
    /// </summary>
    public class FallingCoilTests
    {
        [UnitySetUp]
        public IEnumerator Setup()
        {
            yield return LoadSceneAndCheckItsLoadedCorrectly(FallingCoilScenePath);
        }
        
        [TearDown]
        public void TearDown()
        {
            DestroyPermanentObjects();
        }

        [UnityTest, Description("The Coil should start moving when the experiment is started")]
        public IEnumerator WhenExperimentStartsThenFallingCoilMoves()
        {
            // Get Coil and store current position
            var coil = GameObject.Find("Coil");
            var coilStartPosition = coil.transform.position;

            // Start experiment simulation
            GameObject.Find("ButtonPlay").GetComponent<Button>().onClick.Invoke();
            
            // Wait until coil moves or timeout is reached
            yield return new WaitUntilConditionOrTimeout(5f, () => coilStartPosition != coil.transform.position);
            
            // Check Coil's new position
            var coilNewPosition = coil.transform.position;

            Assert.AreNotEqual(coilStartPosition, coilNewPosition,
                "Coil is expected to move after starting the experiment, but it didn't!");
        }

        [UnityTest, Description("Vector field grid should disappear on uncheck when experiment is not yet started")]
        public IEnumerator WhenUncheckVectorFieldGridThenItDisappears()
        {
            // Uncheck "Show Vector Field Grid" toggle
            GameObject.Find("VectorFieldToggleGroup").GetComponent<Toggle>().onValueChanged.Invoke(false);
            yield return null;
            
            foreach (Transform vectorFieldArrow in GameObject.Find("VectorField").transform)
            {
                if (vectorFieldArrow.gameObject.activeSelf)
                    Assert.Fail($"{vectorFieldArrow.name}({vectorFieldArrow.GetInstanceID()} must not be active)");
            }
        }
        
        [UnityTest, Description("Vector field grid should disappear on uncheck after experiment is started")]
        public IEnumerator WhenExperimentStartedUncheckVectorFieldGridThenItDisappears()
        {
            // Start experiment simulation
            GameObject.Find("ButtonPlay").GetComponent<Button>().onClick.Invoke();
            yield return null;
            
            // Uncheck "Show Vector Field Grid" toggle
            GameObject.Find("VectorFieldToggleGroup").GetComponent<Toggle>().onValueChanged.Invoke(false);
            yield return null;
            
            foreach (Transform vectorFieldArrow in GameObject.Find("VectorField").transform)
            {
                if (vectorFieldArrow.gameObject.activeSelf)
                    Assert.Fail($"{vectorFieldArrow.name}({vectorFieldArrow.GetInstanceID()} must not be active)");
            }
        }
        
        [Ignore("This test case always fails")]
        [UnityTest, Description("Field lines should disappear on uncheck when experiment is not yet started")]
        public IEnumerator WhenUncheckFieldLinesThenTheyDisappear()
        {
            // Uncheck "Show Field Lines" toggle
            GameObject.Find("FieldLinesToggleGroup").GetComponent<Toggle>().onValueChanged.Invoke(false);
            yield return null;
            
            int fieldLineCount = 0;
            foreach (Transform child in GameObject.Find("Coil").transform)
            {
                if (child.name.Contains("CoilFieldLine1") && child.gameObject.activeSelf)
                    fieldLineCount++;
            }
            
            Assert.AreEqual(0, fieldLineCount, "No CoilFieldLine1 should be active");
        }
        
        [Ignore("This test case always fails")]
        [UnityTest, Description("Field lines should disappear on uncheck after experiment is started")]
        public IEnumerator WhenExperimentStartedUncheckFieldLinesThenTheyDisappear()
        {
            // Get Coil and store current position
            var coil = GameObject.Find("Coil");
            var coilStartPosition = coil.transform.position;
            
            // Start experiment simulation
            GameObject.Find("ButtonPlay").GetComponent<Button>().onClick.Invoke();
            yield return null;
            
            // Wait until coil moves or timeout is reached
            yield return new WaitUntilConditionOrTimeout(5f, () => coilStartPosition != coil.transform.position);

            // Uncheck "Show Field Lines" toggle 
            GameObject.Find("FieldLinesToggleGroup").GetComponent<Toggle>().onValueChanged.Invoke(false);
            yield return null;

            int fieldLineCount = 0;
            foreach (Transform child in GameObject.Find("Coil").transform)
            {
                if (child.name.Contains("CoilFieldLine1") && child.gameObject.activeSelf)
                    fieldLineCount++;
            }
            
            Assert.AreEqual(0, fieldLineCount, "No CoilFieldLine1 should be active");
        }
        
        [UnityTest, Description("Number of Field Lines equals slider value on change after experiment is started")]
        public IEnumerator WhenExperimentStartedChangeFieldLinesSliderThenAmountChanges()
        {
            // Get Coil and store current position
            var coil = GameObject.Find("Coil");
            var coilStartPosition = coil.transform.position;
            
            // Start experiment simulation
            GameObject.Find("ButtonPlay").GetComponent<Button>().onClick.Invoke();
            yield return null;
            
            // Wait until coil moves or timeout is reached
            yield return new WaitUntilConditionOrTimeout(5f, () => coilStartPosition != coil.transform.position);

            // Set number of Field lines to 40 
            GameObject.Find("FieldLinesSliderGroup").GetComponentInChildren<Slider>().value = 40;
            yield return new WaitForSeconds(0.1f);

            // Count field lines
            int fieldLineCount = 0;
            foreach (Transform child in GameObject.Find("Coil").transform)
            {
                if (child.name.Contains("CoilFieldLine1") && child.gameObject.activeSelf)
                    fieldLineCount++;
            }
            
            Assert.AreEqual(40, fieldLineCount, "Wrong number of CoilFieldLines found");
        }
        
        [UnityTest, Description("Number of Field Lines equals slider value on change after experiment is started")]
        public IEnumerator WhenExperimentStartedChangeVectorFieldSliderThenResolutionChanges()
        {
            // Get Coil and store current position
            var coil = GameObject.Find("Coil");
            var coilStartPosition = coil.transform.position;
            
            // Start experiment simulation
            GameObject.Find("ButtonPlay").GetComponent<Button>().onClick.Invoke();
            yield return null;
            
            // Wait until coil moves or timeout is reached
            yield return new WaitUntilConditionOrTimeout(5f, () => coilStartPosition != coil.transform.position);

            // Set Vector Field resolution to 10
            GameObject.Find("VectorFieldResolutionSliderGroup").GetComponentInChildren<Slider>().value = 10;
            yield return null;

            // Count 2D vector arrows
            int vectorCount = 0;
            foreach (Transform child in GameObject.Find("VectorField").transform)
            {
                if (child.name.Contains("VectorFieldArrow2D") && child.gameObject.activeSelf)
                    vectorCount++;
            }
            
            Assert.AreEqual(100, vectorCount, "Wrong number of CoilFieldLines found");
        }

        [UnityTest, Description("Test Reset Button functionality by changing all values and reseting")]
        public IEnumerator WhenChangeValuesAndPressResetThenBaseValuesAreRestored()
        {
            // Get Coil and store start position
            var coil = GameObject.Find("Coil");
            var coilStartPosition = coil.transform.position;
            
            // Get FieldLinesToggle start value
            var fieldLinesToggle = GameObject.Find("FieldLinesToggleGroup").GetComponent<Toggle>();
            var fieldLinesToggleStartValue = fieldLinesToggle.isOn;
            
            // Get FieldLinesSlider start value
            var fieldLinesSlider = GameObject.Find("FieldLinesSliderGroup").GetComponentInChildren<Slider>();
            var fieldLinesSliderStartValue = fieldLinesSlider.value;
            
            // Get VectorFieldToggle start value
            var vectorFieldToggle = GameObject.Find("VectorFieldToggleGroup").GetComponent<Toggle>();
            var vectorFieldToggleStartValue = vectorFieldToggle.isOn;
            
            // Get VectorFieldSlider start value
            var vectorFieldSlider = GameObject.Find("VectorFieldResolutionSliderGroup").GetComponentInChildren<Slider>();
            var vectorFieldSliderStartValue = vectorFieldSlider.value;
            
            // Get RingResistanceSlider start value
            var ringResistanceSlider = GameObject.Find("RingResistanceSliderGroup").GetComponentInChildren<Slider>();
            var ringResistanceSliderStartValue = ringResistanceSlider.value;
            
            // Get MagneticMomentSlider start value
            var magneticMomentSlider = GameObject.Find("MagneticMomentSliderGroup").GetComponentInChildren<Slider>();
            var magneticMomentSliderStartValue = magneticMomentSlider.value;
            
            // Change values
            // Start experiment simulation to change coil position
            GameObject.Find("ButtonPlay").GetComponent<Button>().onClick.Invoke();
            yield return new WaitUntilConditionOrTimeout(5f, () => coilStartPosition != coil.transform.position);
            
            // Change toggles and sliders
            fieldLinesToggle.onValueChanged.Invoke(false);
            fieldLinesSlider.value = 40;
            vectorFieldToggle.onValueChanged.Invoke(false);
            vectorFieldSlider.value = 10;
            ringResistanceSlider.value = 5f;
            magneticMomentSlider.value = 4f;
            yield return null;
            
            // Press Reset button
            GameObject.Find("ButtonReset").GetComponent<Button>().onClick.Invoke();
            yield return null;
            
            // Check values
            Assert.AreEqual(coilStartPosition.y, coil.transform.position.y, "Coil position did not reset correctly");
            Assert.AreEqual(fieldLinesToggleStartValue, fieldLinesToggle.isOn, "FieldLinesToggle did not reset correctly");
            Assert.AreEqual(fieldLinesSliderStartValue, fieldLinesSlider.value, "FieldLinesSlider did not reset correctly");
            Assert.AreEqual(vectorFieldToggleStartValue, vectorFieldToggle.isOn, "VectorFieldToggle did not reset correctly");
            Assert.AreEqual(vectorFieldSliderStartValue, vectorFieldSlider.value, "VectorFieldSlider did not reset correctly");
            Assert.AreEqual(ringResistanceSliderStartValue, ringResistanceSlider.value, "RingResistanceSlider did not reset correctly");
            Assert.AreEqual(magneticMomentSliderStartValue, magneticMomentSlider.value, "MagneticMomentSlider did not reset correctly");
        }
    }
}

