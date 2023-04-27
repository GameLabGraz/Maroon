using UnityEngine;

public class PlanetCheckOrder : MonoBehaviour
{
    public GameObject planets;
    public GameObject planetTargetLine;

    public void CheckCorrectOrder()
    {
        int correctCount = 0;
        int totalSpheres = planets.transform.childCount;

        for (int i = 0; i < totalSpheres; i++)
        {
            GameObject planet = planets.transform.GetChild(i).gameObject;
            GameObject placeholder = planetTargetLine.transform.GetChild(i).gameObject;

            if (Vector3.Distance(planet.transform.position, placeholder.transform.position) < 0.1f)
            {
                correctCount++;
            }
        }

        if (correctCount == totalSpheres)
        {
            Debug.Log("All spheres are in the correct order!");
            // Perform any actions when the order is correct, such as showing a message or playing a sound
        }
    }
}
