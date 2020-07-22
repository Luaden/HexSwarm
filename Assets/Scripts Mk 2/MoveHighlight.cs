using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveHighlight : IMoveHighlight
{
    [SerializeField] protected List<Vector3Int> movementRange;
    public IEnumerable<Vector3Int> MovementRange => movementRange;
    [SerializeField] protected List<Vector3Int> damageRange;
    public IEnumerable<Vector3Int> DamageRange => damageRange;
    public MoveHighlight() : this(Array.Empty<Vector3Int>(), Array.Empty<Vector3Int>())  { }
    public MoveHighlight(IEnumerable<Vector3Int> moves, IEnumerable<Vector3Int> attack)
    {
        movementRange = new List<Vector3Int>(moves);
        damageRange = new List<Vector3Int>(attack);
    }
}

