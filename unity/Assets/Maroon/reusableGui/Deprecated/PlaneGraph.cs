//-----------------------------------------------------------------------------
// Graph.cs
//
// Script which handles the drawing of the graph
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;

namespace UI
{
    /// <summary>
    /// Script which handles the drawing of the graph on a plane
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class PlaneGraph : Graph
    {
        private MeshFilter _meshFilter;

        private Renderer _renderer;

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
            _meshFilter = GetComponent<MeshFilter>();

            height = _meshFilter.mesh.bounds.size.z;
            width = _meshFilter.mesh.bounds.size.x;

            Initialize();
            _renderer.material.mainTexture = texture;
        }
    }
}
