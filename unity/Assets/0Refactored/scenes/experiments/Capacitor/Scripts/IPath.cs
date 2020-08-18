using System.Collections.Generic;
using UnityEngine;

public interface IPath
{
    List<Vector3> GetNodes(bool reverseOrder = false);
}
