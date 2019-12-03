using UnityEngine;

public class RulerLine : MonoBehaviour
{

    Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        rend.material.mainTextureScale = new Vector2(this.gameObject.transform.lossyScale.x, 1);

        Vector3 temp = this.gameObject.transform.localScale;
        temp.x += 0.01f;
        this.gameObject.transform.localScale = temp;
    }
}
