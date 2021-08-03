using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [Tooltip("Sensitivity multiplier for moving the camera around")]
    public float lookSensitivity = 1f;


    [Tooltip("Additional sensitivity multiplier for WebGL")]
    public float webglLookSensitivityMultiplier = 0.25f;


    [Tooltip("Limit to consider an input when using a trigger on a controller")]
    public float triggerAxisThreshold = 0.4f;

    PlayerCharacterController m_PlayerCharacterController;
    bool m_FireInputWasHeld;

    private void Start()
    {
        m_PlayerCharacterController = GetComponent<PlayerCharacterController>();

        /*
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        */
    }

    private void LateUpdate()
    {
        m_FireInputWasHeld = GetFireInputHeld();
    }

    public bool CanProcessInput()
    {
        return true; // TODO
        return Cursor.lockState == CursorLockMode.Locked;
    }

    public Vector3 GetMoveInput()
    {
        if(CanProcessInput())
        {
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

            // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
            move = Vector3.ClampMagnitude(move, 1);

            return move;
        }

        return Vector3.zero;
    }

    public float GetLookInputsHorizontal()
    {
        return GetMouseOrStickLookAxis("Mouse X");
    }

    public float GetLookInputsVertical()
    {
        return GetMouseOrStickLookAxis("Mouse Y");
    }

    public bool GetJumpInputDown()
    {
        if (CanProcessInput())
        {
            return Input.GetButtonDown("Jump");
        }

        return false;
    }

    public bool GetJumpInputHeld()
    {
        if (CanProcessInput())
        {
            return Input.GetButton("Jump");
        }

        return false;
    }

    public bool GetFireInputDown()
    {
        return GetFireInputHeld() && !m_FireInputWasHeld;
    }

    public bool GetFireInputReleased()
    {
        return !GetFireInputHeld() && m_FireInputWasHeld;
    }

    public bool GetFireInputHeld()
    {
        if (CanProcessInput())
        {
            bool isGamepad = Input.GetAxis("Fire1") != 0f;
            if (isGamepad)
            {
                return Input.GetAxis("Fire1") >= triggerAxisThreshold;
            }
            else
            {
                return Input.GetButton("Fire1");
            }
        }

        return false;
    }

    public bool GetAimInputHeld()
    {
        if (CanProcessInput())
        {
            bool isGamepad = Input.GetAxis("Gamepad Aim") != 0f;
            bool i = isGamepad ? (Input.GetAxis("Gamepad Aim") > 0f) : Input.GetButton("Aim");
            return i;
        }

        return false;
    }

    public bool GetSprintInputHeld()
    {
        if (CanProcessInput())
        {
            return Input.GetButton("Sprint");
        }

        return false;
    }

    public bool GetCrouchInputDown()
    {
        if (CanProcessInput())
        {
            return Input.GetButtonDown("Crouch");
        }

        return false;
    }

    public bool GetCrouchInputReleased()
    {
        if (CanProcessInput())
        {
            return Input.GetButtonUp("Crouch");
        }

        return false;
    }

    float GetMouseOrStickLookAxis(string mouseInputName)
    {
        if (CanProcessInput())
        {
            float i = Input.GetAxisRaw(mouseInputName);

            // apply sensitivity multiplier
            i *= lookSensitivity;

        
            // reduce mouse input amount to be equivalent to stick movement
            i *= 0.01f;
#if UNITY_WEBGL
            // Mouse tends to be even more sensitive in WebGL due to mouse acceleration, so reduce it even more
            i *= webglLookSensitivityMultiplier;
#endif

            return i;
        }

        return 0f;
    }
}
