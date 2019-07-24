using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VoltageShaderHandler : MonoBehaviour, IResetObject
{
    private SimulationController simController;
    private CoulombLogic _coulombLogic;
    private static readonly int EntryCnt = Shader.PropertyToID("_EntryCnt");
    private static readonly int Entries = Shader.PropertyToID("_Entries");

    // Start is called before the first frame update
    void Start()
    {
        var simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();
        simControllerObject = GameObject.Find("CoulombLogic");
        if (simControllerObject)
            _coulombLogic = simControllerObject.GetComponent<CoulombLogic>();
        
        simController.AddNewResetObject(this);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf && simController.SimulationRunning)
            gameObject.SetActive(false);
    }

    public void DrawVoltage()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            return;
        }
            
        simController.SimulationRunning = false;
        var vecArray = new List<Vector4>();
        
        Debug.Log("ShaderArray: ");
        var particles = _coulombLogic.GetParticles(); // TODO: max 20 particles
        for (var i = 0; i < 20; ++i)
        {
            if (i >= particles.Count)
            { // since we cannot resize the vector array -> https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.SetVectorArray.html
                vecArray.Add(new Vector4(0f, 0f, 0f, 0f));
            }
            else
            {
                var particle = particles[i];
                var pos = particle.transform.position;
                var vec = new Vector4(pos.x, pos.y, pos.z, particle.charge);
                vecArray.Add(vec);
                Debug.Log("Add Particle to shader. " + vec);
            }
        }

        var mat = GetComponent<MeshRenderer>().sharedMaterial;
        mat.SetInt(EntryCnt, particles.Count);
        if (vecArray.Count > 0)
            mat.SetVectorArray(Entries, vecArray);

        gameObject.SetActive(true);
    }
    
    public void ResetObject()
    {        
        gameObject.SetActive(false);
    }
}
