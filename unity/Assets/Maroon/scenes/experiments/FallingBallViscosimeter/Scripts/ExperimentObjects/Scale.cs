using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Maroon.Physics
{
  public class Scale : MonoBehaviour, IResetObject
  {
    private SnapPoint snapPoint;
    private Vector3 on_scale_position;

    [SerializeField]
    private float scale_position_offset;
    private TMP_Text scale_text;

    private static string scale_text_format = "000.0000";



    void IResetObject.ResetObject()
    {

    }

    void OnDrawGizmos() {
      Gizmos.DrawIcon(on_scale_position, "Light Gizmo.tiff", true);
    }

    // Start is called before the first frame update
    void Awake()
    {
      on_scale_position = transform.position + Vector3.up * scale_position_offset;
      scale_text = GetComponentInChildren<TMP_Text>();
      snapPoint = GetComponentInChildren<SnapPoint>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
      updateText();
    }

    void updateText()
    {
      if(snapPoint.currentObject == null)
      {
        scale_text.SetText(0.0f.ToString(scale_text_format) + " g");
      }
      else
      {
        WeighableObject weighable_object = snapPoint.currentObject.gameObject.GetComponent<WeighableObject>();
        scale_text.SetText((weighable_object.getWeight() * 1000.0m).ToString(scale_text_format) + " g");
      }
    }
  }
}

