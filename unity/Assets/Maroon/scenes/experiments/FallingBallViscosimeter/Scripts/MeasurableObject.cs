using UnityEngine;

namespace Maroon.Physics.Viscosimeter
{
  
    public class MeasurableObject : MonoBehaviour
    {
        private bool clickable = false;
        public GameObject measurement_device;
        public float device_rotation = 0;
    
    
        public void SetChooseable(bool chooseable)
        {
            clickable = chooseable;
            SetDragDrop(!chooseable);
        }
    
        private void SetDragDrop(bool active)
        {
            DragDrop dragDrop = gameObject.GetComponent<DragDrop>();
            if (dragDrop)
            {
                dragDrop.dragAndDropEnabled = active;
                Debug.Log(dragDrop.dragAndDropEnabled);
            }
        }

        private void OnMouseDown()
        {
            if (!clickable)
            {
                return;
            }
      
            MeasurementManager.Instance.SetChosenObject(this);
        }
    }

    public enum Axis
    {
        X,
        Y,
        Z
    }
}