using UnityEngine;

public class OnMouseEvent : MonoBehaviour
{
    private Vector3 _initialScale;
    private readonly float _maxScale = 1.2f;
    private float scaleFactor;

    /**
     * Start is called before the first frame update
     * */
    private void Start()
    {
        _initialScale = transform.localScale;
        scaleFactor = 1.05f;
    }

    /**
     * on mouse click event listener on every island to then select it
     * */
    private void OnMouseDown()
    {
        string text = this.name;
        //Debug.Log("OnMouseDown " + text);
        StartCoroutine(MSTController.Instance.SelectIsland(text));
    }

    /**
     * on mouse over event listener on every island so it appears bigger
     * to show the user to click on it
     * */
    private void OnMouseOver()
    {
        if (scaleFactor < _maxScale)
        {
            transform.localScale = _initialScale * scaleFactor;
            scaleFactor += 0.05f;
        }

    }

    /**
     * on mouse exit event listener on every island to change it to its original scale
     * */
    private void OnMouseExit()
    {
        scaleFactor = 1.05f;
        transform.localScale = _initialScale;
    }

}