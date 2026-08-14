using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitSetup : MonoBehaviour
{
    [System.Serializable]
    public class CircuitConnection
    {
        public Output output;
        public InputS input;
    }

    public CircuitConnection[] connections;

    public void ConnectBlocks()
    {
        Output[] outputs = GetComponentsInChildren<Output>(true);

        for (int i = 0; i < outputs.Length; i++)
        {
            outputs[i].Connected = false;
            outputs[i].ConnectedTo = null;
        }

        InputS[] inputs = GetComponentsInChildren<InputS>(true);

        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i].Connected = false;
            inputs[i].ConnectedTo = null;
            inputs[i].InputValue = 0;
        }

        for (int i = 0; i < connections.Length; i++)
        {
            Output output = connections[i].output;
            InputS input = connections[i].input;

            output.Connected = true;
            output.ConnectedTo = input.gameObject;

            input.Connected = true;
            input.ConnectedTo = output.gameObject;
        }
    }
}