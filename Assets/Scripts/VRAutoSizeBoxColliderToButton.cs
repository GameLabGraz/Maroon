using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
[RequireComponent(typeof(BoxCollider))]

public class VRAutoSizeBoxColliderToButton : MonoBehaviour {

    [Tooltip("Check this, if size of the Button changes dynamically during Runtime")]
    [SerializeField] private bool m_ButtonStatic = true;


    private BoxCollider m_Collider;

    private void Start()
    {
        #if !UNITY_EDITOR
        if (m_ButtonStatic) Destroy(this);
        #endif
        m_Collider = GetComponent<BoxCollider>();
        m_Collider.size = new Vector3(((RectTransform)this.transform).sizeDelta.x, ((RectTransform)this.transform).sizeDelta.y, 0.005f);
        m_Collider.center = new Vector3(0f, 0f, 0f);
    }

    // Use this for initialization

    void Update () {
        #if UNITY_EDITOR
        if (!m_ButtonStatic) return;
        #endif
        m_Collider.size = new Vector3(((RectTransform)this.transform).sizeDelta.x, ((RectTransform)this.transform).sizeDelta.y, 0.005f);
        //m_Collider.center = new Vector3(0f, 0f, 0f);
    }
}
