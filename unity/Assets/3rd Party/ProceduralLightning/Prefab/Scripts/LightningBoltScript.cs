//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

#if !UNITY_EDITOR && !UNITY_WEBGL && !UNITY_WEBPLAYER

// experimental - instead of forcing List.ToArray for every mesh update, we can simply modify the Length property of System.Array
//  to be the appropriate value. On desktop this doesn't make a lot of difference, but on mobile it could be a much greater performance gain.
// in order to use unsafe code you must add a smcs.rsp file in your Assets folder with the contents containing a single line of "-unsafe" without quotes
// uncomment this next line to turn on unsafe arrays for mesh generation
// #define ENABLE_UNSAFE_ARRAY

#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DigitalRuby.ThunderAndLightning
{

#if ENABLE_UNSAFE_ARRAY

    public class UnsafeArrayVector4
    {
        private int savedCount;
        private int count;
        private Vector4[] array;

        public UnsafeArrayVector4()
        {
            array = new Vector4[128];
        }

        public Vector4 this[int i]
        {
            get { return array[i]; }
            set { array[i] = value; }
        }

        public void Add(Vector4 obj)
        {
            Add(ref obj);
        }

        public void Add(ref Vector4 obj)
        {
            if (count == array.Length)
            {
                System.Array.Resize<Vector4>(ref array, count * 2);
            }
            array[count++] = obj;
        }

        public void Clear()
        {
            count = 0;
        }

        public void InsertCountBeforeArray()
        {
            savedCount = array.Length;

            unsafe
            {
                fixed (void* ptr = array)
                {
                    int* intPtr = (int*)ptr;
                    *(intPtr - 1) = Count;
                }
            }
        }

        public void RevertCountBeforeArray()
        {
            unsafe
            {
                fixed (void* ptr = array)
                {
                    int* intPtr = (int*)ptr;
                    *(intPtr - 1) = savedCount;
                }
            }
        }

        public int Count
        {
            get { return count; }
        }

        public Vector4[] Array { get { return array; } }
    }

    public class UnsafeArrayVector3
    {
        private int savedCount;
        private int count;
        private Vector3[] array;

        public UnsafeArrayVector3()
        {
            array = new Vector3[128];
        }

        public void Add(Vector3 obj)
        {
            Add(ref obj);
        }

        public void Add(ref Vector3 obj)
        {
            if (count == array.Length)
            {
                System.Array.Resize<Vector3>(ref array, count * 2);
            }
            array[count++] = obj;
        }

        public void Clear()
        {
            count = 0;
        }

        public void InsertCountBeforeArray()
        {
            savedCount = array.Length;

            unsafe
            {
                fixed (void* ptr = array)
                {
                    int* intPtr = (int*)ptr;
                    *(intPtr - 1) = Count;
                }
            }
        }

        public void RevertCountBeforeArray()
        {
            unsafe
            {
                fixed (void* ptr = array)
                {
                    int* intPtr = (int*)ptr;
                    *(intPtr - 1) = savedCount;
                }
            }
        }

        public int Count
        {
            get { return count; }
        }

        public Vector3[] Array { get { return array; } }
    }

    public class UnsafeArrayVector2
    {
        private int savedCount;
        private int count;
        private Vector2[] array;

        public UnsafeArrayVector2()
        {
            array = new Vector2[128];
        }

        public void Add(Vector2 obj)
        {
            Add(ref obj);
        }

        public void Add(ref Vector2 obj)
        {
            if (count == array.Length)
            {
                System.Array.Resize<Vector2>(ref array, count * 2);
            }
            array[count++] = obj;
        }

        public void Clear()
        {
            count = 0;
        }

        public void InsertCountBeforeArray()
        {
            savedCount = array.Length;

            unsafe
            {
                fixed (void* ptr = array)
                {
                    int* intPtr = (int*)ptr;
                    *(intPtr - 1) = Count;
                }
            }
        }

        public void RevertCountBeforeArray()
        {
            unsafe
            {
                fixed (void* ptr = array)
                {
                    int* intPtr = (int*)ptr;
                    *(intPtr - 1) = savedCount;
                }
            }
        }

        public int Count
        {
            get { return count; }
        }

        public Vector2[] Array { get { return array; } }
    }

    public class UnsafeArrayInt
    {
        private int savedCount;
        private int count;
        private int[] array;

        public UnsafeArrayInt()
        {
            array = new int[128];
        }

        public void Add(int obj)
        {
            Add(ref obj);
        }

        public void Add(ref int obj)
        {
            if (count == array.Length)
            {
                System.Array.Resize<int>(ref array, count * 2);
            }
            array[count++] = obj;
        }

        public void Clear()
        {
            count = 0;
        }

        public void InsertCountBeforeArray()
        {
            savedCount = array.Length;

            unsafe
            {
                fixed (void* ptr = array)
                {
                    int* intPtr = (int*)ptr;
                    *(intPtr - 1) = Count;
                }
            }
        }

        public void RevertCountBeforeArray()
        {
            unsafe
            {
                fixed (void* ptr = array)
                {
                    int* intPtr = (int*)ptr;
                    *(intPtr - 1) = savedCount;
                }
            }
        }

        public int Count
        {
            get { return count; }
        }

        public int[] Array { get { return array; } }
    }

    public class UnsafeArrayColor
    {
        private int savedCount;
        private int count;
        private Color[] array;

        public UnsafeArrayColor()
        {
            array = new Color[128];
        }

        public void Add(Color obj)
        {
            Add(ref obj);
        }

        public void Add(ref Color obj)
        {
            if (count == array.Length)
            {
                System.Array.Resize<Color>(ref array, count * 2);
            }
            array[count++] = obj;
        }

        public void Clear()
        {
            count = 0;
        }

        public void InsertCountBeforeArray()
        {
            savedCount = array.Length;

            unsafe
            {
                fixed (void* ptr = array)
                {
                    int* intPtr = (int*)ptr;
                    *(intPtr - 1) = Count;
                }
            }
        }

        public void RevertCountBeforeArray()
        {
            unsafe
            {
                fixed (void* ptr = array)
                {
                    int* intPtr = (int*)ptr;
                    *(intPtr - 1) = savedCount;
                }
            }
        }

        public int Count
        {
            get { return count; }
        }

        public Color[] Array { get { return array; } }
    }

#endif

    /// <summary>
    /// Render lightning using meshes
    /// </summary>
    public class LightningBoltMeshRenderer
    {
        [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
        public class LineRendererMesh : MonoBehaviour
        {
            private static readonly Vector2 uv1 = new Vector2(0.0f, 0.0f);
            private static readonly Vector2 uv2 = new Vector2(1.0f, 0.0f);
            private static readonly Vector2 uv3 = new Vector2(0.0f, 1.0f);
            private static readonly Vector2 uv4 = new Vector2(1.0f, 1.0f);

            private Mesh mesh;
            private MeshRenderer meshRenderer;

#if ENABLE_UNSAFE_ARRAY

            private readonly UnsafeArrayInt indices = new UnsafeArrayInt();
            private readonly UnsafeArrayVector2 texCoords = new UnsafeArrayVector2();
            private readonly UnsafeArrayVector3 vertices = new UnsafeArrayVector3();
            private readonly UnsafeArrayVector4 lineDirs = new UnsafeArrayVector4();
            private readonly UnsafeArrayColor colors = new UnsafeArrayColor();
            private readonly UnsafeArrayVector2 glowModifiers = new UnsafeArrayVector2();
            private readonly UnsafeArrayVector3 ends = new UnsafeArrayVector3();

#else

            private readonly List<int> indices = new List<int>();
            private readonly List<Vector2> texCoords = new List<Vector2>();
            private readonly List<Vector3> vertices = new List<Vector3>();
            private readonly List<Vector4> lineDirs = new List<Vector4>();
            private readonly List<Color> colors = new List<Color>();
            private readonly List<Vector2> glowModifiers = new List<Vector2>();
            private readonly List<Vector3> ends = new List<Vector3>();

#endif

            private void Awake()
            {
                mesh = new Mesh();
                GetComponent<MeshFilter>().sharedMesh = mesh;

                meshRenderer = GetComponent<MeshRenderer>();
                meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                meshRenderer.receiveShadows = false;
                meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                meshRenderer.enabled = false;
            }

            private void AddIndices()
            {
                int vertexIndex = vertices.Count;
                indices.Add(vertexIndex++);
                indices.Add(vertexIndex++);
                indices.Add(vertexIndex);
                indices.Add(vertexIndex--);
                indices.Add(vertexIndex);
                indices.Add(vertexIndex += 2);
            }

            public void Begin()
            {
                meshRenderer.enabled = true;

#if ENABLE_UNSAFE_ARRAY

                vertices.InsertCountBeforeArray();
                mesh.vertices = vertices.Array;

                lineDirs.InsertCountBeforeArray();
                mesh.tangents = lineDirs.Array;

                colors.InsertCountBeforeArray();
                mesh.colors = colors.Array;

                texCoords.InsertCountBeforeArray();
                mesh.uv = texCoords.Array;

                glowModifiers.InsertCountBeforeArray();
                mesh.uv2 = glowModifiers.Array;

                ends.InsertCountBeforeArray();
                mesh.normals = ends.Array;

                indices.InsertCountBeforeArray();
                mesh.triangles = indices.Array;

				vertices.RevertCountBeforeArray();
				lineDirs.RevertCountBeforeArray();
				colors.RevertCountBeforeArray();
				texCoords.RevertCountBeforeArray();
				glowModifiers.RevertCountBeforeArray();
				ends.RevertCountBeforeArray();
                indices.RevertCountBeforeArray();

#else

                mesh.vertices = vertices.ToArray();
                mesh.tangents = lineDirs.ToArray();
                mesh.colors = colors.ToArray();
                mesh.uv = texCoords.ToArray();
                mesh.uv2 = glowModifiers.ToArray();
                mesh.normals = ends.ToArray();
                mesh.triangles = indices.ToArray();

#endif

            }

            public bool PrepareForLines(int lineCount)
            {
                int vertexCount = lineCount * 4;
                if (vertices.Count + vertexCount > 64999)
                {
                    return false;
                }
                return true;
            }

            public void BeginLine(Vector3 start, Vector3 end, float radius, Color c, float glowWidthModifier, float glowIntensity)
            {
                AddIndices();

                Vector2 glowModifier = new Vector2(glowWidthModifier, glowIntensity);
                Vector4 dir = (end - start);
                dir.w = radius;

                vertices.Add(start);
                texCoords.Add(uv1);
                lineDirs.Add(dir);
                colors.Add(c);
                glowModifiers.Add(glowModifier);
                ends.Add(dir);

                vertices.Add(end);
                texCoords.Add(uv2);
                lineDirs.Add(dir);
                colors.Add(c);
                glowModifiers.Add(glowModifier);
                ends.Add(dir);

                dir.w = -radius;

                vertices.Add(start);
                texCoords.Add(uv3);
                lineDirs.Add(dir);
                colors.Add(c);
                glowModifiers.Add(glowModifier);
                ends.Add(dir);

                vertices.Add(end);
                texCoords.Add(uv4);
                lineDirs.Add(dir);
                colors.Add(c);
                glowModifiers.Add(glowModifier);
                ends.Add(dir);
            }

            public void AppendLine(Vector3 start, Vector3 end, float radius, Color c, float glowWidthModifier, float glowIntensity)
            {
                AddIndices();

                Vector2 glowModifier = new Vector2(glowWidthModifier, glowIntensity);
                Vector4 dir = (end - start);
                dir.w = radius;

                vertices.Add(start);
                texCoords.Add(uv1);
                // rotation will be in the same direction as the previous connected vertices
                lineDirs.Add(lineDirs[lineDirs.Count - 3]);
                colors.Add(c);
                glowModifiers.Add(glowModifier);
                ends.Add(dir);

                vertices.Add(end);
                texCoords.Add(uv2);
                lineDirs.Add(dir);
                colors.Add(c);
                glowModifiers.Add(glowModifier);
                ends.Add(dir);

                dir.w = -radius;

                vertices.Add(start);
                texCoords.Add(uv3);
                // rotation will be in the same direction as the previous connected vertices
                lineDirs.Add(lineDirs[lineDirs.Count - 3]);
                colors.Add(c);
                glowModifiers.Add(glowModifier);
                ends.Add(dir);

                vertices.Add(end);
                texCoords.Add(uv4);
                lineDirs.Add(dir);
                colors.Add(c);
                glowModifiers.Add(glowModifier);
                ends.Add(dir);
            }

            public void Reset()
            {
                meshRenderer.enabled = false;
                mesh.triangles = null;
                indices.Clear();
                vertices.Clear();
                colors.Clear();
                lineDirs.Clear();
                texCoords.Clear();
                glowModifiers.Clear();
                ends.Clear();
            }

            public Material Material
            {
                get { return GetComponent<MeshRenderer>().sharedMaterial; }
                set { GetComponent<MeshRenderer>().sharedMaterial = value; }
            }
        }

        private LineRendererMesh currentLineRenderer;
        private readonly Dictionary<LightningBolt, List<LineRendererMesh>> renderers = new Dictionary<LightningBolt, List<LineRendererMesh>>();
        private readonly List<LineRendererMesh> rendererCache = new List<LineRendererMesh>();

        private IEnumerator EnableRenderer(LineRendererMesh renderer, LightningBolt lightningBolt)
        {
            yield return new WaitForSeconds(lightningBolt.MinimumDelay);

            if (renderer != null && lightningBolt.IsActive)
            {
                renderer.Begin();
            }
        }

        private LineRendererMesh CreateLineRenderer(LightningBolt lightningBolt, List<LineRendererMesh> lineRenderers)
        {
            LineRendererMesh lineRenderer;

            if (rendererCache.Count == 0)
            {
                // create a line renderer in a new game object underneath the parent
                GameObject obj = new GameObject();
                obj.name = "LightningBoltMeshRenderer";
                obj.hideFlags = HideFlags.HideAndDontSave;
                lineRenderer = obj.AddComponent<LineRendererMesh>();
            }
            else
            {
                lineRenderer = rendererCache[rendererCache.Count - 1];
                rendererCache.RemoveAt(rendererCache.Count - 1);
            }
            lineRenderer.gameObject.transform.parent = lightningBolt.Parent.transform;

            if (lightningBolt.UseWorldSpace)
            {
                lineRenderer.gameObject.transform.position = Vector3.zero;
            }
            else
            {
                lineRenderer.gameObject.transform.localPosition = Vector3.zero;
            }
            lineRenderer.gameObject.transform.rotation = Quaternion.identity;
            lineRenderer.gameObject.transform.localScale = Vector3.one;
            lineRenderer.Material = (lightningBolt.HasGlow ? Material : MaterialNoGlow);
            currentLineRenderer = lineRenderer;
            lineRenderers.Add(lineRenderer);

            return lineRenderer;
        }

        public void Begin(LightningBolt lightningBolt)
        {
            List<LineRendererMesh> lineRenderers;
            if (!renderers.TryGetValue(lightningBolt, out lineRenderers))
            {
                lineRenderers = new List<LineRendererMesh>();
                renderers[lightningBolt] = lineRenderers;
                CreateLineRenderer(lightningBolt, lineRenderers);
            }
        }

        public void End(LightningBolt lightningBolt)
        {
            if (currentLineRenderer != null)
            {
                Script.StartCoroutine(EnableRenderer(currentLineRenderer, lightningBolt));
                currentLineRenderer = null;
            }
        }

        public void AddGroup(LightningBolt lightningBolt, LightningBoltSegmentGroup group, float growthMultiplier)
        {
            List<LineRendererMesh> lineRenderers = renderers[lightningBolt];
            LineRendererMesh lineRenderer = lineRenderers[lineRenderers.Count - 1];
            float timeStart = Time.timeSinceLevelLoad + group.Delay;
            Color c = new Color(timeStart, timeStart + group.PeakStart, timeStart + group.PeakEnd, timeStart + group.LifeTime);
            float radius = group.LineWidth * 0.5f;
            int lineCount = (group.Segments.Count - group.StartIndex);
            float radiusStep = (radius - (radius * group.EndWidthMultiplier)) / (float)lineCount;

            // growth multiplier
            float timeStep, timeOffset;
            if (growthMultiplier > 0.0f)
            {
                timeStep = (group.LifeTime / (float)lineCount) * growthMultiplier;
                timeOffset = 0.0f;
            }
            else
            {
                timeStep = 0.0f;
                timeOffset = 0.0f;
            }

            if (!lineRenderer.PrepareForLines(lineCount))
            {
                Script.StartCoroutine(EnableRenderer(lineRenderer, lightningBolt));
                lineRenderer = CreateLineRenderer(lightningBolt, lineRenderers);
            }

            lineRenderer.BeginLine(group.Segments[group.StartIndex].Start, group.Segments[group.StartIndex].End, radius, c, GlowWidthMultiplier, GlowIntensityMultiplier);
            for (int i = group.StartIndex + 1; i < group.Segments.Count; i++)
            {
                radius -= radiusStep;
                if (growthMultiplier < 1.0f)
                {
                    timeOffset += timeStep;
                    c = new Color(timeStart + timeOffset, timeStart + group.PeakStart + timeOffset, timeStart + group.PeakEnd, timeStart + group.LifeTime);
                }
                lineRenderer.AppendLine(group.Segments[i].Start, group.Segments[i].End, radius, c, GlowWidthMultiplier, GlowIntensityMultiplier);
            }
        }

        public void Cleanup(LightningBolt lightningBolt)
        {
            List<LineRendererMesh> lineRenderers;
            if (renderers.TryGetValue(lightningBolt, out lineRenderers))
            {
                renderers.Remove(lightningBolt);
                foreach (LineRendererMesh lineRenderer in lineRenderers)
                {
                    rendererCache.Add(lineRenderer);
                    lineRenderer.Reset();
                }
            }
        }

        /// <summary>
        /// Lightning bolt script
        /// </summary>
        public LightningBoltScript Script { get; set; }

        /// <summary>
        /// Material
        /// </summary>
        public Material Material { get; set; }

        /// <summary>
        /// No glow material
        /// </summary>
        public Material MaterialNoGlow { get; set; }

        /// <summary>
        /// Glow width multiplier (0 for none)
        /// </summary>
        public float GlowWidthMultiplier { get; set; }

        /// <summary>
        /// Glow intensity multiplier (0 for none)
        /// </summary>
        public float GlowIntensityMultiplier { get; set; }
    }

    [System.Serializable]
    public class LightningLightParameters
    {
        /// <summary>
        /// Light render mode
        /// </summary>
        [Tooltip("Light render mode - leave as auto unless you have special use cases")]
        [HideInInspector]
        public LightRenderMode RenderMode = LightRenderMode.Auto;

        /// <summary>
        /// Color of light
        /// </summary>
        [Tooltip("Color of the light")]
        public Color LightColor = Color.white;

        /// <summary>
        /// What percent of segments should have a light? Keep this pretty low for performance, i.e. 0.05 or lower depending on generations
        /// Set really really low to only have 1 light, i.e. 0.0000001f
        /// For example, at generations 5, the main trunk has 32 segments, 64 at generation 6, etc.
        /// If non-zero, there wil be at least one light in the middle
        /// </summary>
        [Tooltip("What percent of segments should have a light? For performance you may want to keep this small.")]
        [Range(0.0f, 1.0f)]
        public float LightPercent = 0.000001f;

        /// <summary>
        /// What percent of lights created should cast shadows?
        /// </summary>
        [Tooltip("What percent of lights created should cast shadows?")]
        [Range(0.0f, 1.0f)]
        public float LightShadowPercent;

        /// <summary>
        /// Light intensity
        /// </summary>
        [Tooltip("Light intensity")]
        [Range(0.0f, 8.0f)]
        public float LightIntensity = 0.5f;

        /// <summary>
        /// Bounce intensity
        /// </summary>
        [Tooltip("Bounce intensity")]
        [Range(0.0f, 8.0f)]
        public float BounceIntensity;

        /// <summary>
        /// Shadow strength, 0 - 1. 0 means all light, 1 means all shadow
        /// </summary>
        [Tooltip("Shadow strength, 0 means all light, 1 means all shadow")]
        [Range(0.0f, 1.0f)]
        public float ShadowStrength = 1.0f;

        /// <summary>
        /// Shadow bias
        /// </summary>
        [Tooltip("Shadow bias, 0 - 2")]
        [Range(0.0f, 2.0f)]
        public float ShadowBias = 0.05f;

        /// <summary>
        /// Shadow normal bias
        /// </summary>
        [Tooltip("Shadow normal bias, 0 - 3")]
        [Range(0.0f, 3.0f)]
        public float ShadowNormalBias = 0.4f;

        /// <summary>
        /// Light range
        /// </summary>
        [Tooltip("The range of each light created")]
        public float LightRange;

        /// <summary>
        /// Only light up objects that match this layer mask
        /// </summary>
        [Tooltip("Only light objects that match this layer mask")]
        public LayerMask CullingMask = ~0;
        
        /// <summary>
        /// Should light be shown for these parameters?
        /// </summary>
        public bool HasLight
        {
            get { return (LightColor.a > 0.0f && LightIntensity >= 0.01f && LightPercent >= 0.0000001f && LightRange > 0.01f); }
        }
    }

    /// <summary>
    /// Parameters that control lightning bolt behavior
    /// </summary>
    [System.Serializable]
    public class LightningBoltParameters
    {
        /// <summary>
        /// Start of the bolt
        /// </summary>
        public Vector3 Start;

        /// <summary>
        /// End of the bolt
        /// </summary>
        public Vector3 End;

        /// <summary>
        /// Number of generations (0 for just a point light, otherwise 1 - 8). Higher generations have lightning with finer detail but more expensive to create.
        /// </summary>
        public int Generations;

        /// <summary>
        /// How long the bolt should live in seconds
        /// </summary>
        public float LifeTime;

        /// <summary>
        /// How long to wait in seconds before starting the lightning bolt
        /// </summary>
        public float Delay;

        /// <summary>
        /// How chaotic is the lightning? (0 - 1). Higher numbers create more chaotic lightning.
        /// </summary>
        public float ChaosFactor;

        /// <summary>
        /// The width of the trunk
        /// </summary>
        public float TrunkWidth;

        /// <summary>
        /// The ending width of a segment of lightning
        /// </summary>
        public float EndWidthMultiplier = 0.5f;

        /// <summary>
        /// Intensity of glow (0-1)
        /// </summary>
        public float GlowIntensity;

        /// <summary>
        /// Glow width multiplier
        /// </summary>
        public float GlowWidthMultiplier;

        /// <summary>
        /// How forked the lightning should be, 0 for none, 1 for LOTS of forks
        /// </summary>
        public float Forkedness;

        /// <summary>
        /// Used to generate random numbers, can be null. Passing a random with the same seed and parameters will result in the same lightning.
        /// </summary>
        public System.Random Random;

        /// <summary>
        /// The percent of time the lightning should fade in and out (0 - 1). Example: 0.2 would fade in for 20% of the lifetime and fade out for 20% of the lifetime. Set to 0 for no fade.
        /// </summary>
        public float FadePercent;

        /// <summary>
        /// A value between 0 and 1 that determines how fast the lightning should grow over the lifetime. A value of 1 grows slowest, 0 grows instantly
        /// </summary>
        public float GrowthMultiplier;

        /// <summary>
        /// Light parameters, null for none
        /// </summary>
        public LightningLightParameters LightParameters;
    }

    /// <summary>
    /// A group of lightning bolt segments, such as the main trunk of the lightning bolt
    /// </summary>
    public class LightningBoltSegmentGroup
    {
        /// <summary>
        /// Width
        /// </summary>
        public float LineWidth;

        /// <summary>
        /// Start index of the segment to render (for performance, some segments are not rendered and only used for calculations)
        /// </summary>
        public int StartIndex;

        /// <summary>
        /// Generation
        /// </summary>
        public int Generation;

        /// <summary>
        /// Delay before rendering should start
        /// </summary>
        public float Delay;

        /// <summary>
        /// Peak start, the segments should be fully visible at this point
        /// </summary>
        public float PeakStart;

        /// <summary>
        /// Peak end, the segments should start to go away after this point
        /// </summary>
        public float PeakEnd;

        /// <summary>
        /// Total life time the group will be alive in seconds
        /// </summary>
        public float LifeTime;

        /// <summary>
        /// The width can be scaled down to the last segment by this amount if desired
        /// </summary>
        public float EndWidthMultiplier;

        /// <summary>
        /// Total number of active segments
        /// </summary>
        public int SegmentCount { get { return Segments.Count - StartIndex; } }

        /// <summary>
        /// Segments
        /// </summary>
        public readonly List<LightningBoltSegment> Segments = new List<LightningBoltSegment>();

        /// <summary>
        /// Lights
        /// </summary>
        public readonly List<Light> Lights = new List<Light>();

        /// <summary>
        /// Light parameters
        /// </summary>
        public LightningLightParameters LightParameters;
    }

    /// <summary>
    /// A single segment of a lightning bolt
    /// </summary>
    public struct LightningBoltSegment
    {
        public Vector3 Start;
        public Vector3 End;

        public override string ToString()
        {
            return Start.ToString() + ", " + End.ToString();
        }
    }

    /// <summary>
    /// Contains maximum values for a given quality settings
    /// </summary>
    public class LightningQualityMaximum
    {
        /// <summary>
        /// Maximum generations
        /// </summary>
        public int MaximumGenerations { get; set; }

        /// <summary>
        /// Maximum light percent
        /// </summary>
        public float MaximumLightPercent { get; set; }

        /// <summary>
        /// Maximum light shadow percent
        /// </summary>
        public float MaximumShadowPercent { get; set; }
    }

    /// <summary>
    /// Lightning bolt
    /// </summary>
    public class LightningBolt
    {
        /// <summary>
        /// This is subtracted from the initial generations value, and any generation below that cannot have a fork
        /// </summary>
        public static int GenerationWhereForksStopSubtractor = 5;

        /// <summary>
        /// The maximum number of lights to allow for all lightning
        /// </summary>
        public static int MaximumLightCount = 128;

        /// <summary>
        /// The maximum number of lights to create per batch of lightning emitted
        /// </summary>
        public static int MaximumLightsPerBatch = 8;

        /// <summary>
        /// Contains quality settings for different quality levels. By default, this assumes 6 quality levels, so if you have your own
        /// custom quality setting levels, you may want to clear this dictionary out and re-populate it with your own limits
        /// </summary>
        public static readonly Dictionary<int, LightningQualityMaximum> QualityMaximums = new Dictionary<int, LightningQualityMaximum>();

        /// <summary>
        /// Parent game object
        /// </summary>
        public GameObject Parent { get; private set; }

        /// <summary>
        /// The current minimum delay until anything will start rendering
        /// </summary>
        public float MinimumDelay { get; private set; }

        /// <summary>
        /// Is there any glow for any of the lightning bolts?
        /// </summary>
        public bool HasGlow { get; private set; }

        /// <summary>
        /// Is this lightning bolt active any more?
        /// </summary>
        public bool IsActive { get { return elapsedTime < lifeTime; } }

        /// <summary>
        /// The camera the lightning should be visible in
        /// </summary>
        public Camera Camera { get; private set; }

        /// <summary>
        /// True to use world space, false to use local space
        /// </summary>
        public bool UseWorldSpace { get; set; }

        // how long this bolt has been alive
        private float elapsedTime;

        // total life span of this bolt
        private float lifeTime;

        private int generationWhereForksStop;
        private LightningBoltMeshRenderer lightningBoltRenderer;
        private LightningBoltScript script;
        private readonly List<LightningBoltSegmentGroup> segmentGroups = new List<LightningBoltSegmentGroup>();
        private readonly List<LightningBoltSegmentGroup> segmentGroupsWithLight = new List<LightningBoltSegmentGroup>();
        private bool hasLight;

        private static int lightCount;
        private static readonly List<LightningBoltSegmentGroup> groupCache = new List<LightningBoltSegmentGroup>();
        private static readonly List<Light> lightCache = new List<Light>();
        private static readonly List<LightningBolt> lightningBoltCache = new List<LightningBolt>();

        static LightningBolt()
        {
            string[] names = QualitySettings.names;
            for (int i = 0; i < names.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        QualityMaximums[i] = new LightningQualityMaximum { MaximumGenerations = 3, MaximumLightPercent = 0, MaximumShadowPercent = 0.0f };
                        break;
                    case 1:
                        QualityMaximums[i] = new LightningQualityMaximum { MaximumGenerations = 4, MaximumLightPercent = 0, MaximumShadowPercent = 0.0f };
                        break;
                    case 2:
                        QualityMaximums[i] = new LightningQualityMaximum { MaximumGenerations = 5, MaximumLightPercent = 0.1f, MaximumShadowPercent = 0.0f };
                        break;
                    case 3:
                        QualityMaximums[i] = new LightningQualityMaximum { MaximumGenerations = 5, MaximumLightPercent = 0.1f, MaximumShadowPercent = 0.0f };
                        break;
                    case 4:
                        QualityMaximums[i] = new LightningQualityMaximum { MaximumGenerations = 6, MaximumLightPercent = 0.05f, MaximumShadowPercent = 0.1f };
                        break;
                    case 5:
                        QualityMaximums[i] = new LightningQualityMaximum { MaximumGenerations = 7, MaximumLightPercent = 0.025f, MaximumShadowPercent = 0.05f };
                        break;
                    default:
                        QualityMaximums[i] = new LightningQualityMaximum { MaximumGenerations = 8, MaximumLightPercent = 0.025f, MaximumShadowPercent = 0.05f };
                        break;
                }
            }
        }

        public static LightningBolt GetOrCreateLightningBolt()
        {
            if (lightningBoltCache.Count == 0)
            {
                return new LightningBolt();
            }
            LightningBolt b = lightningBoltCache[lightningBoltCache.Count - 1];
            lightningBoltCache.RemoveAt(lightningBoltCache.Count - 1);

            return b;
        }

        public void Initialize(Camera camera, bool useWorldSpace, LightningBoltQualitySetting quality, LightningBoltMeshRenderer lightningBoltRenderer,
            GameObject parent, LightningBoltScript script, ParticleSystem originParticleSystem, ParticleSystem destParticleSystem, ICollection<LightningBoltParameters> parameters)
        {
            // setup properties
            if (parameters == null || lightningBoltRenderer == null || parameters.Count == 0 || script == null)
            {
                return;
            }

            this.UseWorldSpace = useWorldSpace;
            this.lightningBoltRenderer = lightningBoltRenderer;
            this.Parent = parent;
            this.script = script;

            CheckForGlow(parameters);
            lightningBoltRenderer.Begin(this);
            MinimumDelay = float.MaxValue;
            int maxLightsForEachParameters = MaximumLightsPerBatch / parameters.Count;
            foreach (LightningBoltParameters p in parameters)
            {
                ProcessParameters(p, quality, originParticleSystem, destParticleSystem, maxLightsForEachParameters);
            }
            lightningBoltRenderer.End(this);
        }

        public bool Update()
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime > lifeTime)
            {
                Cleanup();
                return false;
            }
            else if (hasLight)
            {
                UpdateLightsForGroups();
            }
            return true;
        }

        public void Cleanup()
        {
            foreach (LightningBoltSegmentGroup g in segmentGroups)
            {
                foreach (Light l in g.Lights)
                {
                    lightCache.Add(l);
                    l.gameObject.SetActive(false);
                    lightCount--;
                }
                g.LightParameters = null;
                g.Segments.Clear();
                g.Lights.Clear();
                g.StartIndex = 0;
                groupCache.Add(g);
            }
            segmentGroups.Clear();
            segmentGroupsWithLight.Clear();
            if (lightningBoltRenderer != null)
            {
                lightningBoltRenderer.Cleanup(this);
                lightningBoltRenderer = null;
            }
            hasLight = false;
            elapsedTime = 0.0f;
            lifeTime = 0.0f;
            lightningBoltCache.Add(this);
        }

        private void ProcessParameters(LightningBoltParameters p, LightningBoltQualitySetting quality,
            ParticleSystem sourceParticleSystem, ParticleSystem destinationParticleSystem, int maxLights)
        {
            MinimumDelay = Mathf.Min(p.Delay, MinimumDelay);
            generationWhereForksStop = p.Generations - GenerationWhereForksStopSubtractor;
            p.GlowIntensity = Mathf.Clamp(p.GlowIntensity, 0.0f, 1.0f);
            p.Random = (p.Random ?? new System.Random(System.Environment.TickCount));
            p.GrowthMultiplier = Mathf.Clamp(p.GrowthMultiplier, 0.0f, 0.999f);
            lifeTime = Mathf.Max(p.LifeTime + p.Delay, lifeTime);
            LightningLightParameters lp = p.LightParameters;
            if (lp != null)
            {
                if ((hasLight |= lp.HasLight))
                {
                    lp.LightPercent = Mathf.Clamp(lp.LightPercent, 0.0000001f, 1.0f);
                    lp.LightShadowPercent = Mathf.Clamp(lp.LightShadowPercent, 0.0f, 1.0f);
                }
                else
                {
                    lp = null;
                }
            }

            if (p.Generations < 1)
            {
                // we just want a point light and we set the trunk width to 0 so the line doesn't show up
                p.TrunkWidth = 0.0f;
                p.Generations = 1;
            }
            else if (p.Generations > 8)
            {
                p.Generations = 8;
            }

            int forkedness = (int)(p.Forkedness * (float)p.Generations);
            int groupIndex = segmentGroups.Count;
            int generation;
            if (quality == LightningBoltQualitySetting.UseScript)
            {
                generation = p.Generations;
            }
            else
            {
                LightningQualityMaximum maximum;
                int level = QualitySettings.GetQualityLevel();
                if (QualityMaximums.TryGetValue(level, out maximum))
                {
                    generation = Mathf.Min(maximum.MaximumGenerations, p.Generations);
                }
                else
                {
                    generation = p.Generations;
                    Debug.LogError("Unable to read lightning quality settings from level " + level.ToString());
                }
            }

            GenerateLightningBolt(p.Start, p.End, generation, generation, 0.0f, 0.0f, forkedness, p);
            RenderLightningBolt(quality, generation, p.Start, p.End, groupIndex, sourceParticleSystem, destinationParticleSystem, p, lp, maxLights);
        }

        private void RenderLightningBolt(LightningBoltQualitySetting quality, int generations, Vector3 start, Vector3 end, int groupIndex,
            ParticleSystem originParticleSystem, ParticleSystem destParticleSystem, LightningBoltParameters parameters,
            LightningLightParameters lp, int maxLights)
        {
            if (segmentGroups.Count == 0 || groupIndex >= segmentGroups.Count)
            {
                return;
            }

            float delayBase = parameters.LifeTime / (float)segmentGroups.Count;
            float minDelayValue = delayBase * 0.9f;
            float maxDelayValue = delayBase * 1.1f;
            float delayDiff = maxDelayValue - minDelayValue;
            parameters.FadePercent = Mathf.Clamp(parameters.FadePercent, 0.0f, 0.5f);

            if (originParticleSystem != null)
            {
                // we have a strike, create a particle where the lightning is coming from
                script.StartCoroutine(GenerateParticle(originParticleSystem, start, parameters.Delay));
            }
            if (destParticleSystem != null)
            {
                script.StartCoroutine(GenerateParticle(destParticleSystem, end, parameters.Delay * 1.1f));
            }

            if (HasGlow)
            {
                lightningBoltRenderer.GlowIntensityMultiplier = parameters.GlowIntensity;
                lightningBoltRenderer.GlowWidthMultiplier = parameters.GlowWidthMultiplier;
            }

            float currentDelayAmount = 0.0f;
            for (int i = groupIndex; i < segmentGroups.Count; i++)
            {
                LightningBoltSegmentGroup group = segmentGroups[i];
                group.Delay = currentDelayAmount + parameters.Delay;
                group.LifeTime = parameters.LifeTime - currentDelayAmount;
                group.PeakStart = group.LifeTime * parameters.FadePercent;
                group.PeakEnd = group.LifeTime - group.PeakStart;
                group.LightParameters = lp;

                lightningBoltRenderer.AddGroup(this, group, parameters.GrowthMultiplier);
                currentDelayAmount += ((float)parameters.Random.NextDouble() * minDelayValue) + delayDiff;

                // create lights only on the main trunk
                if (lp != null && group.Generation == generations)
                {
                    CreateLightsForGroup(group, lp, quality, maxLights, groupIndex);
                }
            }
        }

        private void CreateLightsForGroup(LightningBoltSegmentGroup group, LightningLightParameters lp, LightningBoltQualitySetting quality,
            int maxLights, int groupIndex)
        {
            if (lightCount == MaximumLightCount || maxLights <= 0)
            {
                return;
            }

            segmentGroupsWithLight.Add(group);

            int segmentCount = group.SegmentCount;
            float lightPercent, lightShadowPercent;
            if (quality == LightningBoltQualitySetting.LimitToQualitySetting)
            {
                int level = QualitySettings.GetQualityLevel();
                LightningQualityMaximum maximum;
                if (QualityMaximums.TryGetValue(level, out maximum))
                {
                    lightPercent = Mathf.Min(lp.LightPercent, maximum.MaximumLightPercent);
                    lightShadowPercent = Mathf.Min(lp.LightShadowPercent, maximum.MaximumShadowPercent);
                }
                else
                {
                    Debug.LogError("Unable to read lightning quality for level " + level.ToString());
                    lightPercent = lp.LightPercent;
                    lightShadowPercent = lp.LightShadowPercent;
                }
            }
            else
            {
                lightPercent = lp.LightPercent;
                lightShadowPercent = lp.LightShadowPercent;
            }

            maxLights = Mathf.Max(1, Mathf.Min(maxLights, (int)(segmentCount * lightPercent)));
            int nthLight = Mathf.Max(1, (int)((segmentCount / maxLights)));
            int nthShadows = maxLights - (int)((float)maxLights * lightShadowPercent);

            int nthShadowCounter = nthShadows;

            // add lights evenly spaced
            for (int i = group.StartIndex + (int)(nthLight * 0.5f); i < group.Segments.Count; i += nthLight)
            {
                if (AddLightToGroup(group, lp, i, nthLight, nthShadows, ref maxLights, ref nthShadowCounter))
                {
                    return;
                }
            }

            // Debug.Log("Lightning light count: " + lightCount.ToString());
        }

        private bool AddLightToGroup(LightningBoltSegmentGroup group, LightningLightParameters lp, int segmentIndex,
            int nthLight, int nthShadows, ref int maxLights, ref int nthShadowCounter)
        {
            Light light = CreateLight(group, lp);
            light.gameObject.transform.position = (group.Segments[segmentIndex].Start + group.Segments[segmentIndex].End) * 0.5f;
            if (lp.LightShadowPercent == 0.0f || ++nthShadowCounter < nthShadows)
            {
                light.shadows = LightShadows.None;
            }
            else
            {
                light.shadows = LightShadows.Soft;
                nthShadowCounter = 0;
            }

            // return true if no more lights possible, false otherwise
            return (++lightCount == MaximumLightCount || --maxLights == 0);
        }

        private Light CreateLight(LightningBoltSegmentGroup group, LightningLightParameters lp)
        {
            Light light;
            while (true)
            {
                if (lightCache.Count == 0)
                {
                    GameObject lightningLightObject = new GameObject();
                    lightningLightObject.hideFlags = HideFlags.HideAndDontSave;
                    lightningLightObject.name = "LightningBoltLight";
                    light = lightningLightObject.AddComponent<Light>();
                    light.type = LightType.Point;
                    break;
                }
                else
                {
                    light = lightCache[lightCache.Count - 1];
                    lightCache.RemoveAt(lightCache.Count - 1);
                    if (light == null)
                    {
                        // may have been disposed or the level re-loaded
                        continue;
                    }
                    break;
                }
            }

            light.color = lp.LightColor;
            light.renderMode = lp.RenderMode;
            light.range = lp.LightRange;
            light.bounceIntensity = lp.BounceIntensity;
            light.shadowStrength = lp.ShadowStrength;
            light.shadowBias = lp.ShadowBias;
            light.shadowNormalBias = lp.ShadowNormalBias;
            light.intensity = 0.0f;
            light.gameObject.transform.parent = Parent.transform;
            light.gameObject.SetActive(true);
            group.Lights.Add(light);

            return light;
        }

        private void UpdateLightsForGroups()
        {
            foreach (LightningBoltSegmentGroup group in segmentGroupsWithLight)
            {
                if (elapsedTime < group.Delay)
                {
                    continue;
                }

                // depending on whether we have hit the mid point of our lifetime, fade the light in or out
                float groupElapsedTime = elapsedTime - group.Delay;
                if (groupElapsedTime >= group.PeakStart)
                {
                    if (groupElapsedTime <= group.PeakEnd)
                    {
                        // fully lit
                        foreach (Light l in group.Lights)
                        {
                            l.intensity = group.LightParameters.LightIntensity;
                        }
                    }
                    else
                    {
                        // fading out
                        float lerp = (groupElapsedTime - group.PeakEnd) / (group.LifeTime - group.PeakEnd);
                        foreach (Light l in group.Lights)
                        {
                            l.intensity = Mathf.Lerp(group.LightParameters.LightIntensity, 0.0f, lerp);
                        }
                    }
                }
                else
                {
                    // fading in
                    float lerp = groupElapsedTime / group.PeakStart;
                    foreach (Light l in group.Lights)
                    {
                        l.intensity = Mathf.Lerp(0.0f, group.LightParameters.LightIntensity, lerp);
                    }
                }
            }
        }

        private LightningBoltSegmentGroup CreateGroup()
        {
            LightningBoltSegmentGroup g;
            if (groupCache.Count == 0)
            {
                g = new LightningBoltSegmentGroup();
            }
            else
            {
                int index = groupCache.Count - 1;
                g = groupCache[index];
                groupCache.RemoveAt(index);
            }
            segmentGroups.Add(g);
            return g;
        }

        private IEnumerator GenerateParticle(ParticleSystem p, Vector3 pos, float delay)
        {
            yield return new WaitForSeconds(delay);

            p.transform.position = pos;
            p.Emit((int)p.emissionRate);
        }

        private void GenerateLightningBolt(Vector3 start, Vector3 end, int generation, int totalGenerations, float offsetAmount,
            float lineWidth, int forkedness, LightningBoltParameters parameters)
        {
            if (generation < 1)
            {
                return;
            }

            LightningBoltSegmentGroup group = CreateGroup();
            group.EndWidthMultiplier = parameters.EndWidthMultiplier;
            group.Segments.Add(new LightningBoltSegment { Start = start, End = end });

            // every generation, get the percentage we have gone down and square it, this makes lines thinner
            // and colors dimmer as we go down generations
            float widthAndColorMultiplier = (float)generation / (float)totalGenerations;
            Vector3 randomVector;
            widthAndColorMultiplier *= widthAndColorMultiplier;

            if (offsetAmount <= 0.0f)
            {
                offsetAmount = (end - start).magnitude * parameters.ChaosFactor;
            }
            if (lineWidth <= 0.0f)
            {
                group.LineWidth = parameters.TrunkWidth;
            }
            else
            {
                group.LineWidth = lineWidth * widthAndColorMultiplier;
            }

            group.LineWidth *= widthAndColorMultiplier;
            group.Generation = generation;

            while (generation-- > 0)
            {
                int previousStartIndex = group.StartIndex;
                group.StartIndex = group.Segments.Count;
                for (int i = previousStartIndex; i < group.StartIndex; i++)
                {
                    start = group.Segments[i].Start;
                    end = group.Segments[i].End;

                    // determine a new direction for the split
                    Vector3 midPoint = (start + end) * 0.5f;

                    // adjust the mid point to be the new location
                    RandomVector(ref start, ref end, offsetAmount, parameters.Random, out randomVector);
                    midPoint += randomVector;

                    // add two new segments
                    group.Segments.Add(new LightningBoltSegment { Start = start, End = midPoint });
                    group.Segments.Add(new LightningBoltSegment { Start = midPoint, End = end });

                    if (generation > generationWhereForksStop && generation >= totalGenerations - forkedness)
                    {
                        int branchChance = parameters.Random.Next(0, generation);
                        if (branchChance < forkedness)
                        {
                            // create a branch in the direction of this segment
                            float multiplier = ((float)parameters.Random.NextDouble() * 0.2f) + 0.6f;
                            Vector3 branchVector = (midPoint - start) * multiplier;
                            Vector3 splitEnd = midPoint + branchVector;
                            GenerateLightningBolt(midPoint, splitEnd, generation, totalGenerations, 0.0f, lineWidth, forkedness, parameters);
                        }
                    }
                }

                // halve the distance the lightning can deviate for each generation down
                offsetAmount *= 0.5f;
            }
        }

#if USE_OTHER_LIGHTNING_PERPENDICULAR_ALGORITHM

        private void GetPerpendicularVector(ref Vector3 directionNormalized, out Vector3 side)
        {
            if (directionNormalized == Vector3.zero)
            {
                side = new Vector3(1.0f, 0.0f, 0.0f);
            }
            else
            {
                // use cross product to find any perpendicular vector around directionNormalized:
                // 0 = x * px + y * py + z * pz
                // => pz = -(x * px + y * py) / z
                // for computational stability use the component farthest from 0 to divide by
                float x = directionNormalized.x, y = directionNormalized.y, z = directionNormalized.z;
                float px, py, pz;
                float ax = Mathf.Abs(x), ay = Mathf.Abs(y), az = Mathf.Abs(z);
                if (ax >= ay && ay >= az)
                {
                    // x is the max, so we can pick (py, pz) arbitrarily at (1, 1):
                    py = 1.0f;
                    pz = 1.0f;
                    px = -(y * py + z * pz) / x;
                }
                else if (ay >= az)
                {
                    // y is the max, so we can pick (px, pz) arbitrarily at (1, 1):
                    px = 1.0f;
                    pz = 1.0f;
                    py = -(x * px + z * pz) / y;
                }
                else
                {
                    // z is the max, so we can pick (px, py) arbitrarily at (1, 1):
                    px = 1.0f;
                    py = 1.0f;
                    pz = -(x * px + y * py) / z;
                }
                side = new Vector3(px, py, pz).normalized;
            }
        }

#endif

        private void RandomVector(ref Vector3 start, ref Vector3 end, float offsetAmount, System.Random random, out Vector3 result)
        {
            Vector3 directionNormalized = (end - start).normalized;

            if (Camera == null || !Camera.orthographic)
            {

#if USE_OTHER_LIGHTNING_PERPENDICULAR_ALGORITHM

                Vector3 side;
                GetPerpendicularVector(ref directionNormalized, out side);

#else

                // get perpendicular vector
                Vector3 side = Vector3.Cross(start, end).normalized;

#endif

                // generate random distance
                float distance = (((float)random.NextDouble() + 0.1f) * offsetAmount);

                // get random rotation angle to rotate around the current direction
                float rotationAngle = ((float)random.NextDouble() * 360.0f);

                // rotate around the direction and then offset by the perpendicular vector
                result = Quaternion.AngleAxis(rotationAngle, directionNormalized) * side * distance;
            }
            else
            {
                // ensure that lightning moves in only x, y giving it a more varied appearance than 3D which can move in the z direction too
                Vector3 side = new Vector3(-directionNormalized.y, directionNormalized.x, directionNormalized.z);
                float distance = ((float)random.NextDouble() * offsetAmount * 2.0f) - offsetAmount;
                result = side * distance;
            }
        }

        private void CheckForGlow(IEnumerable<LightningBoltParameters> parameters)
        {
            // we need to know if there is glow so we can choose the glow or non-glow setting in the renderer
            foreach (LightningBoltParameters p in parameters)
            {
                HasGlow = (p.GlowIntensity > 0.0001f && p.GlowWidthMultiplier >= 0.0001f);

                if (HasGlow)
                {
                    break;
                }
            }
        }
    }

        /// <summary>
    /// Quality settings for lightning
    /// </summary>
    public enum LightningBoltQualitySetting
    {
        /// <summary>
        /// Use all settings from the script, ignoring the global quality setting
        /// </summary>
        UseScript,

        /// <summary>
        /// Use the global quality setting to determine lightning quality and maximum number of lights and shadowing
        /// </summary>
        LimitToQualitySetting
    }

    /// <summary>
    /// Rendering mode
    /// </summary>
    public enum LightningBoltRenderMode
    {
        /// <summary>
        /// Renderer with a square billboard glow
        /// </summary>
        MeshRendererSquareBillboardGlow,

        /// <summary>
        /// Renderer with a glow that follows the shape of the line
        /// </summary>
        MeshRendererLineGlow
    }

    public class LightningBoltScript : MonoBehaviour
    {
        private const LightningBoltRenderMode defaultRenderMode = LightningBoltRenderMode.MeshRendererLineGlow;
        private LightningBoltMeshRenderer lightningBoltRenderer;

        [Tooltip("The camera the lightning should be shown in. Defaults to the current camera, or the main camera if current camera is null. If you are using a different " +
            "camera, you may want to put the lightning in it's own layer and cull that layer out of any other cameras.")]
        public Camera Camera;

        [Tooltip("True if you are using world space coordinates for the lightning bolt, false if you are using coordinates relative to the parent game object.")]
        public bool UseWorldSpace = true;

        [Tooltip("Lightning quality setting. This allows setting limits on generations, lights and shadow casting lights based on the global quality setting.")]
        public LightningBoltQualitySetting QualitySetting = LightningBoltQualitySetting.UseScript;

        [Tooltip("Determines how the lightning is rendererd - this needs to be setup before the script starts.")]
        public LightningBoltRenderMode RenderMode = defaultRenderMode;

        [Tooltip("Lightning material for mesh renderer")]
        public Material LightningMaterialMesh;

        [Tooltip("Lightning material for mesh renderer, without glow")]
        public Material LightningMaterialMeshNoGlow;

        [Tooltip("The texture to use for the lightning bolts, or null for the material default texture.")]
        public Texture2D LightningTexture;

        [Tooltip("Particle system to play at the point of emission (start). 'Emission rate' particles will be emitted all at once.")]
        public ParticleSystem LightningOriginParticleSystem;

        [Tooltip("Particle system to play at the point of impact (end). 'Emission rate' particles will be emitted all at once.")]
        public ParticleSystem LightningDestinationParticleSystem;

        [Tooltip("Tint color for the lightning")]
        public Color LightningTintColor = Color.white;

        [Tooltip("Tint color for the lightning glow")]
        public Color GlowTintColor = new Color(0.1f, 0.2f, 1.0f, 1.0f);

        [Tooltip("Jitter multiplier to randomize lightning size. Jitter depends on trunk width and will make the lightning move rapidly and jaggedly, " +
            "giving a more lively and sometimes cartoony feel. Jitter may be shared with other bolts depending on materials. If you need different " +
            "jitters for the same material, create a second script object.")]
        public float JitterMultiplier = 0.0f;

        [Tooltip("Built in turbulance based on the direction of each segment. Small values usually work better, like 0.2.")]
        public float Turbulence = 0.0f;

		[Tooltip("Global turbulence velocity for this script")]
		public Vector3 TurbulenceVelocity = Vector3.zero;

        [Tooltip("The render queue for the lightning. -1 for default.")]
        public int RenderQueue = -1;

        private Material lightningMaterialMeshInternal;
        private Material lightningMaterialMeshNoGlowInternal;
        private Texture2D lastLightningTexture;
        private LightningBoltRenderMode lastRenderMode = (LightningBoltRenderMode)0x7FFFFFFF;
        private readonly List<LightningBolt> bolts = new List<LightningBolt>();
        private readonly LightningBoltParameters[] oneParameterArray = new LightningBoltParameters[1];

        private void UpdateMaterialsForLastTexture()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            lightningMaterialMeshInternal = new Material(LightningMaterialMesh);
            lightningMaterialMeshNoGlowInternal = new Material(LightningMaterialMeshNoGlow);

            if (LightningTexture != null)
            {
                lightningMaterialMeshInternal.SetTexture("_MainTex", LightningTexture);
                lightningMaterialMeshNoGlowInternal.SetTexture("_MainTex", LightningTexture);
            }

            lastRenderMode = (LightningBoltRenderMode)0x7FFFFFFF;

            CreateRenderer();
        }

        private void CreateRenderer()
        {
            if (RenderMode == lastRenderMode)
            {
                return;
            }
            lastRenderMode = RenderMode;
            lightningBoltRenderer = new LightningBoltMeshRenderer();
            lightningBoltRenderer.MaterialNoGlow = lightningMaterialMeshNoGlowInternal;

            if (RenderMode == LightningBoltRenderMode.MeshRendererSquareBillboardGlow)
            {
                lightningMaterialMeshInternal.DisableKeyword("USE_LINE_GLOW");
            }
            else
            {
                lightningMaterialMeshInternal.EnableKeyword("USE_LINE_GLOW");
            }

            lightningBoltRenderer.Script = this;
            lightningBoltRenderer.Material = lightningMaterialMeshInternal;
        }

        private void UpdateTexture()
        {
            if (LightningTexture != null && LightningTexture != lastLightningTexture)
            {
                lastLightningTexture = LightningTexture;
                UpdateMaterialsForLastTexture();
            }
            else
            {
                CreateRenderer();
            }
        }

        private void UpdateShaderParameters()
        {
            lightningMaterialMeshInternal.SetColor("_TintColor", LightningTintColor);
            lightningMaterialMeshInternal.SetColor("_GlowTintColor", GlowTintColor);
            lightningMaterialMeshInternal.SetFloat("_JitterMultiplier", JitterMultiplier);
            lightningMaterialMeshInternal.SetFloat("_Turbulence", Turbulence);
			lightningMaterialMeshInternal.SetVector("_TurbulenceVelocity", TurbulenceVelocity);
            lightningMaterialMeshInternal.renderQueue = RenderQueue;
            lightningMaterialMeshNoGlowInternal.SetColor("_TintColor", LightningTintColor);
            lightningMaterialMeshNoGlowInternal.SetFloat("_JitterMultiplier", JitterMultiplier);
            lightningMaterialMeshNoGlowInternal.SetFloat("_Turbulence", Turbulence);
			lightningMaterialMeshNoGlowInternal.SetVector("_TurbulenceVelocity", TurbulenceVelocity);
            lightningMaterialMeshNoGlowInternal.renderQueue = RenderQueue;
        }

        private void OnDestroy()
        {
            foreach (LightningBolt bolt in bolts)
            {
                bolt.Cleanup();
            }
        }

        protected virtual void Start()
        {
            if (LightningMaterialMesh == null || LightningMaterialMeshNoGlow == null)
            {
                Debug.LogError("Must assign all lightning materials");
            }
            UpdateMaterialsForLastTexture();

            if (Camera == null)
            {
                Camera = Camera.current;
                if (Camera == null)
                {
                    Camera = Camera.main;
                }
            }
        }

        protected virtual void Update()
        {
            if (bolts.Count == 0)
            {
                return;
            }

            UpdateShaderParameters();
            for (int i = bolts.Count - 1; i >= 0; i--)
            {
                // update each lightning bolt, removing it if update returns false
                if (!bolts[i].Update())
                {
                    bolts.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Create a lightning bolt
        /// </summary>
        /// <param name="p">Lightning bolt creation parameters</param>
        public virtual void CreateLightningBolt(LightningBoltParameters p)
        {
            if (p != null)
            {
                UpdateTexture();
                oneParameterArray[0] = p;
                LightningBolt bolt = LightningBolt.GetOrCreateLightningBolt();
                bolt.Initialize(Camera, UseWorldSpace, QualitySetting, lightningBoltRenderer, gameObject, this,
                    LightningOriginParticleSystem, LightningDestinationParticleSystem, oneParameterArray);
                bolts.Add(bolt);
            }
        }

        /// <summary>
        /// Create multiple lightning bolts, attempting to batch them into as few draw calls as possible
        /// </summary>
        /// <param name="parameters">Lightning bolt creation parameters</param>
        public void CreateLightningBolts(ICollection<LightningBoltParameters> parameters)
        {
            if (parameters != null)
            {
                UpdateTexture();
                LightningBolt bolt = LightningBolt.GetOrCreateLightningBolt();
                bolt.Initialize(Camera, UseWorldSpace, QualitySetting, lightningBoltRenderer, gameObject, this,
                    LightningOriginParticleSystem, LightningDestinationParticleSystem, parameters);
                bolts.Add(bolt);
            }
        }
    }
}