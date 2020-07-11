using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathfinder
{
    IEnumerable<Vector3Int> DirectPath(Vector3Int origin, Vector3Int destination);

    IEnumerable<Vector3Int> AvoidUnitsPath(Vector3Int origin, Vector3Int destination);
}
