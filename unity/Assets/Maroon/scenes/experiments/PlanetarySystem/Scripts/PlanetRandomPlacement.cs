using UnityEngine;

public class PlanetRandomPlacement : MonoBehaviour
{
    public float distanceBetweenSpheres = 2.0f;

    private void Start()
    {
        RandomizeSpheres();
    }

    private void RandomizeSpheres()
    {
        int childCount = transform.childCount;
        int[] indices = new int[childCount];
        for (int i = 0; i < childCount; i++) indices[i] = i;

        for (int i = 0; i < childCount; i++)
        {
            int randomIndex = Random.Range(i, childCount);
            int temp = indices[i];
            indices[i] = indices[randomIndex];
            indices[randomIndex] = temp;

            Vector3 newPos = new Vector3(transform.position.x + indices[i] * distanceBetweenSpheres, transform.position.y, transform.position.z);
            transform.GetChild(i).position = newPos;
        }
    }
}
