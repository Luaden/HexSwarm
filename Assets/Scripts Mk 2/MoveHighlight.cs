using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveHighlight : IMoveHighlight
{
    protected HashSet<Vector3Int> movementRange;
    public IEnumerable<Vector3Int> MovementRange => movementRange;
    protected HashSet<Vector3Int> damageRange;
    public IEnumerable<Vector3Int> DamageRange => damageRange;
    public MoveHighlight(IEnumerable<Vector3Int> moves, IEnumerable<Vector3Int> attack)
    {
        movementRange = new HashSet<Vector3Int>(moves);
        damageRange = new HashSet<Vector3Int>(attack);
    }
}

