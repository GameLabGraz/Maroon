using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using static Tests.Utilities.Constants;
using static Tests.Utilities.UtilityFunctions;

namespace Tests.PlayModeTests.ProofOfConcept
{
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

        [UnityTest]
        public IEnumerator WhenExperimentStartsThenFallingCoilMoves()
        {
            // Get Coil and store current position
            var coil = GameObject.Find("Coil");
            var coilStartPosition = coil.transform.position;

            // Start experiment simulation
            GameObject.Find("ButtonPlay").GetComponent<Button>().onClick.Invoke();
            yield return new WaitForDone(5f, () => coilStartPosition != coil.transform.position);
            
            // Check Coil's new position
            var coilNewPosition = coil.transform.position;

            Assert.AreNotEqual(coilStartPosition, coilNewPosition,
                "Coil is expected to move after starting the experiment, but it didn't!");
        }
    }
}

public sealed class WaitForDone : CustomYieldInstruction
{
    private Func<bool> m_Predicate;
    private float m_timeout;
    private bool WaitForDoneProcess()
    {
        m_timeout -= Time.deltaTime;
        if (m_timeout <= 0f)
            Debug.LogError("Timeout reached!");
        return m_timeout <= 0f || m_Predicate();
    }
 
    public override bool keepWaiting => !WaitForDoneProcess();
 
    public WaitForDone(float timeout, Func<bool> predicate)
    {
        m_Predicate = predicate;
        m_timeout = timeout;
    }
}
