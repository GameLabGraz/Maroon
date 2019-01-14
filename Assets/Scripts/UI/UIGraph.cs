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
using UnityEngine.UI;

namespace UI
{
    /// <inheritdoc />
    /// <summary>
    /// Script which handles the drawing of the graph in a ui panel
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class UIGraph : Graph
    {
        private RectTransform _objectRectTransform;
        private RawImage _image;

        private void Start()
        {
            _objectRectTransform = GetComponent<RectTransform>();
            _image = GetComponent<RawImage>();

            height = _objectRectTransform.rect.height;
            width = _objectRectTransform.rect.width;

            Initialize();
            _image.texture = texture;
        }

        private void Update()
        {
            if (height.Equals(_objectRectTransform.rect.height) && width.Equals(_objectRectTransform.rect.width))
                return;

            height = _objectRectTransform.rect.height;
            width = _objectRectTransform.rect.width;
            ResetObject();
        }
    }
}
