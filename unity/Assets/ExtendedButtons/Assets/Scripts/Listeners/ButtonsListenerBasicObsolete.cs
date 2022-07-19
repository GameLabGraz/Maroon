using UnityEngine;
using UnityEngine.EventSystems;

namespace ExtendedButtons
{
    /// <summary>
    /// Shoud by only one on scene
    /// </summary>
    public class ButtonsListenerBasicObsolete : ButtonsListenerMono
    {
        /// <summary>
        /// listener needs camera to shot a ray from screen to point
        /// </summary>
        [SerializeField] private Camera button3DCamera;

        /// <summary>
        /// trashHold to detect if onClick event should be invoked
        /// if between onDown and onUp actions, pointer movement is bigger than
        /// moveTrashHold then onClick should be declined
        /// </summary>
        [SerializeField] private float moveTrashHold = 4.0f;

        /// <summary>
        /// how far ray should detect objects
        /// </summary>
        [SerializeField] private float maxDistance = 100.0f;

        /// <summary>
        /// which masks ray should detect
        /// </summary>
        [SerializeField] private LayerMask layerMask;

        /// <summary>
        /// which button3D is now detected
        /// </summary>
        private Button3D followedButton3D = null;

        /// <summary>
        /// remeber position of click down to calculate distance when click up
        /// </summary>
        private Vector3 firstInputPosition;

        /// <summary>
        /// flag to detect after click down and up if distance was bigger than trashHold
        /// </summary>
        private bool moved = false;

        /// <summary>
        /// button3D is locked when pointer is down
        /// </summary>
        private Button3D buttonLocked;

        private void Start()
        {
            if (button3DCamera == null)
                button3DCamera = Camera.main;
        }

        public override void Listener()
        {
            if (Input.GetMouseButtonUp(0) && buttonLocked != null)
            {
                buttonLocked.onUp?.Invoke();
                if (!moved)
                    buttonLocked.onClick?.Invoke();
                buttonLocked = null;
            }

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
            
            Ray ray = button3DCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
            {
                Button3D button = hit.transform.GetComponent<Button3D>();
                if (button != null)
                {
                    if (followedButton3D != button)
                    {
                        followedButton3D?.onExit?.Invoke();
                        followedButton3D = button;
                        followedButton3D?.onEnter?.Invoke();
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        moved = false;
                        firstInputPosition = Input.mousePosition;
                        button.onDown?.Invoke();
                        buttonLocked = button;
                    }

                    // button is pressed on Button3D, check if cursor is moved and cancel possibility to onClick if necessary
                    if (Input.GetMouseButton(0))
                    {
                        float movedDistance = Mathf.Abs(Vector3.Distance(firstInputPosition, Input.mousePosition));
                        if (!moved && movedDistance > moveTrashHold)
                        {
                            moved = true;
                        }
                    }
                }
                else // exit
                {
                    followedButton3D?.onExit?.Invoke();
                    followedButton3D = null;
                }
            }
            else
            {
                followedButton3D?.onExit?.Invoke();
                followedButton3D = null;
            }
        }        
    }
}
