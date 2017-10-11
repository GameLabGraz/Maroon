using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistanceBallon : MonoBehaviour
{


    public GameObject sparkingStartPoint;
    public GameObject sparkingEndPoint;
    public VandeGraaffController vandeGraaffController;
    public float normalizationFactor = 0.476f;
    public Text text_distance;



  public void Update()
    {
        text_distance.text = GamificationManager.instance.l_manager.GetString("Distance GUI") + GetDistanceBetweenSparkingPoints();      
    }

    public float GetDistanceBetweenSparkingPoints()
    {
        return Vector3.Distance(sparkingStartPoint.transform.position, sparkingEndPoint.transform.position) * normalizationFactor;
    }


}