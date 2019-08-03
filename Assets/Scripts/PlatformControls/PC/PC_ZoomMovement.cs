using UnityEngine;

public class PC_ZoomMovement : MonoBehaviour, IResetWholeObject
{
    [Tooltip("The nearest position possible.")]
    public Transform nearPosition;
    [Tooltip("The furthest position possible.")]
    public Transform farPosition;
    [Tooltip("A speed multiplier that tells how fast one zooms.")]
    public float movementSpeed = 20f;

    [Tooltip("The default position to which the object is set once zooming is enabled.")]
    [Range(0f, 1f)]
    public float defaultPosition;

    [Header("Other Settings")] 
    [Tooltip("Tells if zoom via + and - buttons is enabled.")]
    public bool enableShortcuts = true;
    [Tooltip("Tells if zooming is currently enabled or not.")]
    public bool isEnabled = true;


    private Vector3 _positionVariable;
    
    // Start is called before the first frame update
    void Start()
    {
        if(isEnabled)
            SetToDefaultPosition();

        _positionVariable = (farPosition.position - nearPosition.position).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnabled) return;
        
        var mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetKey(KeyCode.Equals) // because of https://answers.unity.com/questions/249339/problem-with-keycodeplus.html
            || Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus))
            mouseScroll -= 0.1f;
        if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
            mouseScroll += 0.1f;

        var newPos = transform.position + mouseScroll * movementSpeed * _positionVariable;
        var near = nearPosition.position;
        var far = farPosition.position;
        newPos.x = Mathf.Clamp(newPos.x, Mathf.Min(near.x, far.x), Mathf.Max(near.x, far.x));
        newPos.y = Mathf.Clamp(newPos.y, Mathf.Min(near.y, far.y), Mathf.Max(near.y, far.y));
        newPos.z = Mathf.Clamp(newPos.z, Mathf.Min(near.z, far.z), Mathf.Max(near.z, far.z));

        transform.position = newPos;
    }

    public void EnableZooming(bool enabling)
    {
        isEnabled = enabling;
        if(isEnabled) SetToDefaultPosition();
    }

    private void SetToDefaultPosition()
    {
        transform.position = nearPosition.position +
                             (farPosition.position - nearPosition.position) * defaultPosition;
    }

    public void ResetObject()
    {
        
    }

    public void ResetWholeObject()
    {
        if(isEnabled) SetToDefaultPosition();
    }
}
