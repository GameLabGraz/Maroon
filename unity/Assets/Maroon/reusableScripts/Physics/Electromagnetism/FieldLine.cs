//-----------------------------------------------------------------------------
// FieldLine.cs
//
// Script to draw the field lines
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;
using System.Collections.Generic;

namespace Maroon.Physics.Electromagnetism
{
    /// <summary>
    /// Script to draw the field lines.
    /// One Field Line is cloned around an axis.
    /// </summary>
    [RequireComponent(typeof(AdvancedLineRenderer))]
    public class FieldLine : MonoBehaviour, IResetObject
    {
        /// <summary>
        /// The magnetic field
        /// </summary>
	    public IField field;

        /// <summary>
        /// The vertex count of a single field line.
        /// </summary>
        public int vertexCount = 300;

        /// <summary>
        /// The length of the line between two vertices
        /// </summary>
        public float lineSegmentLength = 0.3f;

        /// <summary>
        /// A certain offset from the origin where
        /// the first vertex is drawn.
        /// </summary>
        public Vector3 originOffset = Vector3.zero;

        /// <summary>
        /// Value to influence the curve of the field line
        /// </summary>
        public float fixClosingAngle = 14;

        /// <summary>
        /// The electro magnet object
        /// </summary>
        private GameObject emObj;

        /// <summary>
        /// Boolean value which indicates if the 
        /// field line is visible
        /// </summary>
        private bool visible = true;

        /// <summary>
        /// The line renderer which draws the line
        /// </summary>
        private AdvancedLineRenderer _lineRenderer;

        public delegate bool StopDrawingCheck(Vector3 position);
        public StopDrawingCheck stopDrawingCheck;

        [SerializeField]
        private float _lineWidth = 0.1f;

        [SerializeField] private float fieldStrengthFactor = 1.0f;

        [SerializeField] private bool invertDirection;

        /// <summary>
        /// Initializes the line renderer.
        /// </summary>
        public void Start()
        {
            _lineRenderer = GetComponent<AdvancedLineRenderer>();
            _lineRenderer.InitLineRenderer();
            _lineRenderer.SetWidth(_lineWidth, _lineWidth);

            emObj = transform.parent.gameObject;
        }

        /// <summary>
        /// Draws the field lines
        /// </summary>
        public void Draw()
        {
            if (_lineRenderer == null)
                return;

            _lineRenderer.Clear();

            if (!visible || Mathf.Abs(GetFieldStrengthFromEmObj()) * fieldStrengthFactor < 0.05)
                return;

            var closingAngle = fixClosingAngle + (4 - GetFieldStrengthFromEmObj()) * 2;

            var positionIndex = 0;
            var position = transform.TransformPoint(Vector3.zero - originOffset);
            _lineRenderer.SetPosition(positionIndex, transform.InverseTransformPoint(position));
            positionIndex++;
            while (positionIndex < vertexCount)
            {
                var p = Vector3.Normalize(field.get(position) * PhysicalConstants.FieldStrengthFactor);

                var direction = Quaternion.AngleAxis(closingAngle, transform.forward) * p;
                if (invertDirection)
                    direction *= -1f;

                position += direction * lineSegmentLength;

                _lineRenderer.SetPosition(positionIndex, transform.InverseTransformPoint(position));
                positionIndex++;

                if (stopDrawingCheck != null && stopDrawingCheck(position))
                    break;
            }

            _lineRenderer.WritePositionsToLineRenderer();

        }

        /// <summary>
        /// Clears the line renderer
        /// </summary>
        private void ClearLineRenderer()
        {
            _lineRenderer.Clear();
        }

        /// <summary>
        /// Sets the visibility of the field lines
        /// on or off
        /// </summary>
        /// <param name="visibility">visible if true else invisible</param>
        public void SetVisibility(bool visibility)
        {
            this.visible = visibility;
            if (!visible)
                ClearLineRenderer();
        }

        /// <summary>
        /// Clears the clones and line renderer
        /// </summary>
        public void ResetObject()
        {
            if(_lineRenderer) _lineRenderer.Clear();
        }

        public List<KeyValuePair<int, Vector3>> GetLinePositions()
        {
            return !_lineRenderer ? new List<KeyValuePair<int, Vector3>>() : _lineRenderer.GetPositions();
        }

        private float GetFieldStrengthFromEmObj()
        {
            if (!field) return 0f;
            switch (field.getFieldType())
            {
                case FieldType.BField:
                    return emObj.GetComponent<IGenerateB>().getFieldStrength();
                case FieldType.EField:
                    return emObj.GetComponent<IGenerateE>().getFieldStrength();
                default:
                    return 0f;
            }
        }
    }
}
