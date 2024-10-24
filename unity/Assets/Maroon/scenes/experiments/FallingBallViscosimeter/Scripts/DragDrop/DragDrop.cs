using UnityEngine;

namespace Maroon.Physics.Viscosimeter
{
    public class DragDrop : MonoBehaviour
    {
        public delegate void DragEndedDelegate(DragDrop dragDrop);

        public DragEndedDelegate dragEndedSnapCallback;
        public bool dragAndDropEnabled = true;
        public bool snap = true;
        public bool offsetDragging = false;
        private Vector3 mouseOffset = Vector3.zero;
        public bool hoverSizeIncrease = true;
        public float hoverSizeFactor = 1.2f;
        public Vector3 planePosition;
        private Vector3 worldPosition;
        private Plane plane;
        private bool isDragged = false;
        public SnapPoint snapPoint = null;

        public bool axisLocked = false;
        public Axis axisLockedInto = Axis.X;

        public bool localPositionLimit = false;
        public float localPositionMax = 0.0f;
        public float localPositionMin = 0.0f;

        private PausableObject _pausableObject;
        private Vector3 oldScale;
        public Ball ball;
        
        private void Awake()
        {
            plane = new Plane(new Vector3(0, 0, -1), planePosition);
            _pausableObject = gameObject.GetComponent<PausableObject>();
            ball = gameObject.GetComponent<Ball>();
        }

        private void Update()
        {
            if (localPositionLimit && axisLocked)
            {
                Vector3 current_local_position = transform.localPosition;
                transform.localPosition = new Vector3(
                    Mathf.Clamp(transform.localPosition.x, localPositionMin, localPositionMax),
                    current_local_position.y, current_local_position.z);
            }
        }

        private void OnMouseDown()
        {
            if (!dragAndDropEnabled)
            {
                return;
            }

            isDragged = true;
            if (snap && snapPoint)
            {
                snapPoint.currentObject = null;
                snapPoint = null;
            }

            if (offsetDragging)
            {
                //get offset from center of object
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float distance;
                Vector3 mousePosition;
                if (plane.Raycast(ray, out distance))
                {
                    mousePosition = ray.GetPoint(distance);
                    mouseOffset = transform.position - mousePosition;
                }
            }
        }

        private void OnMouseDrag()
        {
            if (!dragAndDropEnabled)
            {
                return;
            }

            float distance;
            if (isDragged)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (plane.Raycast(ray, out distance))
                {
                    worldPosition = ray.GetPoint(distance) + mouseOffset;
                }

                if (axisLocked)
                {
                    switch (axisLockedInto)
                    {
                        case Axis.X:
                            transform.position = new Vector3(transform.position.x, worldPosition.y, worldPosition.z);
                            break;
                        case Axis.Y:
                            transform.position = new Vector3(worldPosition.x, transform.position.y, worldPosition.z);
                            break;
                        case Axis.Z:
                            transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
                            break;
                    }
                }
                else
                {
                    transform.position = worldPosition;
                }
            }
        }

        private void OnMouseUp()
        {
            if (!dragAndDropEnabled)
            {
                return;
            }

            isDragged = false;
            if (_pausableObject)
            {
                _pausableObject.GetComponent<RigidBodyStateControl>().StoreRigidBodyState();
            }

            if (snap)
            {
                dragEndedSnapCallback(this);
            }
        }

        private void OnMouseEnter()
        {
            if (hoverSizeIncrease && dragAndDropEnabled)
            {
                oldScale = transform.localScale;
                transform.localScale = oldScale * hoverSizeFactor;
            }
        }

        private void OnMouseOver()
        {
            if (hoverSizeIncrease && dragAndDropEnabled)
            {
                transform.localScale = oldScale * hoverSizeFactor;
            }
        }

        private void OnMouseExit()
        {
            if (hoverSizeIncrease && dragAndDropEnabled)
            {
                transform.localScale = oldScale;
            }
        }
    }
}