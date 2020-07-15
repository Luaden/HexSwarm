using System;
using System.Collections.Generic;
using UnityEngine;


public interface IMoveHighlight
{
    IEnumerable<Vector3Int> MovementRange { get; }
    IEnumerable<Vector3Int> DamageRange { get; }
}
