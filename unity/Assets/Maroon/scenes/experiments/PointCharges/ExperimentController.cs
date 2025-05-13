
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

namespace PointChargeExperiment
{
    enum VectorFieldMode
    {
        XY_PLANE,
        YZ_PLANE,
        XZ_PLANE,
        CAMERA_PLANE,
        FULL,
    }
    
    enum ScalarDisplayMode
    {
        NONE,
        POTENTIAL,
        EFIELD_MAG
    }

    public class ExperimentController : MonoBehaviour
    {
        // Constants
        private const float sphereRadius = 0.1f;
        private const float distanceToPlane = sphereRadius / 2.0f;

        private bool in3DMode = false;

        // Drag and drop data
        [SerializeField]
        private GameObject highlightMarker = null;
        [SerializeField]
        private GameObject player = null; // Holds the camera

        [SerializeField]
        private UnityEngine.UI.GraphicRaycaster uiRaycaster = null;
        private bool dragActive = false;
        private GameObject selectedObject = null;
        private float dragObjectPlaneDistance = 1.0f;

        [SerializeField]
        private GameObject chargeContainer = null;
        [SerializeField]
        private GameObject arrowContainer = null;
        [SerializeField]
        private GameObject pointChargePrefab = null;

        [SerializeField]
        private GameObject backgroundObject2D = null;
        [SerializeField]
        private GameObject backgroundObject3D = null;

        // Vector field data
        [SerializeField]
        private GameObject arrowPrefab = null;
        private Mesh arrowMesh = null;
        private GameObject[] vectorFieldArrows = null;

        private bool vectorFieldEnabled = false;
        private int vectorFieldResolution = 5;
        private float vectorFieldArrowScale = 0.5f;
        private VectorFieldMode vectorFieldMode = VectorFieldMode.XY_PLANE;
        private float vectorField2DOffset = 0.0f;

        private ScalarDisplayMode colorMode = ScalarDisplayMode.NONE;
        private ScalarDisplayMode scaleMode = ScalarDisplayMode.NONE;
        private ScalarDisplayMode alphaMode = ScalarDisplayMode.NONE;

        public float MaxPotentialValue { get; set; } = 100;
        public float PotentialExpo { get; set; } = 1;
        public float MaxEFieldMagnitude { get; set; } = 100;
        public float EFieldExpo { get; set; } = 1;

        // UI Data
        [Header("UI-References")]
        public float uiChargeValue = 0.0f;
        public const float uiChargeMaxValue = 5.0f;
        [SerializeField]
        private TMPro.TMP_InputField chargeValueField = null;
        [SerializeField]
        private UnityEngine.UI.Slider chargeValueSlider = null;
        [SerializeField]
        private UnityEngine.UI.Image chargeMinusImage = null;
        [SerializeField]
        private UnityEngine.UI.Image chargePlusImage = null;
        [SerializeField]
        private UnityEngine.UI.Image chargeImage = null;
        [SerializeField]
        private TMPro.TMP_Dropdown modeDropdown = null;

        private Vector3 startPosition = new Vector3(0, 1.5f, 4);

        // Start is called before the first frame update
        void Start()
        {
            arrowMesh = Maroon.Experiments.CoulombsLaw.ArrowMeshCreator.CreateArrowMesh(0.2f, 1.3f, 0.5f, 12, 4);
            GenerateVectorFieldArrows();
        }

        private void GenerateVectorFieldArrows()
        {
            // Aliases
            int resolution = vectorFieldResolution;
            float arrowSizeScale = vectorFieldArrowScale;
            bool enabled = vectorFieldEnabled;

            // Early exit if vector field is disabled
            if (!enabled) {
                if (vectorFieldArrows != null)
                {
                    foreach (var arrow in vectorFieldArrows)
                    {
                        GameObject.Destroy(arrow);
                    }
                    vectorFieldArrows = null;
                }
                return;
            }

            bool vectorField2DPlacement = !in3DMode || vectorFieldMode != VectorFieldMode.FULL;
            int wantedArrowCount = resolution * resolution * (vectorField2DPlacement ? 1 : resolution);

            const float planeSideLength = 2.0f;
            float cellSideLength = planeSideLength / resolution;
            float arrowScale = cellSideLength / 2.0f * arrowSizeScale; // Arrow mesh is in range [-1, 1], so we need to divide by 2 to fit into cell
            Vector3 boxOrigin = arrowContainer.transform.position + new Vector3(-1, -1, -1);
            if (vectorFieldMode != VectorFieldMode.FULL)
            {
                boxOrigin.z = arrowContainer.transform.position.z + vectorField2DOffset;
            }
            if (!in3DMode)
            {
                boxOrigin.z = arrowContainer.transform.position.z - cellSideLength / 2.0f;
            }

            // Allocate Arrow Game-Objects
            if (vectorFieldArrows != null && vectorFieldArrows.Length != wantedArrowCount)
            {
                foreach (var arrow in vectorFieldArrows) {
                    GameObject.Destroy(arrow);
                }
                vectorFieldArrows = null;
            }
            if (vectorFieldArrows == null)
            {
                vectorFieldArrows = new GameObject[wantedArrowCount];
                for (int i = 0; i < vectorFieldArrows.Length; i++) 
                {
                    // Instanciate and initialize object
                    vectorFieldArrows[i] = Instantiate(arrowPrefab, arrowContainer.transform);
                    var arrow = vectorFieldArrows[i];
                    arrow.GetComponent<MeshFilter>().mesh = arrowMesh;
                    arrow.transform.localScale = new Vector3(arrowScale, arrowScale, arrowScale);
                }
            }

            // Non-XY planes are achieved by applying a rotation to Arrows in XY-Plane
            Quaternion rotation = Quaternion.identity;
            switch (vectorFieldMode)
            {
                case VectorFieldMode.XY_PLANE: break;
                case VectorFieldMode.YZ_PLANE: rotation = Quaternion.Euler(0, 90, 0); break;
                case VectorFieldMode.XZ_PLANE: rotation = Quaternion.Euler(90, 0, 0); break;
                case VectorFieldMode.CAMERA_PLANE: rotation = Camera.main == null ? Quaternion.identity : Camera.main.transform.rotation; break;
                case VectorFieldMode.FULL: break;
            }

            // Set position for each Arrow
            for (int i = 0; i < vectorFieldArrows.Length; i++)
            {
                var arrow = vectorFieldArrows[i];

                int cellX = i % resolution;
                int cellY = (i / resolution) % resolution;
                int cellZ = i / (resolution * resolution);
                Vector3 position = boxOrigin + new Vector3(
                    planeSideLength / resolution * cellX + cellSideLength / 2.0f,
                    planeSideLength / resolution * cellY + cellSideLength / 2.0f,
                    planeSideLength / resolution * cellZ + cellSideLength / 2.0f
                );

                // Apply rotation around current gameObjects center
                arrow.transform.position = transform.position + rotation * (position - transform.position);
            }
        }

        // Orbit Camera settings
        private float orbitInclineAngle = 0.0f;
        private float orbitRotationAngle = 0.0f;
        private float orbitDistanceToCenter = 3.0f;
        private bool orbitDragActive = false;

        void Update()
        {
            // Handle Orbit Camera
            bool lastDragActive = orbitDragActive;
            orbitDragActive = in3DMode && Input.GetMouseButton(1) && Application.isFocused;

            // Active/Deactive cursor
            if (lastDragActive != orbitDragActive)
            {
                UnityEngine.Cursor.lockState = orbitDragActive ? CursorLockMode.Locked : CursorLockMode.None;
                UnityEngine.Cursor.visible = !orbitDragActive;
            }

            // Update camera rotation
            if (orbitDragActive)
            {
                // Update orbit camera coordinates based on mouse delta
                Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                float sensitivity = 3.0f / (1080.0f) * Mathf.PI * 2.0f; // 1 Rotation every 1080 pixel

                orbitRotationAngle -= mouseDelta.x * sensitivity;
                orbitInclineAngle -= mouseDelta.y * sensitivity;

                // Clamp spherical coordinates
                orbitRotationAngle = orbitRotationAngle % (Mathf.PI * 2.0f);
                orbitInclineAngle = Mathf.Clamp(orbitInclineAngle, -75.0f / 360.0f * 2 * Mathf.PI, 45.0f / 360.0f * 2 * Mathf.PI);
            }
            // Update Camera zoom
            {
                const float MOUSE_WHEEL_SENSITIVITY = 0.3f;
                orbitDistanceToCenter -= Input.mouseScrollDelta.y * MOUSE_WHEEL_SENSITIVITY;
                orbitDistanceToCenter = Mathf.Clamp(orbitDistanceToCenter, 1.0f, 3.0f);
            }

            if (!in3DMode)
            {
                orbitInclineAngle = 0.0f;
                orbitRotationAngle = 0.0f;
                orbitDistanceToCenter = 2.0f;
            }

            // Update 3D-Camera
            {
                // Calculate lookDirection from sphere coordinates
                Vector3 lookDir = new Vector3(0, 0, 0);
                lookDir.z = Mathf.Sin(orbitRotationAngle + 2 * Mathf.PI / 4.0f);
                lookDir.x = Mathf.Cos(orbitRotationAngle + 2 * Mathf.PI / 4.0f);

                lookDir.y = Mathf.Sin(orbitInclineAngle);
                lookDir.x *= Mathf.Cos(orbitInclineAngle);
                lookDir.z *= Mathf.Cos(orbitInclineAngle);

                // Set camera postion and rotation
                player.transform.position = gameObject.transform.position - lookDir * orbitDistanceToCenter;
                player.transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
            }

            // Handle Drag and Drop
            {
                Camera camera = UnityEngine.Camera.main;
                if (camera == null)
                    return;
                Ray mouseRay = camera.ScreenPointToRay(Input.mousePosition); // Note: mousePosition.z is always 0

                // Check for new drag-start
                if (!dragActive && Input.GetMouseButtonDown(0))
                {
                    // Check for UI-Intersection first
                    GameObject collisionObject = null;
                    bool collidedWithUI = false;
                    if (uiRaycaster != null)
                    {
                        UnityEngine.EventSystems.PointerEventData pointerEventData =
                            new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
                        pointerEventData.position = Input.mousePosition;
                        List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();
                        uiRaycaster.Raycast(pointerEventData, results);

                        foreach (var result in results)
                        {
                            collidedWithUI = true;
                            if (result.gameObject.GetComponent<IconTo3DDraggable>() != null)
                            {
                                collisionObject = result.gameObject;
                                break;
                            }
                        }
                    }

                    // Check for 3D-Scene intersections
                    if (collisionObject == null)
                    {
                        RaycastHit hitInfo = new RaycastHit();
                        bool hit = Physics.Raycast(mouseRay, out hitInfo);
                        if (hit && hitInfo.collider != null)
                        {
                            collisionObject = hitInfo.collider.gameObject;
                        }
                    }

                    // Handle drag start
                    if (collisionObject != null)
                    {
                        var draggable = collisionObject.GetComponent<Draggable>();
                        var iconDraggable = collisionObject.GetComponent<IconTo3DDraggable>();
                        if (draggable != null)
                        {
                            dragActive = true;
                            selectedObject = collisionObject;

                            dragObjectPlaneDistance = Vector3.Dot(
                                camera.transform.rotation * Vector3.forward,
                                selectedObject.transform.position - camera.transform.position
                            );
                        }
                        else if (iconDraggable != null && pointChargePrefab != null)
                        {
                            GameObject instance3D = Instantiate(pointChargePrefab, chargeContainer.transform);
                            dragActive = true;
                            selectedObject = instance3D;

                            dragObjectPlaneDistance = Vector3.Dot(
                                camera.transform.rotation * Vector3.forward,
                                selectedObject.transform.position - camera.transform.position
                            );

                            // Set charge if instance has Charge Property
                            var chargeComponent = instance3D.GetComponent<PointCharge>();
                            if (chargeComponent != null)
                            {
                                chargeComponent.charge = uiChargeValue;
                            }
                        }
                    }

                    if (!dragActive && !collidedWithUI)
                    {
                        // De-select object if we clicked on something not draggable (e.g. decorative objects)
                        selectedObject = null;
                    }
                }

                // Calculate drag object position
                Plane plane = new Plane(
                    camera.transform.rotation * Vector3.back,
                    camera.transform.position + camera.transform.rotation * Vector3.forward * dragObjectPlaneDistance
                );
                float rayDistanceToPlane = 0.0f;
                bool collisionWithPlane = plane.Raycast(mouseRay, out rayDistanceToPlane);
                Vector3 rayPlaneIntersectionPoint = mouseRay.GetPoint(rayDistanceToPlane);

                // Update active drag
                if (dragActive && selectedObject != null && collisionWithPlane) // dragObject != null also checks if the object was destroyed
                {
                    selectedObject.transform.position = rayPlaneIntersectionPoint;
                    if (!in3DMode)
                    {
                        var pos = selectedObject.transform.position;
                        pos.z = transform.position.z - distanceToPlane;
                        selectedObject.transform.position = pos;
                    }
                }

                bool prevDragActive = dragActive;
                if (!Input.GetMouseButton(0) || !Application.isFocused || selectedObject == null) // the parameter specifies which mouse button, in range [0, 2] for left/right/middle button
                {
                    dragActive = false;
                }

                // Remove selected object if dragged out of bounds
                if (prevDragActive && !dragActive && selectedObject != null)
                {
                    Vector3 offset = selectedObject.transform.position - transform.position;
                    if (Mathf.Abs(offset.x) > 1.0f || Mathf.Abs(offset.y) > 1.0f || Mathf.Abs(offset.z) > 1.0f)
                    {
                        GameObject.Destroy(selectedObject);
                    }
                }

                // Update highlight object position
                if (selectedObject != null && highlightMarker != null)
                {
                    highlightMarker.SetActive(true);
                    highlightMarker.transform.position = selectedObject.transform.position;
                }
                else if (highlightMarker != null)
                {
                    highlightMarker.SetActive(false);
                }
            }

            // Update Arrow Positions if in Camera-Plane mode
            if (in3DMode && vectorFieldMode == VectorFieldMode.CAMERA_PLANE)
            {
                GenerateVectorFieldArrows();
            }

            // Update arrow properties (Direction, color, scale)
            if (vectorFieldArrows != null)
            {
                const float gridSize = 2.0f;
                float cellSideLength = gridSize / vectorFieldResolution;
                float maxArrowScale = cellSideLength / 2.0f * vectorFieldArrowScale; // Arrow mesh is in range [-1, 1], so we need to divide by 2

                var pointCharges = chargeContainer.GetComponentsInChildren<PointCharge>();
                for (int i = 0; i < vectorFieldArrows.Length; i++)
                {
                    var arrow = vectorFieldArrows[i];

                    // Calculate Electric Field and Potential value
                    Vector3 fieldValue = new Vector3();
                    float potential = 0.0f;
                    Vector3 arrowPos = arrow.transform.position;
                    if (!in3DMode) { // 2D mode
                        arrowPos.z = 0.0f;
                    }
                    foreach (var charge in pointCharges)
                    {
                        Vector3 chargePos = charge.transform.position;
                        if (!in3DMode) { // 2D mode
                            chargePos.z = 0.0f;
                        }

                        Vector3 towards = arrowPos - chargePos;
                        float distance = Mathf.Max(towards.magnitude, sphereRadius); // To prevent division by zero we add some threshold
                        fieldValue += towards.normalized * charge.charge / (distance * distance);
                        potential += charge.charge / towards.magnitude;
                    }

                    // Set Arrow direction
                    float len = fieldValue.sqrMagnitude;
                    Vector3 arrowDirection = fieldValue.normalized;
                    if (len < 0.001f) // In case of no electric field, point upwards
                    {
                        arrowDirection = new Vector3(0.0f, 1.0f, 0.0f);
                    }
                    arrow.transform.rotation = Quaternion.FromToRotation(Vector3.forward, arrowDirection);

                    // Calculate Alphas
                    bool potentialNegative = potential < 0.0f;
                    float potentialAlpha = Mathf.Pow(Mathf.Clamp(Mathf.Abs(potential / MaxPotentialValue), 0, 1), PotentialExpo);
                    float efieldMagAlpha = Mathf.Pow(Mathf.Clamp(Mathf.Abs(fieldValue.magnitude / MaxEFieldMagnitude), 0, 1), EFieldExpo);

                    // Find Color, Transparency and Scale
                    Color color = new Color(0.7f, 0.7f, 0.7f, 1.0f);
                    float transparencyAlpha = 1.0f;
                    float scale = maxArrowScale;
                    {
                        if (colorMode != ScalarDisplayMode.NONE)
                        {
                            float alpha = colorMode == ScalarDisplayMode.POTENTIAL ? potentialAlpha : efieldMagAlpha;
                            Color maxColor = potential < 0.0f ? Color.blue : Color.red;
                            if (colorMode == ScalarDisplayMode.EFIELD_MAG)
                            {
                                maxColor = Color.red;
                            }
                            color = Color.Lerp(color, maxColor, alpha);
                        }

                        if (alphaMode != ScalarDisplayMode.NONE)
                        {
                            transparencyAlpha = alphaMode == ScalarDisplayMode.POTENTIAL ? potentialAlpha : efieldMagAlpha;
                        }

                        if (scaleMode != ScalarDisplayMode.NONE)
                        {
                            float scaleAlpha = scaleMode == ScalarDisplayMode.POTENTIAL ? potentialAlpha : efieldMagAlpha;
                            float minScaleValue = maxArrowScale / 5.0f;
                            scale = Mathf.Lerp(minScaleValue, maxArrowScale, scaleAlpha);
                        }
                    }

                    // Set Color, Transparency and Scale
                    { 
                        arrow.transform.localScale = new Vector3(scale, scale, scale);

                        var meshRenderer = arrow.GetComponent<MeshRenderer>();
                        if (meshRenderer != null)
                        {
                            color.a = transparencyAlpha;
                            meshRenderer.material.color = color;
                        }
                    }
                }
            }
        }

        public void ChangeChargeValue(float value)
        {
            // Clamp value
            if (value > uiChargeMaxValue)
                value = uiChargeMaxValue;
            if (value < -uiChargeMaxValue)
                value = -uiChargeMaxValue;
            uiChargeValue = value;

            // Update UI-Components
            if (chargeValueField != null)
            {
                if (!chargeValueField.isFocused)
                {
                    chargeValueField.text = value.ToString("F", CultureInfo.CurrentCulture);
                }
            }
            if (chargeValueSlider != null)
            {
                // Note: This check is necessary to prevent infinit loop of slider updating UIController updating slider...
                if (chargeValueSlider.value != value)
                {
                    chargeValueSlider.value = value;
                }
            }
            // Update currently selected Object (If it's a charge object)
            if (selectedObject != null)
            {
                var pointCharge = selectedObject.GetComponent<PointCharge>();
                if (pointCharge != null)
                {
                    pointCharge.charge = value;
                }
            }
        }

        public static Color ChargeValueToColor(float chargeValue)
        {
            Color neutralColor = new Color(0.8f, 0.8f, 0.8f);
            Color saturatedColor = chargeValue < 0.0f ? Color.blue : Color.red;

            float value = Mathf.Abs(chargeValue);
            float alpha = Mathf.Max(0, value / uiChargeMaxValue);
            return Color.Lerp(neutralColor, saturatedColor, alpha);
        }

        private void LateUpdate()
        {
            if (selectedObject != null)
            {
                var pointCharge = selectedObject.GetComponent<PointCharge>();
                if (pointCharge != null)
                {
                    ChangeChargeValue(pointCharge.charge);
                }
            }

            if (chargeValueSlider != null)
            {
                chargeValueSlider.maxValue = uiChargeMaxValue;
                chargeValueSlider.minValue = -uiChargeMaxValue;
            }

            // Update charge preview image
            {
                chargeImage.color = ChargeValueToColor(uiChargeValue);

                if (uiChargeValue >= 0)
                {
                    chargePlusImage.gameObject.SetActive(true);
                    chargeMinusImage.gameObject.SetActive(false);
                }
                else
                {
                    chargePlusImage.gameObject.SetActive(false);
                    chargeMinusImage.gameObject.SetActive(true);
                }
            }
        }

        public void Set3DModeOnOff(bool in3DMode)
        {
            this.in3DMode = in3DMode;
            backgroundObject2D?.SetActive(!in3DMode);
            backgroundObject3D?.SetActive(in3DMode);
            backgroundObject3D?.SetActive(false);

            // Reset camera
            if (in3DMode)
            {
                transform.position = startPosition + new Vector3(0, 0, 1);
                player.transform.position = startPosition + new Vector3(0, 0, -2);
                player.transform.rotation = Quaternion.identity;
            }
            else
            {
                transform.position = startPosition;
                player.transform.position = startPosition + new Vector3(0, 0, -2);
                player.transform.rotation = Quaternion.identity;

                // Clamp charge positions to 2d
                var pointCharges = chargeContainer.GetComponentsInChildren<PointCharge>();
                foreach (var charge in pointCharges)
                {
                    var pos = charge.transform.position;
                    pos.z = transform.position.z - distanceToPlane;
                    charge.transform.position = pos;
                }
            }
        }

        public void ModeDropdownChanged(int value)
        {
            if (value < 0 || value > (int)VectorFieldMode.FULL)
            {
                value = (int)VectorFieldMode.XY_PLANE;
            }
            vectorFieldMode = (VectorFieldMode)value;
            GenerateVectorFieldArrows();
        }

        public void ColorModeChanged(int value)
        {
            if (value < 0 || value > (int)ScalarDisplayMode.EFIELD_MAG)
            {
                value = (int)ScalarDisplayMode.NONE;
            }
            colorMode = (ScalarDisplayMode)value;
        }

        public void AlphaModeChanged(int value)
        {
            if (value < 0 || value > (int)ScalarDisplayMode.EFIELD_MAG)
            {
                value = (int)ScalarDisplayMode.NONE;
            }
            alphaMode = (ScalarDisplayMode)value;
        }

        public void ScaleModeChanged(int value)
        {
            if (value < 0 || value > (int)ScalarDisplayMode.EFIELD_MAG)
            {
                value = (int)ScalarDisplayMode.NONE;
            }
            scaleMode = (ScalarDisplayMode)value;
        }

        public void SetVectorFieldResolution(int resolution)
        {
            vectorFieldResolution = resolution;
            GenerateVectorFieldArrows();
        }

        public void SetVectorFieldEnabled(bool enabled)
        {
            vectorFieldEnabled = enabled;
            GenerateVectorFieldArrows();
        }

        public void SetVectorFieldArrowScale(float scale)
        {
            vectorFieldArrowScale = scale;
            GenerateVectorFieldArrows();
        }

        public void SetVectorField2DOffset(float offset)
        {
            vectorField2DOffset = offset;
            GenerateVectorFieldArrows();
        }
    }
}
