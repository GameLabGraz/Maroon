using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics
{
  public class Scale : MonoBehaviour, IResetObject
  {

    [SerializeField]
    private IWeighableObject object_on_scale;
    private Vector3 on_scale_position;

    [SerializeField]
    private float scale_position_offset;



    void IResetObject.ResetObject()
    {

    }

    void OnDrawGizmos() {
      Gizmos.DrawIcon(on_scale_position, "Light Gizmo.tiff", true);
    }

    // Start is called before the first frame update
    void Start()
    {
      on_scale_position = transform.position + Vector3.up * scale_position_offset;
    }

    // Update is called once per frame
    void Update()
    {

    }


  }
}

