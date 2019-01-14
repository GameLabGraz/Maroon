//-----------------------------------------------------------------------------
// Graph.cs
//
// Script which handles the drawing of the graph
//
//
// Authors: Michael Stefan Holly
//-----------------------------------------------------------------------------
//

using UnityEngine;

namespace UI
{
    /// <summary>
    /// Script which handles the drawing of the graph
    /// </summary>
    public abstract class Graph : ValueDisplay, IResetObject
    {
        /// <summary>
        /// Material used for drawing the graph
        /// </summary>
        [SerializeField]
        private Material material;

        /// <summary>
        /// Texture used for drawing the graph
        /// </summary>
        protected Texture2D texture;

        /// <summary>
        /// Line renderer which draws the graph
        /// </summary>
        private AdvancedLineRenderer line_renderer;

        /// <summary>
        /// value on x axis of graph
        /// </summary>
        private float time = 0;

        /// <summary>
        /// position of point in line renderer
        /// </summary>
        private int index = 0;

        /// <summary>
        /// Max num of vertices being drawn
        /// </summary>
        private float max_vertex;

        /// <summary>
        /// The rect transform width
        /// </summary>
        protected float width = 0;

        /// <summary>
        /// The rect transform height
        /// </summary>
        protected float height = 0;

        /// <summary>
        /// max value of the graph
        /// </summary>
        [SerializeField]
        private float _max = 1;

        /// <summary>
        /// min value of the graph
        /// </summary>
        [SerializeField]
        private float _min = -1;

        private SimulationController simController;

        [SerializeField]
        private float _stepSize = 0.5f;

        [SerializeField]
        private float _lineWidth = 0.1f;

        [SerializeField]
        private Vector3 _xAxis = Vector3.right;

        [SerializeField]
        private Vector3 _yAxis = Vector3.up;

        [SerializeField]
        private Vector3 _zAxis = Vector3.forward;

        [SerializeField]
        private float _zOffset = 0;

        [SerializeField]
        private uint _fixedUpdateRate = 1;

        private uint _fixedUpdateCount = 0;

        /// <summary>
        /// Draws the graph when the simulation is running
        /// </summary>
        private void FixedUpdate()
        {
            if (!simController.SimulationRunning)
                return;

            if (_fixedUpdateCount++ % _fixedUpdateRate != 0)
                return;

            var value = GetValue<float>();
            var drawPoint = _xAxis * time + _yAxis * GetRange(value) + _zAxis * _zOffset;
            line_renderer.SetPosition(index++, drawPoint);
            line_renderer.WritePositionsToLineRenderer();
            time += _stepSize;
            if (index > max_vertex)
                ResetObject();
        }

        protected void Initialize()
        {
            var simControllerObject = GameObject.Find("SimulationController");
            if (simControllerObject)
                simController = simControllerObject.GetComponent<SimulationController>();

            time = -width / 2;
            max_vertex = width / _stepSize;

            texture = new Texture2D(100, 100);
            InitTexture();

            line_renderer = gameObject.AddComponent<AdvancedLineRenderer>();
            line_renderer.SetWidth(_lineWidth, _lineWidth);
            line_renderer.SetMaterial(material);
            line_renderer.GenerateLightingData = true;
            line_renderer.initLineRenderer();
        }

        /// <summary>
        /// Normalizes value to fit into the graph
        /// </summary>
        /// <param name="value">Value to normalize</param>
        /// <returns>new value</returns>
        private float GetRange(float value)
        {
            if (value > _max)
                value = _max;
            if (value < _min)
                value = _min;

            var range = (height * (value - _min)) / (_max - _min) - (height / 2);
            return range;
        }

        /// <summary>
        /// Initializes the texture of the graph
        /// </summary>
        private void InitTexture()
        {
            var col = new Color[texture.width * texture.height];
            for (var j = 0; j < col.Length; j++)
                col[j] = Color.white;

            texture.SetPixels(col);
            texture.Apply();

            for (var j= 0; j < texture.width; j++) //Black line
                texture.SetPixel(j, texture.height / 2, Color.black);

            texture.Apply();
        }

        /// <inheritdoc />
        /// <summary>
        /// Resets the graph
        /// </summary>
        public void ResetObject()
        {
            time = -width / 2;
            max_vertex = width / _stepSize;
            index = 0;
            line_renderer.Clear();
        }
    }
}
