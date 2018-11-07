//-----------------------------------------------------------------------------
// ArrowController.cs
//
// Controller class for field arrows
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine.Jobs;
using Unity.Collections;

/// <summary>
/// Controller class for field arrows
/// </summary>
public class ArrowController : MonoBehaviour, IResetObject
{
    /// <summary>
    /// The physical field
    /// </summary>
    public GameObject Field;

    /// <summary>
    /// The rotation offset
    /// </summary>
    public float rotationOffset = 0f;

    /// <summary>
    /// The field strength factor
    /// </summary>
    public float fieldStrengthFactor = Teal.FieldStrengthFactor;

    /// <summary>
    /// The start rotation for reseting
    /// </summary>
    public Quaternion start_rot;

    private SimulationController simController;

    /// <summary>
    /// Initialization
    /// </summary>
   /* void Start()
    {
        GameObject simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();

        Field = GameObject.FindGameObjectWithTag("Field");
        if (Field != null)
            rotateArrow();*//*
        start_rot = transform.rotation;

    }*/

    /// <summary>
    /// Update is called every frame and rotates the arrow
    /// </summary>
    /*void Update()
    {
        if (!simController.SimulationRunning)
            return;

        if (Field != null)
            rotateArrow();
        else
            Field = GameObject.FindGameObjectWithTag("Field");
    }*/

    /// <summary>
    /// Rotates the arrow based on the field
    /// </summary>
    public void rotateArrow()
    {
        Vector3 rotate = Field.GetComponent<IField>().get(transform.position) * fieldStrengthFactor;
        rotate.Normalize();

        float rot = 0;

        if(transform.parent != null)
        {
            if(transform.parent.rotation == Quaternion.Euler(-90, 0, 0))
                rot = Mathf.Atan2(rotate.x, rotate.y) * Mathf.Rad2Deg;
            else if (transform.parent.rotation == Quaternion.Euler(90, 0, 0))
                rot = -Mathf.Atan2(rotate.x, rotate.y) * Mathf.Rad2Deg;

            else if (transform.parent.rotation == Quaternion.Euler(90, 90, 0))
                rot = Mathf.Atan2(rotate.z, rotate.y) * Mathf.Rad2Deg;
            else if (transform.parent.rotation == Quaternion.Euler(90, -90, 0))
                rot = -Mathf.Atan2(rotate.z, rotate.y) * Mathf.Rad2Deg;

            else if (transform.parent.rotation == Quaternion.Euler(-90, 90, 0))
                rot = -Mathf.Atan2(rotate.z, rotate.y) * Mathf.Rad2Deg;
            else if (transform.parent.rotation == Quaternion.Euler(-90, -90, 0))
                rot = Mathf.Atan2(rotate.z, rotate.y) * Mathf.Rad2Deg;

            else if (transform.parent.rotation == Quaternion.Euler(0, 0, 0))
                rot = Mathf.Atan2(rotate.x, rotate.z) * Mathf.Rad2Deg;
        }

        transform.localRotation = Quaternion.Euler(-90, rot, 0);
    }

    /// <summary>
    /// Resets the object
    /// </summary>
    public void resetObject()
    {
        //rotateArrow();
    }
}

class ArrowSystem : ComponentSystem
{
    struct Arrow
    {
        public ArrowController arrowController;
        public Transform transform;
    }
    struct VectorFieldSystem
    {
        public VectorField vectorField;
        public Transform transform;
    }
    private static SimulationController simController;
    private float rot = 0.0f;
    private Transform parent;
    private IField Field;
    private const bool useJobSystem = true;



    //For Job System:
    private TurnArrowJob turnArrowJob;

    protected override void OnUpdate()
    {
        if (simController == null)
        {
            if (Field == null)
            {
                if (GetEntities<VectorFieldSystem>().Length == 0)
                    return;
                Field = GameObject.FindGameObjectWithTag("Field").GetComponent<IField>();
                return;
            }
            GameObject simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();
        }
        else if (!simController.SimulationRunning) return; //Arrows should be aligned once, also if the simulation is not running while Startup.


        int rot_case = 0;
        if (!useJobSystem)
        {
            parent = null; //Reset parent to determine once in the Update process if parent have moved
            foreach (var Arrow in GetEntities<Arrow>())
            {
                Vector3 rotate = Field.get(Arrow.transform.position) * Arrow.arrowController.fieldStrengthFactor;
                rotate.Normalize();

                if (Arrow.transform.parent != parent)
                {
                    parent = Arrow.transform.parent;
                    if (parent != null)
                    {
                        if (parent.rotation == Quaternion.Euler(-90, 0, 0))
                            rot_case = 1;
                        else if (parent.rotation == Quaternion.Euler(90, 0, 0))
                            rot_case = 2;
                        else if (parent.rotation == Quaternion.Euler(90, 90, 0))
                            rot_case = 3;
                        else if (parent.rotation == Quaternion.Euler(90, -90, 0))
                            rot_case = 4;
                        else if (parent.rotation == Quaternion.Euler(-90, 90, 0))
                            rot_case = 4;
                        else if (parent.rotation == Quaternion.Euler(-90, -90, 0))
                            rot_case = 3;

                        else if (parent.rotation == Quaternion.Euler(0, 0, 0))
                            rot_case = 5;
                    }
                    else
                        rot_case = 0;
                }
                switch (rot_case)
                {
                    case 0:
                        Debug.LogWarning("[ArrowController:] No parent found!");
                        break;
                    case 1:
                        rot = Mathf.Atan2(rotate.x, rotate.y) * Mathf.Rad2Deg;
                        break;
                    case 2:
                        rot = -Mathf.Atan2(rotate.x, rotate.y) * Mathf.Rad2Deg;
                        break;
                    case 3:
                        rot = Mathf.Atan2(rotate.z, rotate.y) * Mathf.Rad2Deg;
                        break;
                    case 4:
                        rot = -Mathf.Atan2(rotate.z, rotate.y) * Mathf.Rad2Deg;
                        break;
                    case 5:
                        rot = Mathf.Atan2(rotate.x, rotate.z) * Mathf.Rad2Deg;
                        break;
                }



                Arrow.transform.localRotation = Quaternion.Euler(-90, rot, 0);
            }
        }
        else
        {
            foreach (var vectorField_ in GetEntities<VectorFieldSystem>())
            {
                vectorField_.vectorField.turnArrowHandle.Complete();
                for (int i = 0; i < vectorField_.vectorField.transforms.length; i++)
                {
                    vectorField_.vectorField.rotationArray[i] = Field.get((vectorField_.vectorField.transforms[i]).position);
                }
                    if (vectorField_.transform.rotation == Quaternion.Euler(-90, 0, 0))
                        rot_case = 1;
                    else if (vectorField_.transform.rotation == Quaternion.Euler(90, 0, 0))
                        rot_case = 2;
                    else if (vectorField_.transform.rotation == Quaternion.Euler(90, 90, 0))
                        rot_case = 3;
                    else if (vectorField_.transform.rotation == Quaternion.Euler(90, -90, 0))
                        rot_case = 4;
                    else if (vectorField_.transform.rotation == Quaternion.Euler(-90, 90, 0))
                        rot_case = 4;
                    else if (vectorField_.transform.rotation == Quaternion.Euler(-90, -90, 0))
                        rot_case = 3;

                    else if (vectorField_.transform.rotation == Quaternion.Euler(0, 0, 0))
                        rot_case = 5;


                turnArrowJob = new TurnArrowJob()
                {
                   rotationArray_ = vectorField_.vectorField.rotationArray,
                   fieldStrengthFactor_ = vectorField_.vectorField.getFieldStrenghFactor(),
                   rot_case_ = rot_case
                };
                vectorField_.vectorField.turnArrowHandle = turnArrowJob.Schedule(vectorField_.vectorField.transforms);
                Unity.Jobs.JobHandle.ScheduleBatchedJobs();

            }
            }

        }

    public static void resetSimController()
    {
        simController = null;
    }
}

public struct TurnArrowJob : IJobParallelForTransform
{
    [ReadOnly] public NativeArray<Vector3> rotationArray_;
    [ReadOnly] public float fieldStrengthFactor_;
    [ReadOnly] public int rot_case_;


    public void Execute(int index, TransformAccess transform)
    {
        Vector3 rotato = rotationArray_[index] * fieldStrengthFactor_;
        rotato.Normalize();
        switch (rot_case_)
        {
            case 1:
                transform.localRotation = Quaternion.Euler(-90, Mathf.Atan2(rotato.x, rotato.y) * Mathf.Rad2Deg, 0);
                break;
            case 2:
                transform.localRotation = Quaternion.Euler(-90, -Mathf.Atan2(rotato.x, rotato.y) * Mathf.Rad2Deg, 0);

                break;
            case 3:
                transform.localRotation = Quaternion.Euler(-90, Mathf.Atan2(rotato.z, rotato.y) * Mathf.Rad2Deg, 0);

                break;
            case 4:
                transform.localRotation = Quaternion.Euler(-90, -Mathf.Atan2(rotato.z, rotato.y) * Mathf.Rad2Deg, 0);

                break;
            case 5:
                transform.localRotation = Quaternion.Euler(-90, Mathf.Atan2(rotato.x, rotato.z) * Mathf.Rad2Deg, 0);
                break;
            default:
                Debug.LogWarning("[TurnArrowJob:] Something bad happened!");
                break;
        }



    }
}