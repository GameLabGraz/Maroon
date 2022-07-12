using Maroon.Physics;
using UnityEngine;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts
{
    public class NoiseExperimentBase : MonoBehaviour
    {
        [SerializeField] protected NoiseVisualisation noise_visualisation;
        [SerializeField, Range(0, 20)] protected float rotation_speed = 0;

        [SerializeField, Header("Common Configuration")]
        protected QuantityInt seed;

        [SerializeField] public QuantityInt size;
        [SerializeField] public QuantityFloat scale;
        [SerializeField] public QuantityFloat octaves;

        [Space(10)] [SerializeField, Range(0, 2)]
        protected float speed = 1;

        [SerializeField] protected bool dirty;

        private static NoiseExperimentBase _instance;

        public static NoiseExperimentBase Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<NoiseExperimentBase>();

                return _instance;
            }
        }

        protected MeshFilter meshFilter;
        protected bool is_rotating;
        public static readonly Noise Noise3D = new Noise(0);
        public float time { get; protected set; }


        // Start is called before the first frame update
        protected void Start()
        {
            if (!meshFilter)
                meshFilter = GetComponentInChildren<MeshFilter>();
            noise_visualisation.GenerateMesh(meshFilter.sharedMesh);
        }

        private void Update()
        {
            HandleUpdate();
        }


        protected virtual void HandleUpdate()
        {
            time += Time.deltaTime * speed;
            Noise3D.offset = time;
            if (!meshFilter)
                meshFilter = GetComponentInChildren<MeshFilter>();
            noise_visualisation.UpdateMesh(meshFilter.sharedMesh);
            meshFilter.transform.Rotate(Vector3.up, Time.deltaTime * rotation_speed, Space.World);
        }

        public void SetDirty() => dirty = true;


        private (Color32 top, Color32 middle, Color32 bottom) colors = (Color.gray, Color.yellow, Color.cyan);

        public void SetTopColor(float color)
        {
            SetDirty();
            colors.top = Color.HSVToRGB(color, 1, 1);
        }

        public void SetBottomColor(float color)
        {
            SetDirty();
            colors.bottom = Color.HSVToRGB(color, 1, 1);
        }

        public void SetMiddleColor(float color)
        {
            SetDirty();
            colors.middle = Color.HSVToRGB(color, 1, 1);
        }

        public Color GetVertexColor(float value, float bottom, float middle, float top)
        {
            if (value > middle)
                return Color.Lerp(colors.middle, colors.top, value.Map(middle, top));
            return Color.Lerp(colors.bottom, colors.middle, value.Map(bottom, middle));
        }
    }
}